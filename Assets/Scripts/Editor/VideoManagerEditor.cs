#if UNITY_EDITOR
using HotUpdate.Video;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VideoManager))]
public class VideoManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 绘制默认的Inspector内容
        DrawDefaultInspector();

        // 添加一个跳过视频按钮
        if (GUILayout.Button("跳过剧情"))
        {
            var videoManager = (VideoManager)target;
            videoManager.SkipVideo();
        }
    }
}
#endif