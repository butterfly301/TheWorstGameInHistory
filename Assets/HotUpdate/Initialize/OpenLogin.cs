using System.Collections;
using HotUpdate.Audio.Commands;
using HotUpdate.Manager;
using HotUpdate.UI;
using HotUpdate.Utility;
using QFramework;
using UnityEngine;
public class OpenLogin : Open
{

    private string musicName = AddressableKeys.TheSongOfGuanyu_Mp3;

    protected override IEnumerator InitializeSequence()
    {
        yield return null;
        //播放bgm
        this.SendCommand(new PlayMusicCommand(musicName));
        //初始化UI管理器
        AddressablesManager.Instance.LoadAssetAsync<GameObject>(AddressableKeys.UIManager3_Prefab,
        (handle) =>
        {
            UIManager3 uiManager3 = UIManager3.GetOrCreate(handle.Result);
            uiManager3.Init();
        });
        //初始化登录页
        AddressablesManager.Instance.LoadAssetAsync<GameObject>(AddressableKeys.LoginForm_Prefab,
        (handle) =>
        {
            GameObject loginFormObj = Instantiate(handle.Result);
            LoginForm loginForm = loginFormObj.GetComponent<LoginForm>();
            loginForm?.Init();
        });

    }

}
