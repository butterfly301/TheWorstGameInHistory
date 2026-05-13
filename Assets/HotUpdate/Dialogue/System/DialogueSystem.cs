using System;
using System.Collections.Generic;
using HotUpdate.Audio.System;
using HotUpdate.Dialogue.Controller;
using HotUpdate.Dialogue.Data;
using HotUpdate.Dialogue.Utility;
using HotUpdate.Manager;
using HotUpdate.Utility;
using Newtonsoft.Json;
using QFramework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace HotUpdate.Dialogue.System
{
    /// <summary>
    ///     对话系统
    ///     负责加载对话数据、获取本地化文本、提供事件系统
    /// </summary>
    public class DialogueSystem : AbstractSystem
    {
        /// <summary>
        ///     对话数据缓存（对话ID -> 对话数据）
        /// </summary>
        private Dictionary<string, DialogueData> dialogueDataCache;

        /// <summary>
        ///     选项被选择事件
        /// </summary>
        public UnityEvent<string, int> OnChoiceSelected = new();

        /// <summary>
        ///     对话结束事件
        /// </summary>
        public UnityEvent<string> OnDialogueEnd = new();

        /// <summary>
        ///     对话开始事件
        /// </summary>
        public UnityEvent<string> OnDialogueStart = new();

        /// <summary>
        ///     节点切换事件
        /// </summary>
        public UnityEvent<string, string> OnNodeChanged = new();

        protected override void OnInit()
        {
            dialogueDataCache = new Dictionary<string, DialogueData>();
        }

        /// <summary>
        ///     异步加载对话数据
        /// </summary>
        public void LoadDialogueDataAsync(string dialogueId, Action<DialogueData> onComplete,
            Action<string> onError = null)
        {
            // 临时：禁用缓存以便调试
            // if (dialogueDataCache.ContainsKey(dialogueId))
            // {
            //     onComplete?.Invoke(dialogueDataCache[dialogueId]);
            //     return;
            // }

            // 构建Addressable地址
            var address = AddressableKeys.Data.Dialogue.GetDialogue(dialogueId);

            // 使用 Addressables 加载 JSON
            AddressablesManager.Instance.LoadAssetAsync<TextAsset>(address,
                handle =>
                {
                    try
                    {
                        if (handle.Status == AsyncOperationStatus.Succeeded)
                        {
                            var json = handle.Result.text;

                            // 使用 Newtonsoft.Json 替代 JsonUtility（支持Dictionary）
                            var dialogueData = JsonConvert.DeserializeObject<DialogueData>(json);

                            if (dialogueData != null && dialogueData.config != null)
                            {
                                // 加入缓存
                                dialogueDataCache[dialogueId] = dialogueData;
                                onComplete?.Invoke(dialogueData);
                            }
                            else
                            {
                                Debug.LogError($"[DialogueSystem] 对话数据解析失败: {dialogueId}");
                                onError?.Invoke($"对话数据解析失败: {dialogueId}");
                            }
                        }
                        else
                        {
                            Debug.LogError($"[DialogueSystem] 对话数据加载失败: {dialogueId}, 状态: {handle.Status}");
                            onError?.Invoke($"对话数据加载失败: {dialogueId}");
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[DialogueSystem] 加载对话数据时发生异常: {dialogueId}, {e.Message}");
                        onError?.Invoke($"加载对话数据时发生异常: {e.Message}");
                    }
                });
        }

        /// <summary>
        ///     获取对话数据（从缓存）
        /// </summary>
        public DialogueData GetDialogueData(string dialogueId)
        {
            if (dialogueDataCache.ContainsKey(dialogueId)) return dialogueDataCache[dialogueId];
            Debug.LogWarning($"[DialogueSystem] 对话数据未在缓存中: {dialogueId}");
            return null;
        }

        /// <summary>
        ///     获取本地化文本
        /// </summary>
        public string GetLocalizedText(string textKey)
        {
            try
            {
                // 获取当前选中的语言
                var selectedLocale = LocalizationSettings.SelectedLocale;

                if (selectedLocale == null)
                {
                    Debug.LogWarning("[DialogueSystem] 未选择语言，使用默认文本");
                    return textKey;
                }

                // 从 String Table 获取本地化文本
                var stringTable = LocalizationSettings.StringDatabase.GetTable("String Table", selectedLocale);

                if (stringTable == null)
                {
                    Debug.LogWarning("[DialogueSystem] 未找到对话文本表: Dialogue");
                    return textKey;
                }

                var entry = stringTable.GetEntry(textKey);

                if (entry == null)
                {
                    Debug.LogWarning($"[DialogueSystem] 未找到对话文本Key: {textKey}");
                    return textKey;
                }

                return entry.GetLocalizedString();
            }
            catch (Exception e)
            {
                Debug.LogError($"[DialogueSystem] 获取本地化文本失败: {textKey}, {e.Message}");
                return textKey;
            }
        }

        /// <summary>
        ///     触发对话开始事件
        /// </summary>
        public void TriggerDialogueStart(string dialogueId)
        {
            OnDialogueStart?.Invoke(dialogueId);
        }

        /// <summary>
        ///     触发对话结束事件
        /// </summary>
        public void TriggerDialogueEnd(string dialogueId)
        {
            OnDialogueEnd?.Invoke(dialogueId);
        }

        /// <summary>
        ///     触发节点切换事件
        /// </summary>
        public void TriggerNodeChanged(string dialogueId, string nodeId)
        {
            OnNodeChanged?.Invoke(dialogueId, nodeId);
        }

        /// <summary>
        ///     触发选项选择事件
        /// </summary>
        public void TriggerChoiceSelected(string dialogueId, int choiceIndex)
        {
            OnChoiceSelected?.Invoke(dialogueId, choiceIndex);
        }

        /// <summary>
        ///     执行对话事件（播放语音、自定义事件等）
        /// </summary>
        public void ExecuteDialogueEvents(DialogueEventData eventData, string dialogueId)
        {
            if (eventData == null) return;

            try
            {
                switch (eventData.type)
                {
                    case "PlayVoice":
                        // 播放语音
                        PlayVoice(eventData.value);
                        break;

                    case "CustomEvent":
                        // 自定义事件
                        TriggerCustomEvent(eventData.value, dialogueId);
                        break;

                    default:
                        Debug.LogWarning($"[DialogueSystem] 未知的事件类型: {eventData.type}");
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[DialogueSystem] 执行对话事件失败: {eventData.type}, {e.Message}");
            }
        }

        /// <summary>
        ///     播放语音
        /// </summary>
        private void PlayVoice(string voiceClipAddress)
        {
            // 使用 AudioSystem 播放语音
            var audioSystem = this.GetSystem<AudioSystem>();
            audioSystem.PlayVoice(voiceClipAddress);
        }

        /// <summary>
        ///     触发自定义事件
        /// </summary>
        private void TriggerCustomEvent(string eventName, string dialogueId)
        {
            // 这里可以通过事件总线或其他方式通知其他系统
            // TODO: 可以集成事件总线系统
        }

        /// <summary>
        ///     清除对话数据缓存
        /// </summary>
        public void ClearCache()
        {
            dialogueDataCache.Clear();
        }

        /// <summary>
        ///     预加载对话数据
        /// </summary>
        public void PreloadDialogueData(string dialogueId)
        {
            LoadDialogueDataAsync(dialogueId,
                data => { },
                error => { Debug.LogWarning($"[DialogueSystem] 预加载对话数据失败: {dialogueId}, {error}"); });
        }
    }
}