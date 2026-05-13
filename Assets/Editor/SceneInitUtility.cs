#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheWorstGameInHistory.Editor
{
    [InitializeOnLoad]
    public static class SceneInitUtility
    {
        private static readonly string TargetFolderAssetPath = "Assets/HotUpdate/Initialize";

        private static string lastHierarchyHandledScene = "";

        static SceneInitUtility()
        {
            EditorSceneManager.newSceneCreated += OnNewSceneCreated;
        }

        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethodRegistration()
        {
            // ensure no duplicate subscriptions
            EditorSceneManager.newSceneCreated -= OnNewSceneCreated;
            EditorSceneManager.newSceneCreated += OnNewSceneCreated;
            EditorSceneManager.sceneOpened -= OnSceneOpened;
            EditorSceneManager.sceneOpened += OnSceneOpened;
            // fallback: watch hierarchy changes (helps catch new unsaved scenes in some editor versions/workflows)
            // 作为回退：监听层级变化（有些编辑器版本或工作流下可以捕捉未保存新场景）
            EditorApplication.hierarchyChanged -= OnHierarchyChangedFallback;
            EditorApplication.hierarchyChanged += OnHierarchyChangedFallback;
        }

        private static void OnHierarchyChangedFallback()
        {
            // Debounce: only handle when active scene changes from last handled
            var active = EditorSceneManager.GetActiveScene();
            if (!active.isLoaded) return;
            var key = (active.path ?? "<unsaved>") + "|" + active.name;
            if (key == lastHierarchyHandledScene) return;
            // If the scene lacks Initialization root, attempt to initialize — this helps when other events didn't fire
            var root = active.GetRootGameObjects();
            var hasInit = root.Any(go => go.name == "Initialization");
            if (!hasInit)
            {
                Debug.Log(
                    $"SceneInitUtility: 层级变化回退触发，场景 '{active.name}' (path='{active.path}') 中未找到 'Initialization'，正在应用初始化。");
                try
                {
                    var sceneName = SanitizeName(active.name);
                    var changed = CreateInitializationObjectIfMissing(active, sceneName);
                    CreateOpenScriptIfMissing(sceneName);
                    if (changed && !string.IsNullOrEmpty(active.path))
                    {
                        EditorSceneManager.SaveScene(active);
                        Debug.Log("SceneInitUtility: 层级变化回退已应用初始化并已保存。");
                    }
                    else
                    {
                        Debug.Log("SceneInitUtility: 层级变化回退已应用初始化（无实际更改或无需保存）。");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("SceneInitUtility: 层级变化回退异常: " + ex.Message);
                }
            }

            lastHierarchyHandledScene = key;
        }

        private static void OnNewSceneCreated(Scene scene, NewSceneSetup setup, NewSceneMode mode)
        {
            var sceneName = SanitizeName(scene.name);
            Debug.Log($"SceneInitUtility: 新场景已创建 '{scene.name}' (mode={mode})。规范化名称: '{sceneName}'");
            var changed = CreateInitializationObjectIfMissing(scene, sceneName);
            CreateOpenScriptIfMissing(sceneName);
            if (changed && !string.IsNullOrEmpty(scene.path))
            {
                EditorSceneManager.SaveScene(scene);
                Debug.Log("SceneInitUtility: 新场景初始化已保存。");
            }
        }

        private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            Debug.Log(
                $"SceneInitUtility: 场景已打开: name={scene.name}, path={scene.path}, mode={mode}, isLoaded={scene.isLoaded}");
            // Always attempt to ensure initialization for opened scenes (idempotent)
            try
            {
                var sceneName = SanitizeName(scene.name);
                Debug.Log($"SceneInitUtility: 为已打开场景应用初始化 '{scene.name}' (规范化 '{sceneName}')。");
                var changed = CreateInitializationObjectIfMissing(scene, sceneName);
                CreateOpenScriptIfMissing(sceneName);
                if (changed && !string.IsNullOrEmpty(scene.path))
                {
                    EditorSceneManager.SaveScene(scene);
                    Debug.Log("SceneInitUtility: 打开场景的初始化已保存。");
                }
                else
                {
                    Debug.Log("SceneInitUtility: 打开场景的初始化无需保存（未做实际修改）。");
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("SceneInitUtility: 处理 OnSceneOpened 时发生异常: " + ex.Message);
            }
        }

        [MenuItem("工具/场景初始化/对当前场景应用初始化")]
        public static void ApplyToActiveSceneTest()
        {
            var scene = EditorSceneManager.GetActiveScene();
            Debug.Log($"SceneInitUtility: 手动测试：对活动场景应用初始化: name={scene.name}, path={scene.path}");
            var sceneName = SanitizeName(scene.name);
            var changed = CreateInitializationObjectIfMissing(scene, sceneName);
            CreateOpenScriptIfMissing(sceneName);
            if (changed && !string.IsNullOrEmpty(scene.path))
            {
                EditorSceneManager.SaveScene(scene);
                Debug.Log("SceneInitUtility: 手动测试已保存修改。");
            }
            else
            {
                Debug.Log("SceneInitUtility: 手动测试无实际修改或无需保存。");
            }
        }

        // 将返回值改为 bool，表示是否对场景做了实际修改
        public static bool CreateInitializationObjectIfMissing(Scene scene, string sceneName)
        {
            var openedAdditive = false;
            var changed = false;
            // 如果 scene 未加载但有路径，尝试以 Additive 打开；newSceneCreated 时通常已加载且没有 path
            if (!scene.isLoaded && !string.IsNullOrEmpty(scene.path))
            {
                Debug.Log("SceneInitUtility: 场景未加载，正在以 Additive 打开: " + scene.path);
                scene = EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Additive);
                openedAdditive = true;
            }

            var rootObjs = scene.GetRootGameObjects();
            var initGo = rootObjs.FirstOrDefault(go => go.name == "Initialization");
            if (initGo == null)
            {
                Debug.Log("SceneInitUtility: 未找到 'Initialization' GameObject。正在创建。");
                initGo = new GameObject("Initialization");
                SceneManager.MoveGameObjectToScene(initGo, scene);
                changed = true;
            }
            else
            {
                Debug.Log("SceneInitUtility: 找到现有的 'Initialization' GameObject。");
            }

            // 在组件列表中查找名为 InitializationIndex 的组件（避免对类型的直接编译依赖）
            var comp = initGo.GetComponents<Component>().FirstOrDefault(c => c.GetType().Name == "InitializationIndex");
            if (comp == null)
            {
                Debug.Log("SceneInitUtility: 未找到 InitializationIndex 组件。尝试通过反射添加。");
                // 尝试通过反射查找类型并添加
                var type = FindTypeByName("InitializationIndex");
                if (type != null)
                {
                    initGo.AddComponent(type);
                    comp = initGo.GetComponents<Component>()
                        .FirstOrDefault(c => c.GetType().Name == "InitializationIndex");
                    Debug.Log("SceneInitUtility: 已通过反射添加 InitializationIndex 组件。");
                    changed = true;
                }
                else
                {
                    Debug.LogWarning("SceneInitUtility: 在已加载的程序集未找到 InitializationIndex 类型，未添加组件。");
                }
            }
            else
            {
                Debug.Log("SceneInitUtility: 找到现有的 InitializationIndex 组件。");
            }

            if (comp != null)
            {
                var compType = comp.GetType();
                var field = compType.GetField("index");
                if (field != null)
                {
                    var current = field.GetValue(comp) as string;
                    if (current != sceneName)
                    {
                        field.SetValue(comp, sceneName);
                        Debug.Log($"SceneInitUtility: 已将 InitializationIndex.index 设置为 '{sceneName}'。");
                        changed = true;
                    }
                    else
                    {
                        Debug.Log("SceneInitUtility: InitializationIndex.index 已与目标值相同，未修改。");
                    }
                }
            }

            if (changed) EditorSceneManager.MarkSceneDirty(scene);

            if (openedAdditive)
            {
                if (changed)
                {
                    EditorSceneManager.SaveScene(scene);
                    Debug.Log("SceneInitUtility: 已保存因修改而以 Additive 打开的场景。");
                }

                EditorSceneManager.CloseScene(scene, true);
                Debug.Log("SceneInitUtility: 已关闭因修改而以 Additive 打开的场景。");
            }

            return changed;
        }

        // 在当前 AppDomain 中查找类型名（只匹配简单名）
        private static Type FindTypeByName(string typeName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in assemblies)
                try
                {
                    var t = asm.GetTypes().FirstOrDefault(x => x.Name == typeName);
                    if (t != null) return t;
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("SceneInitUtility: FindTypeByName exception: " + ex.Message);
                }

            return null;
        }

        public static void CreateOpenScriptIfMissing(string sceneName)
        {
            // 确保目录存在（使用 Application.dataPath 得到磁盘路径）
            var fullDir = Path.Combine(Application.dataPath, "HotUpdate", "Initialize");
            if (!Directory.Exists(fullDir))
            {
                Debug.Log("SceneInitUtility: 正在创建目录: " + fullDir);
                Directory.CreateDirectory(fullDir);
                // ImportAsset on folder path to ensure Unity recognizes it
                if (!AssetDatabase.IsValidFolder(TargetFolderAssetPath))
                {
                    var parent = "Assets/HotUpdate";
                    if (!AssetDatabase.IsValidFolder(parent)) AssetDatabase.CreateFolder("Assets", "HotUpdate");
                    AssetDatabase.CreateFolder(parent, "Initialize");
                }

                AssetDatabase.ImportAsset(TargetFolderAssetPath);
            }

            var fileName = $"Open{sceneName}.cs";
            var assetPath = PathCombine(TargetFolderAssetPath, fileName); // Assets/... path
            var fullPath = Path.Combine(fullDir, fileName);

            if (File.Exists(fullPath))
            {
                Debug.Log("SceneInitUtility: Open 脚本已存在: " + assetPath);
                return;
            }

            Debug.Log("SceneInitUtility: 正在创建 Open 脚本: " + assetPath);
            var className = $"Open{sceneName}";
            var content = "using UnityEngine;\n\npublic class " + className + " : Open\n{\n    // 自动生成\n}\n";
            File.WriteAllText(fullPath, content);

            AssetDatabase.ImportAsset(assetPath);
            AssetDatabase.Refresh();
            Debug.Log("SceneInitUtility: Open 脚本已创建并导入: " + assetPath);
        }

        // Menu helper: 批量确保所有项目中的场景都有 Initialization 和对应的 Open 脚本
        [MenuItem("工具/场景初始化/确保所有场景都带初始化脚本")]
        public static void EnsureAllProjectScenes()
        {
            // 仅在 Assets/Scene 目录下查找场景
            var targetFolder = "Assets/Scenes";
            var guids = AssetDatabase.FindAssets("t:Scene", new[] { targetFolder });
            Debug.Log($"SceneInitUtility: EnsureAllProjectScenes 扫描文件夹 '{targetFolder}'，找到 {guids.Length} 个场景资源。");
            for (var i = 0; i < guids.Length; i++)
            {
                var scenePath = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (string.IsNullOrEmpty(scenePath)) continue;

                Debug.Log("SceneInitUtility: 正在处理 Assets/Scenes 中的场景: " + scenePath);
                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                try
                {
                    var rawName = Path.GetFileNameWithoutExtension(scenePath);
                    var sceneName = SanitizeName(rawName);
                    var changed = CreateInitializationObjectIfMissing(scene, sceneName);
                    CreateOpenScriptIfMissing(sceneName);
                    if (changed)
                    {
                        EditorSceneManager.SaveScene(scene);
                        Debug.Log("SceneInitUtility: 场景已处理并保存: " + scenePath);
                    }
                    else
                    {
                        Debug.Log("SceneInitUtility: 场景已处理但无实际修改: " + scenePath);
                    }
                }
                finally
                {
                    EditorSceneManager.CloseScene(scene, true);
                }
            }

            AssetDatabase.Refresh();
            Debug.Log("SceneInitUtility: EnsureAllProjectScenes 完成（仅限 Assets/Scenes）。");
        }

        // 保证类名合法：仅保留字母数字和下划线，不能以数字开头
        public static string SanitizeName(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return "Scene";
            var s = Regex.Replace(raw, "[^\\w]", "_");
            if (Regex.IsMatch(s, "^\\d")) s = "_" + s;
            return s;
        }

        private static string PathCombine(string a, string b)
        {
            return (a + "/" + b).Replace("\\", "/");
        }
    }
}
#endif