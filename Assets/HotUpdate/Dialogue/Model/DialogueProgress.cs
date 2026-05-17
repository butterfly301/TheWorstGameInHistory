using System;

namespace HotUpdate.Dialogue.Model
{
    /// <summary>
    ///     对话进度数据
    /// </summary>
    [Serializable]
    public class DialogueProgress
    {
        /// <summary>
        ///     对话ID
        /// </summary>
        public string dialogueId;

/// <summary>
        ///     当前节点ID
        /// </summary>
        public string currentNodeId;

/// <summary>
        ///     对话是否已完成
        /// </summary>
        public bool isCompleted;

/// <summary>
        ///     上次播放时间
        /// </summary>
        public string lastPlayTime;

public DialogueProgress()
        {
        }

public DialogueProgress(string dialogueId, string currentNodeId)
        {
            this.dialogueId = dialogueId;
            this.currentNodeId = currentNodeId;
            isCompleted = false;
            lastPlayTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}