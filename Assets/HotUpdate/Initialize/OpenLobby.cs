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
    private PlayThrough3Data playThrough3Data;

    protected override IEnumerator InitializeSequence()
    {
        yield return null;
        //  播放bgm
        this.SendCommand(new PlayMusicCommand(BGMAddress));
        // 初始化世界
        yield return StartCoroutine(InitializeLobbyWorld());
        // 初始化三周目大厅数据
        yield return StartCoroutine(LoadPlayThrough3Data());
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
        if (UIManager3.HasInstance)
        {
            UIManager3.Instance.Lobby?.Open();
            UIManager3.Instance.PopUp?.Init(playThrough3Data?.popUpData);
            yield break;
        }
    }

    IEnumerator LoadPlayThrough3Data()
    {
        bool loadCompleted = false;
        AddressablesManager.Instance.LoadAssetAsync<TextAsset>(AddressableKeys.PlayThrough3Data_Json,
            handle =>
            {
                var json = handle.Result.text;
                playThrough3Data = JsonUtility.FromJson<PlayThrough3Data>(json);
                loadCompleted = true;
            });
        yield return new WaitUntil(() => loadCompleted);
    }
}
