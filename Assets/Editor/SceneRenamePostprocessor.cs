#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TheWorstGameInHistory.Editor
{
    public class SceneRenamePostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromAssetPaths)
        {
            for (var i = 0; i < movedAssets.Length; i++)
            {
                var to = movedAssets[i];
                var from = movedFromAssetPaths[i];
                if (!to.EndsWith(".unity")) continue;

                Debug.Log($"SceneRenamePostprocessor: 检测到场景已移动，从 '{from}' 到 '{to}'");

                var oldName = PathGetFileNameWithoutExtension(from);
                var newName = PathGetFileNameWithoutExtension(to);

                oldName = SceneInitUtility.SanitizeName(oldName);
                newName = SceneInitUtility.SanitizeName(newName);

                Debug.Log($"SceneRenamePostprocessor: 旧场景规范化名称 '{oldName}'，新 '{newName}'");

                UpdateSceneInitializationIndex(to, newName);
                RenameOpenScript(oldName, newName);
            }
        }

        private static void UpdateSceneInitializationIndex(string scenePath, string newSceneName)
        {
            Debug.Log("SceneRenamePostprocessor: 正在更新场景中的 InitializationIndex: " + scenePath);
            var scene = EditorSceneManager.GetSceneByPath(scenePath);
            var openedAdditive = false;
            if (!scene.isLoaded)
            {
                Debug.Log("SceneRenamePostprocessor: 场景未加载，正在以 Additive 打开: " + scenePath);
                scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                openedAdditive = true;
            }

            if (scene.isLoaded)
            {
                var rootObjs = scene.GetRootGameObjects();
                var initGo = rootObjs.FirstOrDefault(go => go.name == "Initialization");
                if (initGo != null)
                {
                    Debug.Log("SceneRenamePostprocessor: 在场景中找到 'Initialization' GameObject。");
                    // 查找 InitializationIndex 组件
                    var comp = initGo.GetComponents<Component>()
                        .FirstOrDefault(c => c.GetType().Name == "InitializationIndex");
                    if (comp != null)
                    {
                        var compType = comp.GetType();
                        var field = compType.GetField("index");
                        if (field != null)
                        {
                            field.SetValue(comp, newSceneName);
                            Debug.Log($"SceneRenamePostprocessor: 已将 InitializationIndex.index 设置为 '{newSceneName}'");
                            EditorSceneManager.MarkSceneDirty(scene);
                            EditorSceneManager.SaveScene(scene);
                            Debug.Log("SceneRenamePostprocessor: 更新 InitializationIndex 后已保存场景。");
                        }
                        else
                        {
                            Debug.LogWarning("SceneRenamePostprocessor: InitializationIndex 没有 'index' 字段。");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("SceneRenamePostprocessor: 场景中未找到 InitializationIndex 组件。");
                    }
                }
                else
                {
                    Debug.LogWarning("SceneRenamePostprocessor: 场景中未找到 'Initialization' GameObject。");
                }
            }

            if (openedAdditive)
            {
                EditorSceneManager.CloseScene(scene, true);
                Debug.Log("SceneRenamePostprocessor: 已关闭以 Additive 打开的场景: " + scenePath);
            }
        }

        private static void RenameOpenScript(string oldName, string newName)
        {
            Debug.Log($"SceneRenamePostprocessor: 正在重命名 Open 脚本，从 Open{oldName}.cs 到 Open{newName}.cs");
            var folderAsset = "Assets/HotUpdate/Initialize";
            var oldFileAsset = PathCombine(folderAsset, $"Open{oldName}.cs");
            var newFileAsset = PathCombine(folderAsset, $"Open{newName}.cs");

            var fullDir = Path.Combine(Application.dataPath, "HotUpdate", "Initialize");
            var oldFull = Path.Combine(fullDir, $"Open{oldName}.cs");
            var newFull = Path.Combine(fullDir, $"Open{newName}.cs");

            if (!File.Exists(oldFull))
            {
                Debug.LogWarning("SceneRenamePostprocessor: 未找到旧的 Open 文件: " + oldFull + "。如有需要将创建新文件。");
                // 如果旧文件不存在，创建新文件（避免丢失）
                if (!File.Exists(newFull))
                {
                    var className = $"Open{newName}";
                    var content = "using UnityEngine;\n\npublic class " + className + " : Open\n{\n    // 自动生成\n}\n";
                    File.WriteAllText(newFull, content);
                    AssetDatabase.ImportAsset(newFileAsset);
                    AssetDatabase.Refresh();
                    Debug.Log("SceneRenamePostprocessor: 已创建新的 Open 文件: " + newFileAsset);
                }

                return;
            }

            // 读取并替换类名
            var text = File.ReadAllText(oldFull);
            var oldClass = $"class Open{oldName}";
            var newClass = $"class Open{newName}";
            if (text.Contains(oldClass))
            {
                text = text.Replace(oldClass, newClass);
                // 写回到 oldFull 以便后续移动时包含新类名
                File.WriteAllText(oldFull, text);
                Debug.Log("SceneRenamePostprocessor: 已在文件内部替换类名。");
            }

            // 使用 AssetDatabase 移动资产
            var moveResult = AssetDatabase.MoveAsset(oldFileAsset, newFileAsset);
            if (!string.IsNullOrEmpty(moveResult))
            {
                Debug.LogWarning("SceneRenamePostprocessor: AssetDatabase.MoveAsset 返回错误: " + moveResult +
                                 "。回退为复制/删除。");
                // 如果移动失败，则尝试写入新文件并删除旧文件
                File.WriteAllText(newFull, text);
                try
                {
                    File.Delete(oldFull);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("SceneRenamePostprocessor: Failed to delete old file: " + ex.Message);
                }

                AssetDatabase.ImportAsset(newFileAsset);
            }
            else
            {
                Debug.Log("SceneRenamePostprocessor: 成功将资产移动到: " + newFileAsset);
            }

            AssetDatabase.Refresh();
        }

        private static string PathCombine(string a, string b)
        {
            return (a + "/" + b).Replace("\\", "/");
        }

        private static string PathGetFileNameWithoutExtension(string path)
        {
            var fn = path.Replace("\\", "/").Split('/').LastOrDefault();
            if (string.IsNullOrEmpty(fn)) return "";
            var idx = fn.LastIndexOf('.');
            return idx >= 0 ? fn.Substring(0, idx) : fn;
        }
    }
}
#endif