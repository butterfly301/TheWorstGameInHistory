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
        private GameObject videoManagerObj;
        private VideoManager _videoManager;
        private string musicName = AddressableKeys.Audio.BGM.Rest_time_Mp3;

        protected override IEnumerator InitializeSequence()
        {
            yield return null;
            //播放bgm
            this.SendCommand(new PlayMusicCommand(musicName));
            //初始化摄像机
            AddressablesManager.Instance.LoadAssetAsync<GameObject>(AddressableKeys.Prefabs.Camera.InitCamera_Prefab,
                (handle) => { Instantiate(handle.Result); });
            //初始化主菜单
            AddressablesManager.Instance.LoadAssetAsync<GameObject>(AddressableKeys.Prefabs.UI.MainMenu.MainMenuForm_Prefab,
                (handle) =>
                {
                    GameObject mainMenuObj = Instantiate(handle.Result);
                    MainMenu mainMenu = mainMenuObj.GetComponent<MainMenu>();
                    mainMenu.Init();
                });
        }
    }