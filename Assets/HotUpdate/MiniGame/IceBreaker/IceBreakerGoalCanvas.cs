using System;
using System.Collections;
using System.Collections.Generic;
using HotUpdate.Core;
using HotUpdate.Data.Model;
using HotUpdate.Download.System;
using HotUpdate.Effect;
using HotUpdate.SceneLoad.Commands;
using HotUpdate.Utility;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.MiniGame.IceBreaker
{
    public class IceBreakerGoalCanvas : MonoBehaviour, IController
    {
        private readonly Vector3 offset = new(0f, -2f, -5f);
        private Canvas canvas;
        private Button confirm;
        private TypeWriterEffect congratulations;
        private List<string> downloadUrls = new();
        private IceBreakerGoalController goalController;
        private TypeWriterEffect itemName;
        private Button result;
        private string targetSubFolder;
        private Button url;

        public IArchitecture GetArchitecture()
        {
            return TheWorstGameInHistory.Interface;
        }

        public void Init(IceBreakerGoalController goalControllerVar)
        {
            goalController = goalControllerVar;
            canvas = GetComponent<Canvas>();

            congratulations = transform.Find("Congratulations").GetComponent<TypeWriterEffect>();
            congratulations.gameObject.SetActive(false);
            congratulations.onTypeComplete.AddListener(OnCongratulationsButtonClicked);

            result = transform.Find("Result").GetComponent<Button>();
            result.onClick.AddListener(() => { goalController.StartRearranging(); });
            result.interactable = false;

            itemName = transform.Find("ItemName").GetComponent<TypeWriterEffect>();
            itemName.gameObject.SetActive(false);
            itemName.onTypeComplete.AddListener(() => { confirm.gameObject.SetActive(true); });

            confirm = transform.Find("Confirm").GetComponent<Button>();
            confirm.onClick.AddListener(RearrangePlayerShards);
            confirm.gameObject.SetActive(false);
            confirm.interactable = false; // 等待下载完成后再允许点击

            url = transform.Find("URL").GetComponent<Button>();
            url.onClick.AddListener(OnURLButtonClicked);
            url.gameObject.SetActive(false);

            // 使用下载系统在后台下载并保存任意类型文件
            StartDownloadAndSaveAsync();
        }

        // 开始下载与保存，下载成功后允许确认
        private async void StartDownloadAndSaveAsync()
        {
            try
            {
                var downloadData = IceBreakerManager.Instance.GetIceBreakerData().downloadData;
                targetSubFolder = downloadData.targetSubFolder;
                var currentData = this.GetModel<GameDataModel>().CurrentGameData.Value;
                downloadUrls = !currentData.software.Contains(SoftwareName.TLH_1793)
                    ? downloadData.downloadTLH2Urls
                    : downloadData.downloadOtherFileUrls;
                if (downloadUrls == null || downloadUrls.Count == 0)
                {
                    Debug.LogWarning("未配置下载链接，确认按钮将保持不可用。");
                    return;
                }

                var downloadSystem = this.GetSystem<DownloadSystem>();
                var summary = await downloadSystem.DownloadAndSaveAsync(downloadUrls, targetSubFolder);
                confirm.interactable = summary is { SuccessCount: > 0 };
                if (summary != null)
                    Debug.Log($"下载完成：成功 {summary.SuccessCount}，失败 {summary.FailCount}，目录: {summary.SaveDirectory}");
            }
            catch (Exception)
            {
                Debug.LogWarning("完全下载失败");
            }
        }

        private void OnCongratulationsButtonClicked()
        {
            itemName.gameObject.SetActive(true);
            itemName.StartTyping();
        }

        private void RearrangePlayerShards()
        {
            var player = goalController.GetPlayer();
            if (player != null && Camera.main != null)
            {
                player.RearrangeShards(goalController.GetCameraPosition() + offset);
                StartCoroutine(EnableURLButton(2f));
            }
        }

        public IEnumerator MoveCanvas(Vector3 targetPosition, float duration)
        {
            canvas.sortingOrder = 6;
            result.interactable = false;
            var startPosition = transform.position;
            var elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                var t = Mathf.Clamp01(elapsedTime / duration);
                t = t * t * (3f - 2f * t); // SmoothStep
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                yield return null;
            }

            transform.position = targetPosition;
            congratulations.gameObject.SetActive(true);
            congratulations.StartTyping();
        }

        private IEnumerator EnableURLButton(float delay)
        {
            yield return new WaitForSeconds(delay);
            url.gameObject.SetActive(true);
        }

        private void OnURLButtonClicked()
        {
            this.GetSystem<DownloadSystem>().OpenFolder(targetSubFolder);
            this.SendCommand(new LoadSceneCommand(AddressableKeys.Scenes.MainMenu_Unity, false));
        }

        public void SetResultButtonInteractable(bool interactable)
        {
            result.interactable = interactable;
        }
    }
}