using System.Collections.Generic;
using UnityEngine;

public class SpriteOrderToZPosition : MonoBehaviour
{
    [ContextMenu("根据OrderInLayer调整Z轴")]
    public void AdjustZBySpriteOrder()
    {
        // 获取所有子物体中的SpriteRenderer（包括无限层级）
        var spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        // 用于记录已经处理过的物体，避免重复处理
        var processedObjects = new HashSet<GameObject>();

        var count = 0;

        foreach (var renderer in spriteRenderers)
        {
            var targetObject = renderer.gameObject;

            // 如果这个物体已经处理过，跳过
            if (processedObjects.Contains(targetObject))
                continue;

            // 获取OrderInLayer值
            var orderInLayer = renderer.sortingOrder;

            // 计算Z轴坐标：orderInLayer * -0.001
            var newZ = orderInLayer * -0.001f;

            // 更新物体的位置
            var currentPosition = targetObject.transform.position;
            targetObject.transform.position = new Vector3(currentPosition.x, currentPosition.y, newZ);

            // 标记为已处理
            processedObjects.Add(targetObject); // 这里修正为 targetObject
            count++;

            Debug.Log($"已调整物体 {targetObject.name} 的Z轴位置：{currentPosition.z} -> {newZ} (OrderInLayer: {orderInLayer})",
                targetObject);
        }

        Debug.Log($"操作完成！共调整了 {count} 个物体的Z轴位置。");
    }

    // 在Inspector中显示的按钮
    [ContextMenu("执行调整操作")]
    private void ExecuteAdjustment()
    {
        AdjustZBySpriteOrder();
    }
}