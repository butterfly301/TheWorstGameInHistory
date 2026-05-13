using System.Collections.Generic;
using HotUpdate.Interface;
using UnityEngine;
using UnityEngine.UI;

public class GraphicPanel : MonoBehaviour, OptionPanelChildren,IAutoBind
{
    // 分辨率预设数组，与Toggle顺序对应
    private readonly Vector2Int[] resolutions =
    {
        new(1920, 1080), // 索引0 -> 第一个Toggle
        new(1600, 900), // 索引1 -> 第二个Toggle  
        new(1280, 720) // 索引2 -> 第三个Toggle
    };

    [Header("全屏设置")] [SerializeField]private Toggle fullscreenToggle; // 全屏Toggle独立声明
    [SerializeField] private Transform group;

    private ToggleGroup resolutionToggleGroup;

    [Header("分辨率设置 (按顺序拖入Toggle)")] private Toggle[] resolutionToggles; // 使用数组存储分辨率Toggle

    public void Init()
    {
        resolutionToggles = FindResolutionToggles();
        // 安全检查：确保数组元素数量匹配
        if (resolutionToggles.Length != resolutions.Length)
        {
            Debug.LogError("分辨率Toggle数量与预设分辨率数量不匹配！");
            return;
        }

        InitializeToggleGroup();
        LoadCurrentSettings();
        SetupEventListeners();
    }

    public Toggle[] FindResolutionToggles()
    {
        var result = new List<Toggle>();

        // 获取当前物体及其所有子物体的Transform组件
        var allTransforms = group.GetComponentsInChildren<Transform>(true);

        foreach (var t in allTransforms)
            // 检查名字是否包含"ResolutionToggle"
            if (t.name.Contains("ResolutionToggle"))
                result.Add(t.gameObject.GetComponent<Toggle>());

        return result.ToArray();
    }

    private void InitializeToggleGroup()
    {
        // 创建或获取ToggleGroup组件
        resolutionToggleGroup = GetComponent<ToggleGroup>();
        resolutionToggleGroup.allowSwitchOff = false;

        // 将所有分辨率Toggle添加到同一组
        foreach (var toggle in resolutionToggles) toggle.group = resolutionToggleGroup;
    }

    private void LoadCurrentSettings()
    {
        // 加载当前全屏设置[4](@ref)
        fullscreenToggle.isOn = Screen.fullScreen;

        // 加载当前分辨率并选择对应的Toggle[1](@ref)
        var currentResolution = new Vector2Int(Screen.width, Screen.height);
        var resolutionFound = false;

        for (var i = 0; i < resolutions.Length; i++)
            if (currentResolution == resolutions[i])
            {
                resolutionToggles[i].isOn = true;
                resolutionFound = true;
                break;
            }

        // 如果当前分辨率不在预设中，选择第一个(1080p)
        if (!resolutionFound)
        {
            resolutionToggles[0].isOn = true;
            SetResolution(resolutions[0].x, resolutions[0].y);
        }
    }

    private void SetupEventListeners()
    {
        // 全屏Toggle事件
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggleChanged);

        // 循环设置所有分辨率Toggle的事件监听[6,8](@ref)
        for (var i = 0; i < resolutionToggles.Length; i++)
        {
            var index = i; // 重要：创建局部变量避免闭包问题
            resolutionToggles[i].onValueChanged.AddListener(isOn =>
            {
                if (isOn) OnResolutionToggleChanged(index);
            });
        }
    }

    private void OnFullscreenToggleChanged(bool isFullscreen)
    {
        var selectedIndex = GetSelectedResolutionIndex();
        SetResolution(resolutions[selectedIndex].x, resolutions[selectedIndex].y, isFullscreen);
    }

    private void OnResolutionToggleChanged(int resolutionIndex)
    {
        SetResolution(resolutions[resolutionIndex].x, resolutions[resolutionIndex].y, fullscreenToggle.isOn);
    }

    private int GetSelectedResolutionIndex()
    {
        // 查找当前选中的Toggle索引[6](@ref)
        for (var i = 0; i < resolutionToggles.Length; i++)
            if (resolutionToggles[i].isOn)
                return i;

        // 默认返回第一个
        return 0;
    }

    private void SetResolution(int width, int height, bool? fullscreen = null)
    {
        var fullscreenMode = fullscreen ?? Screen.fullScreen;

        // 设置分辨率[1,4](@ref)
        Screen.SetResolution(width, height, fullscreenMode);

        // 更新摄像机宽高比以适应新分辨率
        if (Camera.main != null) Camera.main.aspect = (float)width / height;

        // 强制刷新UI布局
        Canvas.ForceUpdateCanvases();

        Debug.Log($"分辨率已设置为: {width}x{height}, 全屏: {fullscreenMode}");
    }

    // 提供外部调用的便捷方法
    public void SetResolutionByIndex(int index)
    {
        if (index >= 0 && index < resolutions.Length) OnResolutionToggleChanged(index);
    }

    public Vector2Int GetCurrentResolution()
    {
        var index = GetSelectedResolutionIndex();
        return resolutions[index];
    }
}