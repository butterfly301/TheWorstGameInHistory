using System.Collections.Generic;
using HotUpdate.Core.Condition;
using HotUpdate.Dialogue.Data;
using UnityEngine;

namespace HotUpdate.Dialogue.Utility
{
    /// <summary>
    ///     对话条件判断工具类
    ///     现在使用通用的条件系统（ConditionChecker）
    /// </summary>
    public static class DialogueCondition
    {
        /// <summary>
        ///     判断选项条件是否满足
        /// </summary>
        public static bool CheckChoiceCondition(ChoiceCondition condition)
        {
            if (condition == null) return true;

// 将对话系统的选项条件转换为通用条件数据
            var conditionData = new ConditionData
            {
                type = condition.type,
                target = condition.target,
                value = condition.value
            };

return ConditionChecker.CheckCondition(conditionData);
        }

/// <summary>
        ///     判断节点条件列表，返回第一个满足条件的节点ID
        /// </summary>
        public static string CheckNodeConditions(List<NodeCondition> conditions)
        {
            if (conditions == null || conditions.Count == 0) return null;

foreach (var condition in conditions)
                if (CheckNodeCondition(condition))
                    return condition.targetNodeId;

return null;
        }

/// <summary>
        ///     判断单个节点条件
        /// </summary>
        private static bool CheckNodeCondition(NodeCondition condition)
        {
            // 将对话系统的节点条件转换为通用条件数据
            var conditionData = new ConditionData
            {
                type = condition.type,
                target = condition.target,
                value = condition.value
            };

return ConditionChecker.CheckCondition(conditionData);
        }
    }
}
