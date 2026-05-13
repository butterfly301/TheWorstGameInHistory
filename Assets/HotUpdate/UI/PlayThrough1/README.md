# UIManager1 架构说明

## 📐 架构设计

使用**组合模式**将UIManager1拆分为多个独立的子管理器，每个子系统有专属管理器，避免多人协作时的代码冲突。

## 📁 子管理器列表

| 子管理器 | 文件 | 职责 | 负责人 |
|---------|------|------|--------|
| **DialogueUIManager** | `DialogueUIManager.cs` | 对话系统UI（对话框、泡泡对话） | 你 |
| **SkillTreeUIManager** | `SkillTreeUIManager.cs` | 技能树UI | 同事 |
| **PauseUIManager** | `PauseUIManager.cs` | 暂停面板 | - |
| **MapPanelManager** | `MapPanelManager.cs` | 地图面板 | - |
| **GlitchEffectManager** | `GlitchEffectManager.cs` | 故障特效 | - |
| **GlitchWindowManager** | `GlitchWindowManager.cs` | 故障窗口 | - |
| **TouchControlManager** | `TouchControlManager.cs` | 触屏控制 | - |

## 🎯 使用示例

```csharp
// 对话系统
UIManager1.Instance.Dialogue.ShowDialogueView(DialogueViewType.DialogueViewTypeName.NarratorView);
UIManager1.Instance.Dialogue.RegisterSpeaker(CharacterName.Player, playerBubble);

// 技能树系统（同事使用）
UIManager1.Instance.SkillTree.OpenSkillTreePanel();
UIManager1.Instance.SkillTree.CloseSkillTreePanel();

// 暂停面板
UIManager1.Instance.PauseUI.OpenPausePanel();
UIManager1.Instance.PauseUI.ClosePausePanel();

// 地图面板
UIManager1.Instance.MapPanel.LoadMapPanelPrefab("0");
UIManager1.Instance.MapPanel.OpenMapPanel();

// 故障特效
UIManager1.Instance.GlitchEffect.AdjustGlitchEffect(0.5f);

// 故障窗口
UIManager1.Instance.GlitchWindow.OpenGlitchWindow();

// 触屏控制
UIManager1.Instance.TouchControl.ShowTouchControls();
UIManager1.Instance.TouchControl.HideTouchControls();
```

## ✅ 优势

1. **完全解耦** - 各自修改自己的管理器文件，零冲突
2. **职责清晰** - 每个管理器只负责一个功能模块
3. **易于维护** - 代码组织清晰，易于查找和修改
4. **便于扩展** - 添加新系统只需创建新的子管理器

## 🔄 如何添加新的子管理器

1. 创建新的管理器类（例如 `InventoryUIManager.cs`）
2. 在 `UIManager1` 中添加属性和初始化代码
3. 在委托区域添加 override 方法

### 示例：

```csharp
// 1. 创建 InventoryUIManager.cs
public class InventoryUIManager
{
    // 实现库存UI逻辑
}

// 2. 在 UIManager1 中添加
public InventoryUIManager Inventory { get; private set; }

private void Init()
{
    Inventory = new InventoryUIManager(parentTransform);
    Inventory.Init();
}

// 3. 添加委托方法
public override void OpenInventory()
{
    Inventory.OpenInventory();
}
```

## 📝 注意事项

- 每个子管理器都是独立的，不要在子管理器之间直接调用
- 所有UI面板的加载和显示逻辑都在对应的子管理器中
- UIManager1只负责初始化和委托，不包含具体UI逻辑
- 需要使用协程时，可以使用 `GlitchEffectManager` 中的 `CoroutineRunner`
