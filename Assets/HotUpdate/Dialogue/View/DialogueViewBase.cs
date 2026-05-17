using System;
using System.Collections.Generic;
using HotUpdate.Dialogue.Data;
using HotUpdate.Effect;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using HotUpdate.Interface;
using HotUpdate.Manager;
using HotUpdate.Utility;

namespace HotUpdate.Dialogue.View
{
    /// <summary>
    ///     对话视图基类
    /// </summary>
    public class DialogueViewBase : MonoBehaviour, IDialogueView, IAutoBind
    {
        [Header("对话组件")] [SerializeField] protected TextMeshProUGUI speakerNameText;

[SerializeField] protected TextMeshProUGUI dialogueText;
        [SerializeField] protected Transform choicesContainer;

[Header("打字机配置")] [SerializeField] protected float defaultTypingSpeed = 0.05f;

[SerializeField] protected bool canSkip = true;
        protected Dictionary<GameObject, int> choiceButtonIndexMap = new();
        protected List<GameObject> currentChoiceButtons = new();
        protected float currentTypingSpeed;
        protected Action<int> onChoiceSelectedCallback;

protected TypeWriterEffect typeWriterEffect;
        protected GameObject choiceButtonPrefab;

protected virtual void OnDestroy()
        {
            ClearChoices();
        }

public virtual void ShowDialogue(string speaker, string text)
        {
            SetSpeakerName(speaker);
            StartTyping(text);
        }

public virtual void ShowChoices(ChoiceData[] choices, Action<int> onChoiceSelected)
        {
            ClearChoices();
            onChoiceSelectedCallback = onChoiceSelected;

if (choices == null || choices.Length == 0)
            {
                Debug.LogWarning("[DialogueViewBase] 选项列表为空");
                return;
            }

// 生成选项按钮
            for (var i = 0; i < choices.Length; i++) CreateChoiceButton(choices[i], i);
        }

public virtual void Hide()
        {
            gameObject.SetActive(false);
            ClearChoices();
        }

public virtual void SetTypingSpeed(float speed)
        {
            currentTypingSpeed = speed;
            if (typeWriterEffect != null)
            {
                // TypeWriterEffect 的 typingSpeed 字段是私有的，无法直接设置
                // 这里可以通过反射或者修改 TypeWriterEffect 来支持动态设置速度
                // 暂时先记录速度，在 StartTyping 时使用
            }
        }

public virtual void StartTyping(string text)
        {
            if (dialogueText == null || typeWriterEffect == null) return;

dialogueText.text = text;
            typeWriterEffect.SetTypingSpeed(currentTypingSpeed);
            typeWriterEffect.StartTyping();
        }

public virtual void CompleteTyping()
        {
            if (typeWriterEffect != null && canSkip)
            {
                typeWriterEffect.StopAllCoroutines();
                // 直接显示完整文本
                if (dialogueText != null) dialogueText.text = dialogueText.text;
            }
        }

public virtual bool IsTyping()
        {
            return typeWriterEffect != null && typeWriterEffect.GetIsTyping();
        }

public virtual void ClearChoices()
        {
            foreach (var button in currentChoiceButtons)
                if (button != null)
                    Destroy(button);

currentChoiceButtons.Clear();
            choiceButtonIndexMap.Clear();
        }

public virtual void SetSpeakerName(string speakerName)
        {
            if (speakerNameText != null) speakerNameText.text = speakerName;
        }

public void Init()
        {
            typeWriterEffect = dialogueText?.GetComponent<TypeWriterEffect>();
            currentTypingSpeed = defaultTypingSpeed;
        }

/// <summary>
        /// 设置选择按钮预制体
        /// </summary>
        public void SetChoiceButtonPrefab(GameObject prefab)
        {
            choiceButtonPrefab = prefab;
        }

protected virtual void CreateChoiceButton(ChoiceData choice, int index)
        {
            if (choiceButtonPrefab == null || choicesContainer == null)
            {
                Debug.LogError("[DialogueViewBase] choiceButtonPrefab 或 choicesContainer 未设置");
                return;
            }

var buttonObj = Instantiate(choiceButtonPrefab, choicesContainer);
            currentChoiceButtons.Add(buttonObj);
            choiceButtonIndexMap[buttonObj] = index;

var button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                var capturedIndex = index; // 捕获索引
                button.onClick.AddListener(() => OnChoiceClicked(capturedIndex));
            }

// 设置按钮文本
            var buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
                // TODO: 使用本地化系统获取文本
                buttonText.text = choice.textKey;
        }

protected virtual void OnChoiceClicked(int choiceIndex)
        {
            onChoiceSelectedCallback?.Invoke(choiceIndex);
        }
    }
}