# 对话面板多视图配置指南

## 概述

对话系统支持 **3种面板类型**，通过在 JSON 配置中指定 `viewType` 来选择使用哪个面板。

**集成方式：** 使用项目现有的 **UIManager** 来管理对话面板。

## 面板类型

| viewType | 说明 | 逻辑类 | 预制体 |
|----------|------|--------|--------|
| `Traditional` | 传统模式 | `TraditionalDialogueView` | 传统对话预制体 |
| `Narrator` | 旁白模式 | `TraditionalDialogueView` | 旁白对话预制体 |
| `Bubble` | 气泡模式 | `BubbleDialogueView` (你自己实现) | 气泡对话预制体 |

**重点：**
- **传统模式** 和 **旁白模式** 使用**同一个逻辑类** (`TraditionalDialogueView`)
- 它们的**预制体不同**（视觉样式不同），通过 UIManager 的路径映射来区分
- **气泡模式** 逻辑不同，你自己实现 `BubbleDialogueView`

## UIManager 实现示例

```csharp
public class YourUIManager : UIManager
{
    // Prefab 路径映射
    private Dictionary<string, string> dialogueViewPaths = new Dictionary<string, string>
    {
        { "Traditional", "UI/Dialogues/TraditionalDialogueView" },
        { "Narrator", "UI/Dialogues/NarratorDialogueView" },  // 不同预制体！
        { "Bubble", "UI/Dialogues/BubbleDialogueView" }
    };

    // 缓存已加载的视图实例
    private Dictionary<string, IDialogueView> dialogueViewCache = new Dictionary<string, IDialogueView>();

    public override IDialogueView GetDialogueView(string viewType)
    {
        if (string.IsNullOrEmpty(viewType))
            viewType = "Traditional"; // 默认视图

        // 从缓存获取
        if (dialogueViewCache.ContainsKey(viewType))
            return dialogueViewCache[viewType];

        // 加载预制体
        if (!dialogueViewPaths.ContainsKey(viewType))
        {
            Debug.LogWarning($"[UIManager] 未找到 viewType '{viewType}' 的路径配置");
            return null;
        }

        string path = dialogueViewPaths[viewType];
        var prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError($"[UIManager] 无法加载预制体: {path}");
            return null;
        }

        // 实例化并缓存
        var viewObj = Instantiate(prefab);
        var view = viewObj.GetComponent<IDialogueView>();

        if (view == null)
        {
            Debug.LogError($"[UIManager] 预制体 {path} 缺少 IDialogueView 组件");
            Destroy(viewObj);
            return null;
        }

        dialogueViewCache[viewType] = view;
        DontDestroyOnLoad(viewObj); // 可选

        return view;
    }

    public override IDialogueView ShowDialogueView(string viewType)
    {
        var view = GetDialogueView(viewType);
        if (view != null)
        {
            var viewObj = (view as Component)?.gameObject;
            if (viewObj != null)
                viewObj.SetActive(true);
        }
        return view;
    }

    public override void HideDialogueView(string viewType)
    {
        if (dialogueViewCache.ContainsKey(viewType))
        {
            var viewObj = (dialogueViewCache[viewType] as Component)?.gameObject;
            if (viewObj != null)
                viewObj.SetActive(false);
        }
    }

    public override void HideAllDialogueViews()
    {
        foreach (var kvp in dialogueViewCache)
        {
            var viewObj = (kvp.Value as Component)?.gameObject;
            if (viewObj != null)
                viewObj.SetActive(false);
        }
    }
}
```

## JSON 配置示例

```json
{
  "config": {
    "dialogueId": "story_001",
    "startNodeId": "node1",
    "viewType": "Narrator",    ← 使用旁白模式
    "saveProgress": true,
    "canSkip": true
  },
  "nodes": { ... }
}
```

```json
{
  "config": {
    "dialogueId": "npc_talk_001",
    "startNodeId": "node1",
    "viewType": "Traditional", ← 使用传统模式
    "saveProgress": false
  },
  "nodes": { ... }
}
```

```json
{
  "config": {
    "dialogueId": "bubble_talk_001",
    "startNodeId": "node1",
    "viewType": "Bubble",     ← 使用气泡模式
    "saveProgress": false
  },
  "nodes": { ... }
}
```

## 工作流程

```
JSON: viewType = "Narrator"
    ↓
UIManager.ShowDialogueView("Narrator")
    ↓
查找路径: dialogueViewPaths["Narrator"]
    ↓
加载预制体: "UI/Dialogues/NarratorDialogueView"
    ↓
获取组件: IDialogueView (实际是 TraditionalDialogueView)
    ↓
返回给 DialogueController 使用
```

## 关键点

1. **旁白和传统共用逻辑类** - `TraditionalDialogueView`
2. **预制体路径不同** - 通过 UIManager 的字典映射区分
3. **气泡模式你自己实现** - `BubbleDialogueView` 逻辑不同

## 文件结构

```
Dialogue/
├── View/
│   ├── TraditionalDialogueView.cs   ← 旁白+传统共用
│   ├── BubbleDialogueView.cs        ← 你自己实现
│   ├── DialogueViewBase.cs          ← 基类
│   └── IDialogueView.cs             ← 接口
├── Controller/
│   └── DialogueController.cs        ← 使用 UIManager
└── README_视图配置.md                ← 本文档
```
