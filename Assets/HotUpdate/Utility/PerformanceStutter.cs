using System.Collections;
using UnityEngine;

namespace HotUpdate.Utility
{
    public class PerformanceStutter : MonoBehaviour
    {
        public int stutterIntensity = 10;
        public float stutterDuration = 0.5f;
        private int originalTargetFrameRate; // 用于存储原来的目标帧率
        private int originalVSyncCount; // 用于存储原来的垂直同步设置

public void TriggerStutterEffect()
        {
            StartCoroutine(StutterCoroutine(stutterIntensity, stutterDuration));
        }

private IEnumerator StutterCoroutine(int stutterValue, float stutterDurationValue)
        {
            // 1. 保存原来的设置
            originalTargetFrameRate = Application.targetFrameRate;
            originalVSyncCount = QualitySettings.vSyncCount;

// 2. 应用卡顿设置：大幅降低帧率
            Application.targetFrameRate = stutterValue; // 将帧率限制到很低的值[6,7](@ref)
            QualitySettings.vSyncCount = 0; // 关闭垂直同步，避免其干扰帧率限制[8](@ref)

// 3. 等待指定的卡顿持续时间
            yield return new WaitForSecondsRealtime(stutterDurationValue); // 使用不受时间缩放影响的时间

// 4. 恢复原来的设置
            Application.targetFrameRate = originalTargetFrameRate;
            QualitySettings.vSyncCount = originalVSyncCount;
        }
    }
}