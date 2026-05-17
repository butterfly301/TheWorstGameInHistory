using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace HotUpdate.Effect
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TypeWriterEffect : MonoBehaviour
    {
        [SerializeField] private float typingSpeed = 0.05f; // 每个字符的显示间隔
        public bool typingOnEnable;
        public UnityEvent onTypeComplete; // 打字完成时的事件
        private bool isTyping;

private TextMeshProUGUI textComponent;
        private string originalText { get; set; }

private void Awake()
        {
            textComponent = GetComponent<TextMeshProUGUI>();
        }

private void OnEnable()
        {
            if (typingOnEnable)
                StartTyping();
        }

private void OnDisable()
        {
            StopAllCoroutines();
            isTyping = false;

//禁用时重置文本到原始状态
            if (textComponent != null && !string.IsNullOrEmpty(originalText)) textComponent.text = originalText;
        }

public void StartTyping()
        {
            // 停止任何正在进行的打字协程，以确保我们可以用新文本重新开始
            StopAllCoroutines();

originalText = textComponent.text;
            textComponent.text = "";

// 启动新的打字协程
            StartCoroutine(TypeText());
        }

private IEnumerator TypeText()
        {
            isTyping = true;

foreach (var letter in originalText)
            {
                textComponent.text += letter;
                // 使用 WaitForSecondsRealtime 来避免受 Time.timeScale 影响
                yield return new WaitForSecondsRealtime(typingSpeed);
            }

isTyping = false;
            onTypeComplete?.Invoke(); // 打字完成后调用事件
        }

public void SetTypingSpeed(float typingSpeedVar)
        {
            typingSpeed = typingSpeedVar;
        }

public bool GetIsTyping()
        {
            return isTyping;
        }
    }
}