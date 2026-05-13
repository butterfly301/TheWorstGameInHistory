# 条件判断系统 (Condition System)

## 📋 简介

通用条件判断系统，可用于对话、任务、触发器等各种需要条件判断的场景。

## 🏗️ 架构

### 核心组件

- **ConditionData** - 条件数据类
- **ConditionChecker** - 条件检查器（静态类）
- **ConditionSystem** - 条件系统（QFramework System）
- **ConditionGroup** - 条件组（支持AND/OR逻辑）

## ✨ 内置条件类型

| 条件类型 | 说明 | 参数示例 |
|---------|------|---------|
| `HasItem` | 检查是否拥有物品 | target: 物品ID |
| `GreaterThan` | 检查数值是否大于 | target: 数据键, value: 比较值 |
| `LessThan` | 检查数值是否小于 | target: 数据键, value: 比较值 |
| `Equal` | 检查数值是否等于 | target: 数据键, value: 比较值 |
| `GreaterThanOrEqual` | 检查数值是否大于等于 | target: 数据键, value: 比较值 |
| `LessThanOrEqual` | 检查数值是否小于等于 | target: 数据键, value: 比较值 |
| `HasCompletedDialogue` | 检查是否已完成对话 | target: 对话ID |

## 📖 使用方法

### 1. 基础使用

```csharp
using HotUpdate.Core.Condition;

// 创建条件数据
var condition = new ConditionData
{
    type = "GreaterThan",
    target = "level",
    value = 10
};

// 检查条件
bool result = ConditionChecker.CheckCondition(condition);
```

### 2. 条件组（AND/OR）

```csharp
// 创建条件组（AND关系）
var group = new ConditionGroup
{
    type = ConditionGroupType.All,
    conditions = new List<ConditionData>
    {
        new ConditionData { type = "HasItem", target = "key" },
        new ConditionData { type = "GreaterThan", target = "level", value = 5 }
    }
};

bool allMet = ConditionChecker.CheckConditionGroup(group);

// 创建条件组（OR关系）
var orGroup = new ConditionGroup
{
    type = ConditionGroupType.Any,
    conditions = new List<ConditionData>
    {
        new ConditionData { type = "HasItem", target = "key_a" },
        new ConditionData { type = "HasItem", target = "key_b" }
    }
};

bool anyMet = ConditionChecker.CheckConditionGroup(orGroup);
```

### 3. 扩展方法

```csharp
using HotUpdate.Core.Condition;

var conditions = new List<ConditionData>
{
    new ConditionData { type = "HasItem", target = "key" },
    new ConditionData { type = "GreaterThan", target = "level", value = 5 }
};

// 检查所有条件（AND）
bool all = conditions.CheckAll();

// 检查任意条件（OR）
bool any = conditions.CheckAny();
```

### 4. 通过ConditionSystem使用

```csharp
// 获取ConditionSystem实例
var conditionSystem = this.GetSystem<ConditionSystem>();

// 检查条件
var condition = new ConditionData { type = "HasItem", target = "sword" };
bool result = conditionSystem.Check(condition);
```

### 5. 自定义条件类型

```csharp
// 方式1：直接注册
ConditionChecker.RegisterConditionType("IsDayTime", condition =>
{
    // 实现你的条件逻辑
    return DateTime.Now.Hour >= 6 && DateTime.Now.Hour < 18;
});

// 方式2：通过ConditionSystem注册
var conditionSystem = this.GetSystem<ConditionSystem>();
conditionSystem.RegisterConditionType("IsDayTime", condition =>
{
    return DateTime.Now.Hour >= 6 && DateTime.Now.Hour < 18;
});

// 使用自定义条件
var condition = new ConditionData { type = "IsDayTime" };
bool isDay = ConditionChecker.CheckCondition(condition);
```

## 🔧 待实现功能

以下条件类型的具体逻辑需要根据实际项目需求实现：

1. **HasItem** - 需要集成背包系统
2. **数值比较** - 需要实现 `GetNumericValue()` 方法
3. **HasCompletedDialogue** - 已实现（依赖DialogueModel）

实现位置：`ConditionChecker.cs` 的 `#region 默认条件检查实现` 区域

## 📝 数据格式示例

### JSON格式（可用于配置文件）

```json
{
  "condition": {
    "type": "GreaterThan",
    "target": "level",
    "value": 10
  }
}
```

### 条件组JSON格式

```json
{
  "conditionGroup": {
    "type": "All",
    "conditions": [
      {
        "type": "HasItem",
        "target": "key",
        "value": 0
      },
      {
        "type": "GreaterThan",
        "target": "level",
        "value": 5
      }
    ]
  }
}
```

## 🎯 使用场景

### 对话系统
- 选项显示条件
- 节点跳转条件

### 任务系统
- 任务接取条件
- 任务完成条件

### 触发器系统
- 事件触发条件
- 区域进入条件

### 成就系统
- 成就解锁条件

## ⚠️ 注意事项

1. 条件检查器在首次使用时会自动注册默认条件类型
2. 自定义条件类型建议在游戏初始化时注册
3. 条件数据支持序列化，可直接用于ScriptableObject或JSON配置
