using System;
using System.Collections.Generic;
using HotUpdate.Utility;

namespace HotUpdate.Dialogue.Data
{
    /// <summary>
    ///     对话配置元数据
    /// </summary>
    [Serializable]
    public class DialogueConfig
    {
        /// <summary>
        ///     对话ID
        /// </summary>
        public string dialogueId;

        /// <summary>
        ///     起始节点ID
        /// </summary>
        public string startNodeId;

        /// <summary>
        ///     视图类型：Traditional（传统对话框）或 Bubble（泡泡对话框）
        /// </summary>
        public DialogueViewType viewType;

        /// <summary>
        ///     是否保存进度
        /// </summary>
        public bool saveProgress;

        /// <summary>
        ///     打字速度（秒），可选，不填使用默认值
        /// </summary>
        public float typingSpeed = 0.05f;

        /// <summary>
        ///     是否可以跳过打字（点击立即显示全部）
        /// </summary>
        public bool canSkip = true;

        /// <summary>
        ///     对话事件配置
        /// </summary>
        public DialogueEventsConfig events;

        /// <summary>
        ///     所有对话节点
        /// </summary>
        public Dictionary<string, DialogueNode> nodes;
    }

    /// <summary>
    ///     对话事件配置
    /// </summary>
    [Serializable]
    public class DialogueEventsConfig
    {
        /// <summary>
        ///     对话开始时的事件列表
        /// </summary>
        public List<DialogueEventData> onStart;

        /// <summary>
        ///     对话结束时的事件列表
        /// </summary>
        public List<DialogueEventData> onEnd;
    }

    /// <summary>
    ///     对话事件数据
    /// </summary>
    [Serializable]
    public class DialogueEventData
    {
        /// <summary>
        ///     事件类型：PlayVoice（播放语音）、CustomEvent（自定义事件）等
        /// </summary>
        public string type;

        /// <summary>
        ///     事件参数（根据类型不同，参数含义不同）
        ///     例如：PlayVoice类型时，为语音地址
        ///     CustomEvent类型时，为事件名称
        /// </summary>
        public string value;
    }
}