using System.Collections;
using HotUpdate.Audio.Commands;
using HotUpdate.Manager;
using HotUpdate.UI;
using HotUpdate.Utility;
using HotUpdate.Video;
using QFramework;
using UnityEngine;
public class OpenMainMenu : Open
{
    private string musicName = AddressableKeys.Rest_time_Mp3;

    protected override IEnumerator InitializeSequence()
    {
        yield return null;
        CleanupUIManagers();
        //播放bgm
        this.SendCommand(new PlayMusicCommand(musicName));
        //初始化摄像机
        AddressablesManager.Instance.LoadAssetAsync<GameObject>(AddressableKeys.InitCamera_Prefab,
        (handle) => { Instantiate(handle.Result); });
        //初始化主菜单
        AddressablesManager.Instance.LoadAssetAsync<GameObject>(AddressableKeys.MainMenuForm_Prefab,
        (handle) =>
        {
            GameObject mainMenuObj = Instantiate(handle.Result);
            MainMenuForm mainMenu = mainMenuObj.GetComponent<MainMenuForm>();
            mainMenu.Init();
        });

    }

    private void CleanupUIManagers()
    {
        if (UIManager1.HasInstance)
        {
            UIManager1.Instance.Dispose();
        }

        if (UIManager2.HasInstance)
        {
            UIManager2.Instance.Dispose();
        }

        if (UIManager3.HasInstance)
        {
            UIManager3.Instance.Dispose();
        }
    }

}
