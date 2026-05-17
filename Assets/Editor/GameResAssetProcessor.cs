using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace HotUpdate.Utility.Editor
{
    /// <summary>
    /// 监听 GameRes 文件夹变化，自动更新 AddressableKeys.cs 并添加到 Addressable 系统
    /// </summary>
    public class GameResAssetProcessor : AssetPostprocessor
    {
        private static readonly string GameResPath = "Assets/GameRes";
        private static readonly string OutputPath = "Assets/HotUpdate/Key/AddressableKeys.cs";
        private static readonly string AutoGenHeader = "// 此文件由 GameResAssetProcessor 自动生成\n// 请勿手动修改此文件\n";
        private static readonly string DefaultGroupName = "Default Local Group";

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            bool shouldRegenerate = false;
            List<string> newGameResAssets = new List<string>();

            // 检查导入的资产
            foreach (var asset in importedAssets)
            {
                if (asset.StartsWith(GameResPath) && !asset.EndsWith(".meta"))
                {
                    shouldRegenerate = true;
                    newGameResAssets.Add(asset);

                    // 自动添加到 Addressable 系统
                    AddToAddressable(asset);
                }
            }

            // 检查删除的资产
            foreach (var asset in deletedAssets)
            {
                if (asset.StartsWith(GameResPath) && !asset.EndsWith(".meta"))
                {
                    shouldRegenerate = true;
                    // 从 Addressable 系统中移除
                    RemoveFromAddressable(asset);
                }
            }

            // 检查移动的资产
            foreach (var asset in movedAssets)
            {
                if (asset.StartsWith(GameResPath) && !asset.EndsWith(".meta"))
                {
                    shouldRegenerate = true;
                    AddToAddressable(asset);
                }
            }

            if (shouldRegenerate)
            {
                GenerateAddressableKeys();
            }
        }

        [MenuItem("工具/重置索引")]
        public static void ResetIndexMenu()
        {
            // 先将所有 GameRes 文件添加到 Addressable
            AddAllGameResToAddressable();

            // 然后生成 AddressableKeys
            GenerateAddressableKeys();

            Debug.Log("索引重置完成：已将所有 GameRes 文件添加到 Addressable 并生成 AddressableKeys");
        }

        [MenuItem("Tools/生成 AddressableKeys")]
        public static void GenerateAddressableKeysMenu()
        {
            GenerateAddressableKeys();
        }

        [MenuItem("Tools/清理 Addressable 中不存在的 GameRes 文件")]
        public static void CleanupMissingAddressableEntries()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                EditorUtility.DisplayDialog("错误", "Addressable Asset Settings 未找到", "确定");
                return;
            }

            if (!EditorUtility.DisplayDialog("确认清理", "这将删除 Addressable 中所有指向 GameRes 但文件已不存在的条目。\n\n确定要继续吗？", "确定", "取消"))
            {
                return;
            }

            int removedCount = 0;
            var group = settings.groups.FirstOrDefault(g => g.Name == DefaultGroupName);

            if (group != null)
            {
                EditorUtility.DisplayProgressBar("清理 Addressable", "正在检查文件...", 0);

                try
                {
                    var entriesToRemove = new List<AddressableAssetEntry>();

                    foreach (var entry in group.entries)
                    {
                        var assetPath = AssetDatabase.GUIDToAssetPath(entry.guid);
                        if (string.IsNullOrEmpty(assetPath) || !File.Exists(assetPath))
                        {
                            entriesToRemove.Add(entry);
                        }
                    }

                    for (int i = 0; i < entriesToRemove.Count; i++)
                    {
                        float progress = (float)i / entriesToRemove.Count;
                        EditorUtility.DisplayProgressBar("清理 Addressable", $"正在删除: {i + 1}/{entriesToRemove.Count}", progress);

                        settings.RemoveAssetEntry(entriesToRemove[i].guid);
                        removedCount++;
                    }

                    AssetDatabase.SaveAssets();

                    EditorUtility.ClearProgressBar();
                    Debug.Log($"清理完成：已删除 {removedCount} 个不存在的 Addressable 条目");
                    EditorUtility.DisplayDialog("完成", $"已删除 {removedCount} 个不存在的 Addressable 条目", "确定");
                }
                catch (System.Exception e)
                {
                    EditorUtility.ClearProgressBar();
                    Debug.LogError($"清理时出错: {e.Message}");
                }
            }
            else
            {
                EditorUtility.ClearProgressBar();
                Debug.LogWarning("未找到默认 Addressable 组");
            }
        }

        [MenuItem("Tools/显示 GameRes 统计信息")]
        public static void ShowStatistics()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                EditorUtility.DisplayDialog("错误", "Addressable Asset Settings 未找到", "确定");
                return;
            }

            var gameResFolder = new DirectoryInfo(GameResPath);
            if (!gameResFolder.Exists)
            {
                EditorUtility.DisplayDialog("错误", $"GameRes 文件夹不存在: {GameResPath}", "确定");
                return;
            }

            // 使用 AssetDatabase 获取所有 GameRes 文件
            var allAssetPaths = AssetDatabase.FindAssets("", new[] { GameResPath })
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Where(path => !string.IsNullOrEmpty(path) && path.StartsWith(GameResPath) && !path.EndsWith(".meta") && !Directory.Exists(path))
                .ToArray();

            // 统计 Addressable 中的 GameRes 条目
            var group = settings.groups.FirstOrDefault(g => g.Name == DefaultGroupName);
            int addressableCount = 0;
            var notInAddressable = new List<string>();

            if (group != null)
            {
                foreach (var entry in group.entries)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(entry.guid);
                    if (!string.IsNullOrEmpty(assetPath) && assetPath.StartsWith(GameResPath))
                    {
                        addressableCount++;
                    }
                }
            }

            // 找出未添加到 Addressable 的文件
            foreach (var assetPath in allAssetPaths)
            {
                var entry = settings.FindAssetEntry(assetPath);
                if (entry == null)
                {
                    notInAddressable.Add(assetPath);
                }
            }

            // 构建统计信息
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== GameRes 统计信息 ===");
            sb.AppendLine();
            sb.AppendLine($"📁 GameRes 文件总数: {allAssetPaths.Length}");
            sb.AppendLine($"📦 已添加到 Addressable: {addressableCount}");
            sb.AppendLine($"❌ 未添加到 Addressable: {notInAddressable.Count}");
            sb.AppendLine();

            if (notInAddressable.Count > 0 && notInAddressable.Count <= 20)
            {
                sb.AppendLine("未添加的文件:");
                foreach (var file in notInAddressable)
                {
                    sb.AppendLine($"  - {file}");
                }
            }
            else if (notInAddressable.Count > 20)
            {
                sb.AppendLine($"未添加的文件 (显示前20个，共{notInAddressable.Count}个):");
                for (int i = 0; i < 20; i++)
                {
                    sb.AppendLine($"  - {notInAddressable[i]}");
                }
                sb.AppendLine("  ...");
            }

            string message = sb.ToString();
            Debug.Log(message);
            EditorUtility.DisplayDialog("GameRes 统计信息", message, "确定");
        }

        [MenuItem("Tools/将 GameRes 所有文件添加到 Addressable")]
        public static void AddAllGameResToAddressable()
        {
            var gameResFolder = new DirectoryInfo(GameResPath);
            if (!gameResFolder.Exists)
            {
                Debug.LogWarning($"GameRes 文件夹不存在: {GameResPath}");
                return;
            }

            // 使用 AssetDatabase 获取所有资产，避免路径问题
            var allAssetPaths = AssetDatabase.FindAssets("", new[] { GameResPath })
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Where(path => !string.IsNullOrEmpty(path) && path.StartsWith(GameResPath) && !path.EndsWith(".meta") && !Directory.Exists(path))
                .ToArray();

            int totalCount = allAssetPaths.Length;
            int processedCount = 0;
            int skippedCount = 0;
            int errorCount = 0;

            // 显示进度条
            EditorUtility.DisplayProgressBar("添加到 Addressable", "正在扫描文件...", 0);

            try
            {
                var settings = AddressableAssetSettingsDefaultObject.Settings;
                if (settings == null)
                {
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("错误", "Addressable Asset Settings 未找到，请先创建 Addressable 配置", "确定");
                    return;
                }

                for (int i = 0; i < totalCount; i++)
                {
                    var assetPath = allAssetPaths[i];

                    // 更新进度条
                    float progress = (float)i / totalCount;
                    EditorUtility.DisplayProgressBar("添加到 Addressable", $"正在处理: {Path.GetFileName(assetPath)} ({i + 1}/{totalCount})", progress);

                    // 检查是否已经存在
                    var entry = settings.FindAssetEntry(assetPath);
                    if (entry != null)
                    {
                        skippedCount++;
                        continue;
                    }

                    // 添加到 Addressable
                    if (AddToAddressable(assetPath))
                    {
                        processedCount++;
                    }
                    else
                    {
                        errorCount++;
                    }
                }

                AssetDatabase.SaveAssets();

                // 显示结果
                string message = $"处理完成！\n总计: {totalCount} 个文件\n";
                message += $"新添加: {processedCount} 个\n";
                message += $"已存在(跳过): {skippedCount} 个\n";
                if (errorCount > 0)
                {
                    message += $"失败: {errorCount} 个";
                }

                Debug.Log(message);
                EditorUtility.DisplayDialog("完成", message, "确定");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private static void GenerateAddressableKeys()
        {
            try
            {
                var gameResFolder = new DirectoryInfo(GameResPath);
                if (!gameResFolder.Exists)
                {
                    Debug.LogWarning($"GameRes 文件夹不存在: {GameResPath}");
                    return;
                }

                EditorUtility.DisplayProgressBar("生成 AddressableKeys", "正在扫描文件结构...", 0.1f);

                var sb = new StringBuilder();
                sb.AppendLine(AutoGenHeader);
                sb.AppendLine("namespace HotUpdate.Utility");
                sb.AppendLine("{");
                sb.AppendLine("public static class AddressableKeys");
                sb.AppendLine("{");

                var allAssetPaths = AssetDatabase.FindAssets("", new[] { GameResPath })
                    .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                    .Where(path => !string.IsNullOrEmpty(path) &&
                                   path.StartsWith(GameResPath) &&
                                   !path.EndsWith(".meta") &&
                                   !Directory.Exists(path))
                    .OrderBy(path => path)
                    .ToArray();

                var directoryStructure = allAssetPaths
                    .GroupBy(path =>
                    {
                        var relativePath = path.Substring(GameResPath.Length + 1);
                        return Path.GetDirectoryName(relativePath)?.Replace("\\", "/") ?? string.Empty;
                    })
                    .ToDictionary(group => group.Key, group => group.OrderBy(path => path).ToArray());

                foreach (var assetPath in allAssetPaths)
                {
                    AddIndent(sb, 1);
                    sb.AppendLine($"public const string {BuildFlatMemberName(assetPath)} = \"{assetPath}\";");
                }

                if (allAssetPaths.Length > 0 && directoryStructure.Count > 0)
                {
                    sb.AppendLine();
                }

                foreach (var entry in directoryStructure.OrderBy(pair => pair.Key))
                {
                    AddIndent(sb, 1);
                    sb.AppendLine($"public static string {BuildGetterName(entry.Key)}(string path) => \"{BuildDirectoryPath(entry.Key)}/\" + path + \"{GetSharedExtension(entry.Value)}\";");
                }

                sb.AppendLine("}");
                sb.AppendLine("}");

                var outputDir = Path.GetDirectoryName(OutputPath);
                if (!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                EditorUtility.DisplayProgressBar("生成 AddressableKeys", "正在写入文件...", 0.9f);
                var outputPath = Path.GetFullPath(OutputPath);
                File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);

                AssetDatabase.Refresh();
                Debug.Log($"AddressableKeys 已生成: {outputPath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"生成 AddressableKeys 时出错: {e.Message}\n{e.StackTrace}");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private static string BuildFlatMemberName(string assetPath)
        {
            var relativePath = assetPath.Substring(GameResPath.Length + 1).Replace("\\", "/");
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(relativePath);
            var extension = NormalizeExtension(Path.GetExtension(relativePath));

            return $"{SanitizePathSegment(fileNameWithoutExtension)}_{extension}";
        }

        private static string BuildGetterName(string directory)
        {
            if (string.IsNullOrEmpty(directory))
            {
                return "GetGameRes";
            }

            var flattenedDirectory = string.Join("_", directory.Split('/').Select(SanitizePathSegment));
            return $"Get{flattenedDirectory}";
        }

        private static string BuildDirectoryPath(string directory)
        {
            return string.IsNullOrEmpty(directory)
                ? GameResPath
                : $"{GameResPath}/{directory}";
        }

        private static string GetSharedExtension(IEnumerable<string> assetPaths)
        {
            var extensions = assetPaths
                .Select(Path.GetExtension)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            return extensions.Length == 1 ? extensions[0] : string.Empty;
        }

        private static string NormalizeExtension(string extension)
        {
            var trimmed = extension.TrimStart('.');
            if (string.IsNullOrEmpty(trimmed))
            {
                return "File";
            }

            var sanitized = SanitizeClassName(trimmed);
            return char.ToUpper(sanitized[0]) + sanitized.Substring(1);
        }

        private static string SanitizePathSegment(string name)
        {
            if (string.IsNullOrEmpty(name))
                return "Empty";

            var result = System.Text.RegularExpressions.Regex.Replace(name, "[^a-zA-Z0-9_]", "_");
            if (string.IsNullOrWhiteSpace(result) || result.All(c => c == '_'))
                return "Empty";

            return result.TrimStart('_');
        }

        private static void AddIndent(StringBuilder sb, int count)
        {
            sb.Append(new string(' ', count * 4));
        }
        private static string SanitizeClassName(string name)
        {
            // 清理类名，移除无效字符
            if (string.IsNullOrEmpty(name))
                return "Empty";

            var result = System.Text.RegularExpressions.Regex.Replace(name, "[^a-zA-Z0-9_]", "_");

            // 如果结果为空或全是下划线，返回默认名称
            if (string.IsNullOrWhiteSpace(result) || result.All(c => c == '_'))
                return "Empty";

            if (char.IsDigit(result[0]))
                result = "_" + result;

            return result;
        }

        private static string SanitizeVariableName(string name)
        {
            // 清理变量名，移除无效字符
            if (string.IsNullOrEmpty(name))
                return "empty";

            var result = System.Text.RegularExpressions.Regex.Replace(name, "[^a-zA-Z0-9_]", "_");

            // 如果结果为空或全是下划线，返回默认名称
            if (string.IsNullOrWhiteSpace(result) || result.All(c => c == '_'))
                return "empty";

            if (char.IsDigit(result[0]))
                result = "_" + result;

            return result;
        }

        /// <summary>
        /// 将资产添加到 Addressable 系统
        /// </summary>
        /// <returns>是否成功添加</returns>
        private static bool AddToAddressable(string assetPath, bool showLog = false)
        {
            try
            {
                var settings = AddressableAssetSettingsDefaultObject.Settings;
                if (settings == null)
                {
                    if (showLog) Debug.LogWarning("Addressable Asset Settings 未找到，请先创建 Addressable 配置");
                    return false;
                }

                // 检查是否已经在 Addressable 中
                var entry = settings.FindAssetEntry(assetPath);
                if (entry != null)
                {
                    return false; // 已经存在，不需要添加
                }

                // 获取或创建默认组
                var group = settings.groups.FirstOrDefault(g => g.Name == DefaultGroupName);
                if (group == null)
                {
                    group = settings.CreateGroup(DefaultGroupName, false, false, false, null);
                }

                // 添加到 Addressable
                var guid = AssetDatabase.AssetPathToGUID(assetPath);
                if (!string.IsNullOrEmpty(guid))
                {
                    entry = settings.CreateOrMoveEntry(guid, group);
                    entry.SetAddress(assetPath);
                    entry.SetLabel("default", true, true);

                    if (showLog) Debug.Log($"已添加到 Addressable: {assetPath}");
                    return true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"添加到 Addressable 失败 ({assetPath}): {e.Message}");
            }
            return false;
        }

        /// <summary>
        /// 从 Addressable 系统中移除资产
        /// </summary>
        private static void RemoveFromAddressable(string assetPath)
        {
            try
            {
                var settings = AddressableAssetSettingsDefaultObject.Settings;
                if (settings == null)
                    return;

                var entry = settings.FindAssetEntry(assetPath);
                if (entry != null)
                {
                    settings.RemoveAssetEntry(entry.guid);
                    Debug.Log($"已从 Addressable 移除: {assetPath}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"从 Addressable 移除失败 ({assetPath}): {e.Message}");
            }
        }
    }
}
