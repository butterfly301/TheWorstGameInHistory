using System;

namespace HotUpdate.Dialogue.Data
{
    /// <summary>
    ///     选项数据
    /// </summary>
    [Serializable]
    public class ChoiceData
    {
        /// <summary>
        ///     选项文本的本地化Key
        /// </summary>
        public string textKey;

/// <summary>
        ///     选择此选项后跳转的节点ID
        /// </summary>
        public string nextNodeId;

/// <summary>
        ///     选项显示条件（可选，不填则总是显示）
        /// </summary>
        public ChoiceCondition condition;
    }

/// <summary>
    ///     选项显示条件
    /// </summary>
    [Serializable]
    public class ChoiceCondition
    {
        /// <summary>
        ///     条件类型：HasItem（拥有物品）、GreaterThan（大于）、LessThan（小于）、Equal（等于）等
        /// </summary>
        public string type;

/// <summary>
        ///     条件参数（根据类型不同，参数含义不同）
        /// </summary>
        public string target;

/// <summary>
        ///     比较值（用于数值比较）
        /// </summary>
        public int value;
    }
}