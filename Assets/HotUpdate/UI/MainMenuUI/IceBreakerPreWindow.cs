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
    [Header("鏁版嵁")] private readonly List<string> targetNames = new();

    [SerializeField]private Button change;
    [SerializeField]private Button hack;

    [SerializeField]private Transform main;

    [SerializeField]private RawImage screen;
    [SerializeField]private LocalizeStringEvent targetName;
    [SerializeField]private TextMeshProUGUI time;

    // Keep a reference to the time update coroutine.
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
        // Cache component references.
        videoPlayer = screen.GetComponent<VideoPlayer>();
        hack.onClick.AddListener(() => { this.SendCommand(new LoadSceneCommand(AddressableKeys.IceBreaker_Unity, false)); });
        change.onClick.AddListener(() => { StartCoroutine(SwitchTarget()); });
        videoPlayer.prepareCompleted += OnVideoPrepared;
        StartCoroutine(SwitchTarget());
    }

    private IEnumerator SwitchTarget(float delay = 1f)
    {
        // Stop the existing time update coroutine before switching target.
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

        // Start or restart the coroutine that refreshes the time text.
        if (timeCoroutine != null)
        {
            StopCoroutine(timeCoroutine);
            timeCoroutine = null;
        }

        timeCoroutine = StartCoroutine(UpdateTimeWhilePlaying());
    }

    // Refresh the clock once per second while the preview is playing.
    private IEnumerator UpdateTimeWhilePlaying()
    {
        while (videoPlayer != null && videoPlayer.isPlaying)
        {
            if (time != null) time.SetText(CurrentTimeUtility.GetCurrentTimeString());
            yield return new WaitForSeconds(1f);
        }

        // Do one final refresh after playback stops.
        if (time != null) time.SetText(CurrentTimeUtility.GetCurrentTimeString());

        timeCoroutine = null;
    }
}
