using System;
using System.Collections.Generic;
using HotUpdate.Data.Utility;
using QFramework;
using UnityEngine;

namespace HotUpdate.Dialogue.Model
{
    /// <summary>
    ///     对话数据模型
    ///     负责管理对话进度数据
    /// </summary>
    public class DialogueModel : AbstractModel
    {
        /// <summary>
        ///     存储工具
        /// </summary>
        private IStorage storage;

/// <summary>
        ///     所有对话的进度字典（对话ID -> 进度数据）
        /// </summary>
        public Dictionary<string, DialogueProgress> DialoguesProgress { get; private set; }

/// <summary>
        ///     已完成的对话ID集合
        /// </summary>
        public HashSet<string> CompletedDialogues { get; private set; }

protected override void OnInit()
        {
            // 初始化数据结构
            DialoguesProgress = new Dictionary<string, DialogueProgress>();
            CompletedDialogues = new HashSet<string>();

// 获取存储工具
            storage = this.GetUtility<IStorage>();

// 从存档加载对话进度
            LoadDialogueProgress();

// 监听数据变化并自动保存
            // 注意：这里简化处理，实际应该在每次修改进度时手动调用保存
        }

/// <summary>
        ///     获取对话进度
        /// </summary>
        public DialogueProgress GetDialogueProgress(string dialogueId)
        {
            if (DialoguesProgress.TryGetValue(dialogueId, value: out var progress)) return progress;
            return null;
        }

/// <summary>
        ///     创建或更新对话进度
        /// </summary>
        public void UpdateDialogueProgress(string dialogueId, string currentNodeId, bool isCompleted = false)
        {
            if (!DialoguesProgress.ContainsKey(dialogueId))
            {
                DialoguesProgress[dialogueId] = new DialogueProgress(dialogueId, currentNodeId);
            }
            else
            {
                DialoguesProgress[dialogueId].currentNodeId = currentNodeId;
                DialoguesProgress[dialogueId].lastPlayTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }

if (isCompleted)
            {
                DialoguesProgress[dialogueId].isCompleted = true;
                CompletedDialogues.Add(dialogueId);
            }

// 保存到本地存储
            SaveDialogueProgress();
        }

/// <summary>
        ///     检查对话是否已完成
        /// </summary>
        public bool HasCompletedDialogue(string dialogueId)
        {
            return CompletedDialogues.Contains(dialogueId);
        }

/// <summary>
        ///     检查对话是否有进度记录
        /// </summary>
        public bool HasDialogueProgress(string dialogueId)
        {
            return DialoguesProgress.ContainsKey(dialogueId);
        }

/// <summary>
        ///     删除对话进度（用于测试或重置）
        /// </summary>
        public void RemoveDialogueProgress(string dialogueId)
        {
            if (DialoguesProgress.ContainsKey(dialogueId))
            {
                CompletedDialogues.Remove(dialogueId);
                DialoguesProgress.Remove(dialogueId);
                SaveDialogueProgress();
            }
        }

/// <summary>
        ///     从存档加载对话进度
        /// </summary>
        private void LoadDialogueProgress()
        {
            try
            {
                var loadedData = storage.Load<Dictionary<string, DialogueProgress>>("DialogueProgress");

if (loadedData != null)
                {
                    DialoguesProgress = loadedData;

// 重建已完成对话集合
                    CompletedDialogues.Clear();
                    foreach (var kvp in DialoguesProgress)
                        if (kvp.Value.isCompleted)
                            CompletedDialogues.Add(kvp.Key);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[DialogueModel] 加载对话进度失败: {e.Message}");
            }
        }

/// <summary>
        ///     保存对话进度到本地存储
        /// </summary>
        private void SaveDialogueProgress()
        {
            try
            {
                storage.Save("DialogueProgress", DialoguesProgress);
            }
            catch (Exception e)
            {
                Debug.LogError($"[DialogueModel] 保存对话进度失败: {e.Message}");
            }
        }

/// <summary>
        ///     清空所有对话进度（用于测试）
        /// </summary>
        public void ClearAllProgress()
        {
            DialoguesProgress.Clear();
            CompletedDialogues.Clear();
            SaveDialogueProgress();
        }
    }
}