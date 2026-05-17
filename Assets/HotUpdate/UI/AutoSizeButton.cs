using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.UI
{
    public class AutoSizeButton : MonoBehaviour
    {
        public Vector2 Padding = new(40, 20); // 内边距
        public float minWidth = 80f;
        public float minHeight = 40f;
        private Button button;
        private TextMeshProUGUI buttonText;

private void Start()
        {
            if (button == null) button = GetComponent<Button>();
            if (buttonText == null) buttonText = GetComponentInChildren<TextMeshProUGUI>();

// 注册文本变化事件监听[6](@ref)
            if (buttonText != null)
                // 监听TMP文本变化事件
                buttonText.RegisterDirtyVerticesCallback(OnTextChanged);

AdjustButtonSize();
        }

// 清理事件监听
        private void OnDestroy()
        {
            if (buttonText != null) buttonText.UnregisterDirtyVerticesCallback(OnTextChanged);
        }

public void AdjustButtonSize()
        {
            if (buttonText == null) return;

// 强制立即更新网格以获取准确的尺寸信息[2](@ref)
            buttonText.ForceMeshUpdate(true, true);

// 获取文本的 preferred 宽度和高度[1](@ref)
            var preferredWidth = buttonText.preferredWidth + Padding.x;
            var preferredHeight = buttonText.preferredHeight + Padding.y;

// 确保不小于最小尺寸
            var width = Mathf.Max(preferredWidth, minWidth);
            var height = Mathf.Max(preferredHeight, minHeight);

// 调整按钮尺寸
            var rectTransform = button.GetComponent<RectTransform>();
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

// 如果需要刷新布局（在复杂布局中可能需要）[1](@ref)
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

// 当文本改变时自动调用此方法
        public void OnTextChanged()
        {
            AdjustButtonSize();
        }
    }
}