using System;
using System.IO;
using HotUpdate.Core;
using HotUpdate.Interface;
using HotUpdate.Manager;
using HotUpdate.Utility;
using QFramework;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using UnityEngine.Video;

namespace HotUpdate.Video
{
    public class VideoManager : MonoSingleton<VideoManager>, IController,IAutoBind
    {
        private GameName gameName;
        private bool hasSkipButton;

        private bool isPlaying;
        public Action OnVideoEndEvent;
        [SerializeField]private GameObject pauseIcon;
        [SerializeField]private RawImage rawImage;
        private GameObject skipButton;
        private VideoPlayer videoPlayer;
        public bool hasTriggeredSkipButton = false; // New field to track if TriggerSkipButton has been called

        private void Update()
        {
            if (Input.anyKeyDown)
                if (isPlaying)
                    if (!hasSkipButton)
                        TriggerSkipButton();
        }

        protected override void OnDestroy()
        {
            OnVideoEndEvent = null;
            if (videoPlayer != null)
            {
                videoPlayer.prepareCompleted -= OnVideoPrepared;
                videoPlayer.errorReceived -= OnVideoError;
                videoPlayer.loopPointReached -= OnVideoEnd;
            }

            base.OnDestroy();
        }

        public IArchitecture GetArchitecture()
        {
            return TheWorstGameInHistory.Interface;
        }

        public void Init(GameName gameNameVar = GameName.None)
        {
            gameName = gameNameVar;

            AddressablesManager.Instance.LoadAssetAsync<RenderTexture>(
                AddressableKeys.Art.RenderTexture.Video_Render_Texture_RenderTexture,
                handle =>
                {
                    rawImage.texture = handle.Result;
                });
            pauseIcon.SetActive(false);

            LoadSkipButtonByGameName();

            videoPlayer = GetComponent<VideoPlayer>();
            videoPlayer.prepareCompleted += OnVideoPrepared;
            videoPlayer.errorReceived += OnVideoError;
            videoPlayer.loopPointReached += OnVideoEnd;
        }

        private void LoadSkipButtonByGameName()
        {
            switch (gameName)
            {
                case GameName.None:
                    skipButton = null;
                    break;
                case GameName.TLH1:
                    AddressablesManager.Instance.LoadAssetAsync<GameObject>(
                        AddressableKeys.Prefabs.UI.Playthrough1.SkipButton_Prefab,
                        handle => { skipButton = handle.Result; });
                    break;
                case GameName.TLH2:
                    AddressablesManager.Instance.LoadAssetAsync<GameObject>(
                        AddressableKeys.Prefabs.UI.Playthrough2.SkipButton_Prefab,
                        handle => { skipButton = handle.Result; });
                    break;
            }
        }

        private void TriggerSkipButton()
        {
            if (hasTriggeredSkipButton) return;

            if (skipButton != null)
            {
                var skipButtonObj = Instantiate(skipButton, transform);
                skipButtonObj.GetComponent<SkipButton>()?.Init();
            }

            hasTriggeredSkipButton = true; 
        }

        /// <summary>
        ///     播放视频，现在会根据当前语言动态选择文件
        /// </summary>
        /// <param name="baseVideoName">视频的基础名称，不带语言后缀和扩展名 (例如 "OpenVideo0")</param>
        public void PlayVideo(string baseVideoName)
        {
            // 获取当前语言代码
            var langCode = LocalizationSettings.SelectedLocale.Identifier.Code;

            //构建完整文件名 (例如 "OpenVideo0_zh-CN.mp4")
            var fullFileName = $"{baseVideoName}_{langCode}.mp4";

            // 组合StreamingAssets路径
            var videoPath = Path.Combine(Application.streamingAssetsPath, "Videos", fullFileName);

            // 检查文件是否存在，并提供回退机制
            if (!File.Exists(videoPath))
            {
                Debug.LogWarning($"未找到当前语言 '{langCode}' 的视频文件，尝试使用默认英文版。");
                fullFileName = $"{baseVideoName}_en-US.mp4"; // 默认回退到英文版
                videoPath = Path.Combine(Application.streamingAssetsPath, "Videos", fullFileName);

                if (!File.Exists(videoPath))
                {
                    Debug.LogError($"连默认的英文版视频文件都找不到: {videoPath}");
                    return;
                }
            }

            // 设置URL并准备播放
            videoPlayer.url = videoPath;
            videoPlayer.Prepare();
        }

        private void OnVideoPrepared(VideoPlayer source)
        {
            rawImage.gameObject.SetActive(true);
            isPlaying = true;
            videoPlayer.Play();
        }

        private void OnVideoError(VideoPlayer source, string message)
        {
            Debug.LogError($"视频播放错误: {message}");
            Debug.LogError($"URL: {source.url}");
        }

        private void OnVideoEnd(VideoPlayer source)
        {
            rawImage.gameObject.SetActive(false);
            isPlaying = false;
            OnVideoEndEvent?.Invoke();
        }

        public void Play()
        {
            isPlaying = true;
            pauseIcon.SetActive(false);
            videoPlayer.Play();
        }

        public void PauseVideo()
        {
            isPlaying = false;
            pauseIcon.SetActive(true);
            videoPlayer.Pause();
        }

        public void StopVideo()
        {
            isPlaying = false;
            pauseIcon.SetActive(true);
            videoPlayer.Stop();
        }

        public void SkipVideo()
        {
            if (videoPlayer != null && videoPlayer.isPlaying)
            {
                videoPlayer.Stop();
                isPlaying = false;
                pauseIcon.SetActive(false);
                OnVideoEndEvent?.Invoke();
            }
        }

        public void SetHasSkipButton(bool hasSkipButtonBoolValue)
        {
            hasSkipButton = hasSkipButtonBoolValue;
        }
    }

    public enum GameName
    {
        None,
        TLH1,
        TLH2
    }
}