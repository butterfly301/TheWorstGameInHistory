#if UNITY_EDITOR

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

public static class HybridCLRBuildTool
{
    // --------- 你提供的路径（已硬编码） -------------
    private const string ProjectRoot = @"E:\1FairyOnTheTree\The-worst-game-in-history";
    private const string HotUpdateDllSourceDir = ProjectRoot + @"\HybridCLRData\HotUpdateDlls\StandaloneWindows64";

    private const string AotDllSourceDir =
        ProjectRoot + @"\HybridCLRData\AssembliesPostIl2CppStrip\StandaloneWindows64";

    private const string HotUpdateBytesTargetDir = "Assets/HybridCLRGroup/HotUpdateDll";
    private const string AotBytesTargetDir = "Assets/HybridCLRGroup/AOTDll";

    private const string HotUpdateGroupName = "HotUpdateGroup";
    private const string AotGroupName = "AOTMetadataGroup";

    private const string HotUpdateLabel = "HotUpdateDll";
    private const string AotLabel = "AotDll";

    // -------------------------------------------------

    [MenuItem("HybridCLR/Generate DLL.bytes & Mark Addressable")]
    public static void GenerateDllBytesAndMarkAddressable()
    {
        try
        {
            EnsureTargetDirectories();

            // 1. Copy HotUpdate.dll -> HotUpdate.dll.bytes
            CopyHotUpdateDll();

            // 2. Copy all AOT dlls -> *.dll.bytes
            CopyAllAotDlls();

            // 3. Import assets
            AssetDatabase.Refresh();

            // 4. Mark as Addressables
            MarkAddressables();

            Debug.Log("[HybridCLRBuildUtility] 完成！现在可以执行菜单：HybridCLR/Build Addressables。");
        }
        catch (Exception ex)
        {
            Debug.LogError("[HybridCLRBuildUtility] Exception: " + ex);
        }
    }

    [MenuItem("HybridCLR/Build Addressables")]
    public static void BuildAddressablesContent()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError(
                "[HybridCLRBuildUtility] AddressableAssetSettings not found! 请先通过 Window -> Asset Management -> Addressables 创建。");
            return;
        }

        Debug.Log("[HybridCLRBuildUtility] 开始构建 Addressables...");
        try
        {
            AddressableAssetSettings.BuildPlayerContent();
            Debug.Log("[HybridCLRBuildUtility] Addressables 构建完成！");
        }
        catch (Exception ex)
        {
            Debug.LogError("[HybridCLRBuildUtility] Addressables 构建错误：" + ex);
        }
    }

    private static void EnsureTargetDirectories()
    {
        if (!Directory.Exists(HotUpdateBytesTargetDir))
        {
            Directory.CreateDirectory(HotUpdateBytesTargetDir);
            Debug.Log("[HybridCLRBuildUtility] 创建目录: " + HotUpdateBytesTargetDir);
        }

        if (!Directory.Exists(AotBytesTargetDir))
        {
            Directory.CreateDirectory(AotBytesTargetDir);
            Debug.Log("[HybridCLRBuildUtility] 创建目录: " + AotBytesTargetDir);
        }
    }

    private static void CopyHotUpdateDll()
    {
        var dllPath = Directory
            .EnumerateFiles(HotUpdateDllSourceDir, "HotUpdate.dll", SearchOption.TopDirectoryOnly)
            .FirstOrDefault();

        if (dllPath == null)
        {
            Debug.LogError($"[HybridCLRBuildUtility] HotUpdate.dll 不存在: {HotUpdateDllSourceDir}");
            return;
        }

        var destBytesPath = Path.Combine(HotUpdateBytesTargetDir, "HotUpdate.dll.bytes");
        File.Copy(dllPath, destBytesPath, true);

        Debug.Log($"[HybridCLRBuildUtility] HotUpdate.dll 已复制到 {destBytesPath}");
        AssetDatabase.ImportAsset(destBytesPath, ImportAssetOptions.ForceUpdate);
    }

    private static void CopyAllAotDlls()
    {
        if (!Directory.Exists(AotDllSourceDir))
        {
            Debug.LogError($"[HybridCLRBuildUtility] AOT dll 源路径不存在: {AotDllSourceDir}");
            return;
        }

        var dllFiles = Directory.EnumerateFiles(AotDllSourceDir, "*.dll", SearchOption.TopDirectoryOnly);
        var count = 0;

        foreach (var dll in dllFiles)
        {
            var fileName = Path.GetFileName(dll);
            var destBytesPath = Path.Combine(AotBytesTargetDir, fileName + ".bytes");

            File.Copy(dll, destBytesPath, true);
            AssetDatabase.ImportAsset(destBytesPath, ImportAssetOptions.ForceUpdate);

            count++;
        }

        Debug.Log($"[HybridCLRBuildUtility] {count} 个 AOT DLL 已复制至 {AotBytesTargetDir}");
    }


    //---------------------------------------------
    // Addressables 标记流程（无条件依赖）
    //---------------------------------------------
    private static void MarkAddressables()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;

        if (settings == null)
        {
            Debug.LogError("[HybridCLRBuildUtility] 无法找到 Addressables 设置，请先创建 Addressables。");
            return;
        }

        var hotGroup = settings.FindGroup(HotUpdateGroupName)
                       ?? settings.CreateGroup(HotUpdateGroupName, false, false, false, null);

        var aotGroup = settings.FindGroup(AotGroupName)
                       ?? settings.CreateGroup(AotGroupName, false, false, false, null);

        EnsureGroupHasBundledSchema(hotGroup);
        EnsureGroupHasBundledSchema(aotGroup);

        // HotUpdate bytes
        foreach (var file in Directory.EnumerateFiles(HotUpdateBytesTargetDir, "*.bytes"))
            AddOrMoveEntry(settings, file, hotGroup, HotUpdateLabel);

        // AOT bytes
        foreach (var file in Directory.EnumerateFiles(AotBytesTargetDir, "*.bytes"))
            AddOrMoveEntry(settings, file, aotGroup, AotLabel);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[HybridCLRBuildUtility] Addressables 标记完成！");
    }

    private static void EnsureGroupHasBundledSchema(AddressableAssetGroup group)
    {
        if (group.GetSchema<BundledAssetGroupSchema>() == null)
        {
            group.AddSchema(typeof(BundledAssetGroupSchema));
            Debug.Log($"[HybridCLRBuildUtility] Group {group.Name} 已添加 BundledAssetGroupSchema");
        }
    }

    private static void AddOrMoveEntry(AddressableAssetSettings settings, string filePath, AddressableAssetGroup group,
        string label)
    {
        var path = NormalizePath(filePath);
        var guid = AssetDatabase.AssetPathToGUID(path);

        if (string.IsNullOrEmpty(guid))
        {
            Debug.LogError($"[HybridCLRBuildUtility] 无法获取 GUID: {path}");
            return;
        }

        var entry = settings.FindAssetEntry(guid);
        if (entry != null)
        {
            if (entry.parentGroup != group)
            {
                settings.MoveEntry(entry, group);
                Debug.Log($"[HybridCLRBuildUtility] 移动至 Group {group.Name}: {path}");
            }

            entry.SetLabel(label, true);
            return;
        }

        // 新建
        entry = settings.CreateOrMoveEntry(guid, group);
        entry.SetLabel(label, true);

        Debug.Log($"[HybridCLRBuildUtility] 添加到 Group {group.Name} Label {label}: {path}");
    }

    private static string NormalizePath(string filePath)
    {
        var path = filePath.Replace("\\", "/");
        if (path.StartsWith(Application.dataPath)) path = "Assets" + path.Substring(Application.dataPath.Length);
        return path;
    }
}
#endif