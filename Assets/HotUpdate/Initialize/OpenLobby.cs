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
    
    private LobbyWorldNode lobbyWorld;

    protected override IEnumerator InitializeSequence()
    {
        yield return null;
        //  播放bgm
        this.SendCommand(new PlayMusicCommand(BGMAddress));
        // 初始化世界
        yield return StartCoroutine(InitializeLobbyWorld());
        // 初始化ui管理器
        yield return StartCoroutine(InitializeUIManager());
        //全部加载完毕。拉开帷幕
        this.SendCommand(new HideLoadingScreenCommand());
    }

    IEnumerator InitializeLobbyWorld()
    {
        bool initCompleted = false;
        AddressablesManager.Instance.LoadAssetAsync<GameObject>(AddressableKeys.LobbyWorld_Prefab,
            (handle) =>
            {
                GameObject lobbyWorldObj = Instantiate(handle.Result);
                this.lobbyWorld = lobbyWorldObj.GetComponent<LobbyWorldNode>();
                this.lobbyWorld.Init();
                initCompleted = true;
            });
        yield return new WaitUntil(() => initCompleted);
    }

    IEnumerator InitializeUIManager()
    {
        bool initCompleted = false;
        AddressablesManager.Instance.LoadAssetAsync<GameObject>(AddressableKeys.UIManager3_Prefab,
            (handle) =>
            {
                GameObject uiManagerObj = Instantiate(handle.Result);
                UIManager3 uIManager3 = uiManagerObj.GetComponent<UIManager3>();
                uIManager3?.Init();
                initCompleted = true;
            });
        yield return new WaitUntil(() => initCompleted);
    }
}

