using UnityEngine;
using UnityEngine.EventSystems;

public class UIDrag : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private Vector3 offset; // 关键的偏移量：鼠标点击位置与UI元素中心点的距离
    private RectTransform targetRectTransform; // 要移动的UI元素自身的RectTransform

private void Start()
    {
        // 获取UI元素自身的RectTransform
        targetRectTransform = GetComponent<RectTransform>();
    }

public void OnDrag(PointerEventData eventData)
    {
        if (targetRectTransform == null) return;

Vector3 worldPoint;
        // 将屏幕坐标转换为世界坐标
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                targetRectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out worldPoint))
            // 应用偏移量，确保UI元素不会瞬移到鼠标位置
            targetRectTransform.position = worldPoint + offset;
    }

public void OnPointerDown(PointerEventData eventData)
    {
        // 将屏幕坐标转换为世界坐标，计算精准的偏移量
        Vector3 worldPoint;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                targetRectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out worldPoint))
            // 计算偏移量：UI元素当前位置与鼠标点击位置的世界坐标差值
            offset = targetRectTransform.position - worldPoint;

// 将当前拖拽的UI元素置于渲染层级最前
        transform.SetAsLastSibling();
    }
}