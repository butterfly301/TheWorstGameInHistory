using System;

namespace HotUpdate.Dialogue.Data
{
    /// <summary>
    ///     对话数据根类（对应JSON文件）
    /// </summary>
    [Serializable]
    public class DialogueData
    {
        /// <summary>
        ///     对话配置元数据
        /// </summary>
        public DialogueConfig config;

        /// <summary>
        ///     获取对话ID
        /// </summary>
        public string DialogueId => config.dialogueId;
    }
}