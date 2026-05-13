using System.Collections;
using HotUpdate.Audio.Commands;
using HotUpdate.Manager;
using HotUpdate.Utility;
using QFramework;
using UnityEngine;

public class OpenLogin : Open
{
    private string musicName = AddressableKeys.Audio.BGM.ReplaceInTheFuture.TheSongOfGuanyu_Mp3;
    protected override IEnumerator InitializeSequence()
    {
        yield return null;
        //播放bgm
        this.SendCommand(new PlayMusicCommand(musicName));
        //初始化摄像机
        AddressablesManager.Instance.LoadAssetAsync<GameObject>(AddressableKeys.Prefabs.Camera.InitCamera_Prefab,
            (handle) => { Instantiate(handle.Result); });
        //初始化登录页
        AddressablesManager.Instance.LoadAssetAsync<GameObject>(AddressableKeys.Prefabs.UI.Playthrough3.LoginForm_Prefab,
            (handle) =>
            {
                GameObject loginFormObj = Instantiate(handle.Result);
                LoginForm loginForm = loginFormObj.GetComponent<LoginForm>();
                loginForm?.Init();
            });
    }
}
