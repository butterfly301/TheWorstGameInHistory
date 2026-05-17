using HotUpdate.Data.Model;
using HotUpdate.UI;
using HotUpdate.UI.MainMenuUI;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class DownloadWindow : ConfirmWindow
{
    [Header("下载所需时间(秒)")] [SerializeField] private float downloadDuration = 60f;

[Header("下载的是什么软件")] public SoftwareName softwareName;

private float currentDownloadTime;
    [SerializeField]private Transform downloadingUI;
    [SerializeField]private LocalizeStringEvent downloadResultText;
    private bool isDownloading;
    [SerializeField]private Slider progressSlider;
    [SerializeField]private TextMeshProUGUI progressText;

protected virtual void Update()
    {
        if (isDownloading)
        {
            currentDownloadTime += Time.deltaTime;
            var progress = CalculateDeceptiveProgress(currentDownloadTime, downloadDuration);
            UpdateProgressUI(progress);

if (currentDownloadTime >= downloadDuration)
            {
                isDownloading = false;
                UpdateProgressUI(1f); // 确保最后显示100%
                OnDownloadComplete();
            }
        }
    }

protected override void OnEnable()
    {
        base.OnEnable();
        StartDownload();
    }

public override void Init(MainMenu mainMenuVar)
    {
        base.Init(mainMenuVar);
        confirm.onClick.AddListener(() => { gameObject.SetActive(false); });
        confirm.gameObject.SetActive(false);
        quit.gameObject.SetActive(false);
        downloadResultText.gameObject.SetActive(false);
    }

private void StartDownload()
    {
        currentDownloadTime = 0f;
        isDownloading = true;
        UpdateProgressUI(0f);
    }

private float CalculateDeceptiveProgress(float currentTime, float totalDuration)
    {
        float progress;

// 定义时间点和进度点
        var phase1Time = 1.0f; // 第一阶段持续1秒
        var phase2ProgressTarget = 0.95f; // 第二阶段目标进度95%

// 调整第二阶段的结束时间点，使其更快。让快速增长阶段在开始后5秒就完成。
        var phase2EndTime = 5.0f;
        // 确保总时长大于第二阶段的结束时间
        var phase3StartTime = Mathf.Max(phase2EndTime, totalDuration - totalDuration / 2f);

if (currentTime <= phase1Time)
        {
            // 阶段1: 开始的1秒，进度非常慢
            // 线性增长到1%，这样看起来没有卡住
            progress = Mathf.Lerp(0, 0.01f, currentTime / phase1Time);
        }
        else if (currentTime <= phase3StartTime)
        {
            // 阶段2: 快速增长阶段，从1秒后到最后阶段开始前
            // 进度从1%快速增长到95%
            var phase2Duration = phase3StartTime - phase1Time;
            var phase2Time = currentTime - phase1Time;
            progress = Mathf.Lerp(0.01f, phase2ProgressTarget, phase2Time / phase2Duration);
        }
        else
        {
            // 阶段3: 最后的慢速阶段
            // 进度从95%缓慢增长到100%
            var phase3Duration = totalDuration - phase3StartTime;
            var phase3Time = currentTime - phase3StartTime;
            // 避免除以零
            if (phase3Duration > 0)
                progress = Mathf.Lerp(phase2ProgressTarget, 1f, phase3Time / phase3Duration);
            else
                progress = 1f;
        }

return Mathf.Clamp01(progress);
    }

private void UpdateProgressUI(float progress)
    {
        if (progressSlider != null) progressSlider.value = progress;

if (progressText != null) progressText.text = $"{(int)(progress * 100)}%";
    }

protected virtual void OnDownloadComplete()
    {
        // 下载完成后的逻辑，例如显示确认按钮或关闭窗口
        // 这里可以根据需求添加逻辑，比如显示确认按钮
        downloadingUI.gameObject.SetActive(false);
        downloadResultText.gameObject.SetActive(true);
        confirm.gameObject.SetActive(true);
    }

protected void DownloadSuccess()
    {
        downloadResultText.SetEntry("Download Complete");
        mainMenu.GetAppGroup().EnableSoftware(softwareName);
    }

protected void DownloadFailure()
    {
        downloadResultText.SetEntry("Download failed");
    }
}