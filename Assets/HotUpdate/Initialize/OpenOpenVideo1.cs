using System.Collections;
using HotUpdate.Manager;
using HotUpdate.SceneLoad.Commands;
using HotUpdate.SceneLoad.System;
using HotUpdate.UI;
using HotUpdate.Utility;
using HotUpdate.Video;
using QFramework;
using UnityEngine;
public class OpenOpenVideo1 : Open
{
    protected override IEnumerator InitializeSequence()
    {
        yield return null;
        //初始化摄像机
        AddressablesManager.Instance.LoadAssetAsync<GameObject>(AddressableKeys.InitCamera_Prefab,
            (handle) => { Instantiate(handle.Result); });
        //初始化UI管理器
        AddressablesManager.Instance.LoadAssetAsync<GameObject>(AddressableKeys.UIManager1_Prefab,
            (handle) =>
            {
                GameObject uiManagerObj = Instantiate(handle.Result);
                UIManager1 uiManager1 = uiManagerObj.GetComponent<UIManager1>();
                uiManager1.Init();
            });
        //初始化视频管理器
        AddressablesManager.Instance.LoadAssetAsync<GameObject>(AddressableKeys.VideoManager_Prefab,
            (handle) =>
            {
                GameObject videoManagerObj = Instantiate(handle.Result);
                VideoManager videoManager = videoManagerObj.GetComponent<VideoManager>();
                videoManager.Init(GameName.TLH1);
                VideoManager.Instance.OnVideoEndEvent += OnOpenVideoEnd;
                VideoManager.Instance.PlayVideo(VideoKeys.Opening.OpenVideo0);
            });
    }

    private void OnOpenVideoEnd()
    {
        this.SendCommand(new LoadSceneCommand(AddressableKeys.ToiletVillage_Unity, true,
            LoadingScreenType.PlayThrough1));
    }
}