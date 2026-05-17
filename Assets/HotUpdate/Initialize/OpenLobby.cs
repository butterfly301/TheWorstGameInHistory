using System.Collections.Generic;
using UnityEngine;
using QFramework;
using HotUpdate.Utility;
using System.Collections;
using HotUpdate.Audio.Commands;
using HotUpdate.Manager;
using HotUpdate.SceneLoad.Commands;
public class OpenLobby : Open
{
    private string BGMAddress = AddressableKeys.TheSongOfXiaoQiao_Mp3;
    protected override IEnumerator InitializeSequence()
    {
        yield return null;
        //  播放bgm
        this.SendCommand(new PlayMusicCommand(BGMAddress));
        // 初始化摄像机
        AddressablesManager.Instance.LoadAssetAsync<GameObject>(AddressableKeys.InitCamera_Prefab,
            (handle) => { Instantiate(handle.Result); });
        // 初始化ui管理器
        AddressablesManager.Instance.LoadAssetAsync<GameObject>(AddressableKeys.UIManager3_Prefab,
            (handle) =>
            {   GameObject uiManager = Instantiate(handle.Result);
                uiManager.GetComponent<UIManager3>()?.Init();
            });
        // 初始化世界
        AddressablesManager.Instance.LoadAssetAsync<GameObject>(AddressableKeys.LobbyWorld_Prefab,
            (handle) => { 
                GameObject lobbyWorld = Instantiate(handle.Result);
                lobbyWorld.GetComponent<LobbyWorld>()?.Init();
            });
        //全部加载完毕。拉开帷幕
        this.SendCommand(new HideLoadingScreenCommand());
    }
}


