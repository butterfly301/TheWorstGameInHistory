using System;
using HotUpdate.Dialogue.Data;

namespace HotUpdate.Dialogue.View
{
    /// <summary>
    ///     对话视图接口
    /// </summary>
    public interface IDialogueView
    {
        /// <summary>
        ///     显示对话
        /// </summary>
        void ShowDialogue(string speaker, string text);

/// <summary>
        ///     显示选项
        /// </summary>
        void ShowChoices(ChoiceData[] choices, Action<int> onChoiceSelected);

/// <summary>
        ///     隐藏对话UI
        /// </summary>
        void Hide();

/// <summary>
        ///     设置打字机速度
        /// </summary>
        void SetTypingSpeed(float speed);

/// <summary>
        ///     开始打字效果
        /// </summary>
        void StartTyping(string text);

/// <summary>
        ///     立即完成打字
        /// </summary>
        void CompleteTyping();

/// <summary>
        ///     是否正在打字
        /// </summary>
        bool IsTyping();

/// <summary>
        ///     清空选项
        /// </summary>
        void ClearChoices();

/// <summary>
        ///     设置说话者名称
        /// </summary>
        void SetSpeakerName(string speakerName);
    }
}