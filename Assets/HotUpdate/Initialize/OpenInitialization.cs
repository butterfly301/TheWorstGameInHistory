using HotUpdate.SceneLoad.Commands;
using HotUpdate.SceneLoad.System;
using HotUpdate.Utility;
using QFramework;
using UnityEngine;
    public class OpenInitialization : Open
    {
        private void Awake()
        {
            Debug.Log("=== 应用程序启动 ===");
            this.SendCommand(new LoadSceneCommand(AddressableKeys.Scenes.MainMenu_Unity, false));
        }
    }