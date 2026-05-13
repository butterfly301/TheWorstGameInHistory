using System;
using System.Collections.Generic;

namespace HotUpdate.Dialogue.Data
{
    /// <summary>
    ///     对话节点
    /// </summary>
    [Serializable]
    public class DialogueNode
    {
        /// <summary>
        ///     节点ID
        /// </summary>
        public string nodeId;

        /// <summary>
        ///     说话者角色名
        /// </summary>
        public string speaker;

        /// <summary>
        ///     对话文本的本地化Key
        /// </summary>
        public string textKey;

        /// <summary>
        ///     选项列表（如果有选项，则玩家需要选择；如果没有，则直接按继续键进入下一个节点）
        /// </summary>
        public List<ChoiceData> choices;

        /// <summary>
        ///     无选项时的下一个节点ID（可选）
        /// </summary>
        public string nextNodeId;

        /// <summary>
        ///     节点条件（可选，用于条件分支）
        /// </summary>
        public List<NodeCondition> conditions;

        /// <summary>
        ///     节点级事件（可选，节点开始时触发）
        ///     例如：播放语音、自定义事件等
        /// </summary>
        public List<DialogueEventData> events;
    }

    /// <summary>
    ///     节点条件（用于条件分支）
    /// </summary>
    [Serializable]
    public class NodeCondition
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

        /// <summary>
        ///     满足条件时跳转的节点ID
        /// </summary>
        public string targetNodeId;
    }
}