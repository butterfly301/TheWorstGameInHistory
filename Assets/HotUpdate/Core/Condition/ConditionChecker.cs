using System;
using System.Collections.Generic;
using HotUpdate.Data.Model;
using QFramework;
using UnityEngine;

namespace HotUpdate.Core.Condition
{
    /// <summary>
    /// 通用条件检查器
    /// 负责执行各种条件类型的判断逻辑
    /// </summary>
    public static class ConditionChecker
    {
        /// <summary>
        /// 注册的条件类型检查器
        /// 条件类型字符串 -> 检查函数
        /// </summary>
        private static readonly Dictionary<string, Func<ConditionData, bool>> RegisteredCheckers = new();

/// <summary>
        /// 静态构造函数，注册默认的条件类型
        /// </summary>
        static ConditionChecker()
        {
            RegisterDefaultConditionTypes();
        }

/// <summary>
        /// 检查单个条件是否满足
        /// </summary>
        public static bool CheckCondition(ConditionData condition)
        {
            if (condition == null)
            {
                Debug.LogWarning("[ConditionChecker] 条件数据为null，默认返回true");
                return true;
            }

// 查找注册的检查器
            if (RegisteredCheckers.TryGetValue(condition.type, out var checker))
            {
                try
                {
                    return checker(condition);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[ConditionChecker] 条件检查异常: {condition.type}, {e.Message}");
                    return false;
                }
            }

Debug.LogWarning($"[ConditionChecker] 未知的条件类型: {condition.type}");
            return false;
        }

/// <summary>
        /// 获取已注册的条件类型数量
        /// </summary>
        public static int GetRegisteredConditionTypesCount()
        {
            return RegisteredCheckers.Count;
        }

/// <summary>
        /// 检查条件组是否满足
        /// </summary>
        public static bool CheckConditionGroup(ConditionGroup group)
        {
            if (group == null || group.conditions == null || group.conditions.Count == 0)
            {
                return true;
            }

// 根据条件组类型决定检查方式
            foreach (var condition in group.conditions)
            {
                var result = CheckCondition(condition);

if (group.type == ConditionGroupType.Any && result)
                {
                    // Any模式：只要有一个满足就返回true
                    return true;
                }

if (group.type == ConditionGroupType.All && !result)
                {
                    // All模式：只要有一个不满足就返回false
                    return false;
                }
            }

// All模式全部满足，或Any模式全部不满足
            return group.type == ConditionGroupType.All;
        }

/// <summary>
        /// 检查条件列表（OR关系，返回第一个满足条件的索引）
        /// 返回值：满足条件的索引，-1表示都不满足
        /// </summary>
        public static int CheckConditions(List<ConditionData> conditions)
        {
            if (conditions == null || conditions.Count == 0)
            {
                return -1;
            }

for (var i = 0; i < conditions.Count; i++)
            {
                if (CheckCondition(conditions[i]))
                {
                    return i;
                }
            }

return -1;
        }

/// <summary>
        /// 注册自定义条件类型
        /// </summary>
        public static void RegisterConditionType(string typeName, Func<ConditionData, bool> checker)
        {
            if (string.IsNullOrEmpty(typeName) || checker == null)
            {
                Debug.LogError("[ConditionChecker] 条件类型名称或检查函数为空");
                return;
            }

if (RegisteredCheckers.ContainsKey(typeName))
            {
                Debug.LogWarning($"[ConditionChecker] 条件类型已存在，将被覆盖: {typeName}");
            }

RegisteredCheckers[typeName] = checker;
            Debug.Log($"[ConditionChecker] 已注册条件类型: {typeName}");
        }

/// <summary>
        /// 取消注册条件类型
        /// </summary>
        public static void UnregisterConditionType(string typeName)
        {
            if (RegisteredCheckers.Remove(typeName))
            {
                Debug.Log($"[ConditionChecker] 已取消注册条件类型: {typeName}");
            }
        }

/// <summary>
        /// 注册默认的条件类型
        /// </summary>
        private static void RegisterDefaultConditionTypes()
        {
            // HasItem：检查是否拥有物品
            RegisterConditionType("HasItem", condition =>
            {
                return CheckHasItem(condition.target);
            });

// GreaterThan：检查数值是否大于指定值
            RegisterConditionType("GreaterThan", condition =>
            {
                var currentValue = GetNumericValue(condition.target);
                return currentValue > condition.value;
            });

// LessThan：检查数值是否小于指定值
            RegisterConditionType("LessThan", condition =>
            {
                var currentValue = GetNumericValue(condition.target);
                return currentValue < condition.value;
            });

// Equal：检查数值是否等于指定值
            RegisterConditionType("Equal", condition =>
            {
                var currentValue = GetNumericValue(condition.target);
                return currentValue == condition.value;
            });

// GreaterThanOrEqual：检查数值是否大于等于指定值
            RegisterConditionType("GreaterThanOrEqual", condition =>
            {
                var currentValue = GetNumericValue(condition.target);
                return currentValue >= condition.value;
            });

// LessThanOrEqual：检查数值是否小于等于指定值
            RegisterConditionType("LessThanOrEqual", condition =>
            {
                var currentValue = GetNumericValue(condition.target);
                return currentValue <= condition.value;
            });

// HasCompletedDialogue：检查是否已完成指定对话
            RegisterConditionType("HasCompletedDialogue", condition =>
            {
                return CheckHasCompletedDialogue(condition.target);
            });

Debug.Log("[ConditionChecker] 默认条件类型注册完成");
        }

#region 默认条件检查实现

/// <summary>
        /// 检查是否拥有物品
        /// TODO: 需要集成背包系统后实现
        /// </summary>
        private static bool CheckHasItem(string itemId)
        {
            // TODO: 从背包系统获取物品信息
            // var inventoryModel = TheWorstGameInHistory.Interface.GetModel<InventoryModel>();
            // return inventoryModel.HasItem(itemId);

Debug.LogWarning($"[ConditionChecker] CheckHasItem 未实现，物品ID: {itemId}");
            return false;
        }

/// <summary>
        /// 获取数值类型的游戏数据
        /// target 格式：类型:键值，例如 "playthrough"、"level"、"item:wood" 等
        /// TODO: 需要根据实际游戏数据结构实现
        /// </summary>
        private static int GetNumericValue(string target)
        {
            if (string.IsNullOrEmpty(target))
            {
                return 0;
            }

// 尝试从 GameDataModel 获取数据
            try
            {
                var gameDataModel = TheWorstGameInHistory.Interface.GetModel<GameDataModel>();
                var gameData = gameDataModel.CurrentGameData.Value;

if (gameData == null)
                {
                    return 0;
                }

// TODO: 根据target的格式解析并返回对应的数值
                // 例如：
                // if (target == "playthrough") return gameData.playthrough;
                // if (target.StartsWith("item:")) return GetItemCount(target.Substring(5), gameData.inventory);

Debug.LogWarning($"[ConditionChecker] GetNumericValue 未完全实现，target: {target}");
                return 0;
            }
            catch (Exception e)
            {
                Debug.LogError($"[ConditionChecker] GetNumericValue 异常: {e.Message}");
                return 0;
            }
        }

/// <summary>
        /// 检查是否已完成指定对话
        /// </summary>
        private static bool CheckHasCompletedDialogue(string dialogueId)
        {
            try
            {
                var dialogueModel = TheWorstGameInHistory.Interface.GetModel<HotUpdate.Dialogue.Model.DialogueModel>();
                return dialogueModel.HasCompletedDialogue(dialogueId);
            }
            catch (Exception e)
            {
                Debug.LogError($"[ConditionChecker] CheckHasCompletedDialogue 异常: {e.Message}");
                return false;
            }
        }

#endregion
    }
}
