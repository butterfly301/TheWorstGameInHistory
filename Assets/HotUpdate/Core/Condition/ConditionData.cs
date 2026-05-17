using System;
using System.Collections.Generic;
using UnityEngine;

namespace HotUpdate.Core.Condition
{
    /// <summary>
    /// 通用条件数据
    /// 可用于各种系统（对话、任务、触发器等）的条件判断
    /// </summary>
    [Serializable]
    public class ConditionData
    {
        /// <summary>
        /// 条件类型：HasItem（拥有物品）、GreaterThan（大于）、LessThan（小于）、Equal（等于）等
        /// 可通过 RegisterConditionType 动态扩展
        /// </summary>
        public string type;

/// <summary>
        /// 条件参数（根据类型不同，参数含义不同）
        /// 例如：
        /// - HasItem类型时，为物品ID
        /// - 数值比较类型时，为数据键值（如 "playthrough"、"level" 等）
        /// </summary>
        public string target;

/// <summary>
        /// 比较值（用于数值比较）
        /// </summary>
        public int value;

/// <summary>
        /// 额外的字符串参数（可选，用于某些特殊条件类型）
        /// </summary>
        public string extraParam;
    }

/// <summary>
    /// 条件组（多个条件的AND/OR关系）
    /// </summary>
    [Serializable]
    public class ConditionGroup
    {
        /// <summary>
        /// 条件组类型：All（全部满足）或 Any（满足任意一个）
        /// </summary>
        public ConditionGroupType type;

/// <summary>
        /// 条件列表
        /// </summary>
        public List<ConditionData> conditions;
    }

/// <summary>
    /// 条件组类型
    /// </summary>
    public enum ConditionGroupType
    {
        All, // 全部条件都要满足（AND）
        Any  // 满足任意一个条件（OR）
    }

/// <summary>
    /// 条件判断结果
    /// </summary>
    public class ConditionResult
    {
        /// <summary>
        /// 是否满足条件
        /// </summary>
        public bool isMet;

/// <summary>
        /// 不满足的原因（可选，用于调试）
        /// </summary>
        public string reason;

public ConditionResult(bool isMet, string reason = null)
        {
            this.isMet = isMet;
            this.reason = reason;
        }
    }
}
