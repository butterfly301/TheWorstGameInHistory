using System.Collections;
using System.Collections.Generic;
using System.IO;
using HotUpdate.SceneLoad.Commands;
using HotUpdate.UI;
using HotUpdate.Utility;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.Video;

public class IceBreakerPreWindow : WindowBase
{
    [Header("数据")] private readonly List<string> targetNames = new();

    [SerializeField]private Button change;
    [SerializeField]private Button hack;

    [SerializeField]private Transform main;

    [SerializeField]private RawImage screen;
    [SerializeField]private LocalizeStringEvent targetName;
    [SerializeField]private TextMeshProUGUI time;

    //跟踪时间更新协程
    private Coroutine timeCoroutine;
    private VideoPlayer videoPlayer;

    private void OnDestroy()
    {
        if (videoPlayer != null) videoPlayer.prepareCompleted -= OnVideoPrepared;
        if (timeCoroutine != null)
        {
            StopCoroutine(timeCoroutine);
            timeCoroutine = null;
        }
    }

    public override void Init(MainMenu mainMenuVar)
    {
        base.Init(mainMenuVar);
        targetNames.AddRange(mainMenuVar.MainMenuData.targetNames);
        // 初始化组件引用
        videoPlayer = screen.GetComponent<VideoPlayer>();
        hack.onClick.AddListener(() => { this.SendCommand(new LoadSceneCommand(AddressableKeys.Scenes.IceBreaker_Unity, false)); });
        change.onClick.AddListener(() => { StartCoroutine(SwitchTarget()); });
        videoPlayer.prepareCompleted += OnVideoPrepared;
        StartCoroutine(SwitchTarget());
    }

    private IEnumerator SwitchTarget(float delay = 1f)
    {
        // 切换目标时停止已有的时间更新协程并清空时间显示
        if (timeCoroutine != null)
        {
            StopCoroutine(timeCoroutine);
            timeCoroutine = null;
        }

        screen.color = new Color(1f, 1f, 1f, 0f);
        time.SetText("");
        yield return new WaitForSeconds(delay);
        var target = targetNames[Random.Range(0, targetNames.Count)];
        targetName.SetEntry(target);
        var fullFileName = target + ".mp4";
        var videoPath = Path.Combine(Application.streamingAssetsPath, "Videos", "HackPreview", fullFileName);
        videoPlayer.url = videoPath;
        videoPlayer.Prepare();
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        screen.color = new Color(1f, 1f, 1f, 1f);
        time.SetText(CurrentTimeUtility.GetCurrentTimeString());
        videoPlayer.Play();

        // 启动或重启每秒更新时间的协程
        if (timeCoroutine != null)
        {
            StopCoroutine(timeCoroutine);
            timeCoroutine = null;
        }

        timeCoroutine = StartCoroutine(UpdateTimeWhilePlaying());
    }

    // 每秒更新一次时间，直到视频停止播放
    private IEnumerator UpdateTimeWhilePlaying()
    {
        while (videoPlayer != null && videoPlayer.isPlaying)
        {
            if (time != null) time.SetText(CurrentTimeUtility.GetCurrentTimeString());
            yield return new WaitForSeconds(1f);
        }

        // 视频停止后做一次最终更新（可选）
        if (time != null) time.SetText(CurrentTimeUtility.GetCurrentTimeString());

        timeCoroutine = null;
    }
}