using System.Collections;
using System.Collections.Generic;
using HotUpdate.Audio.Commands;
using HotUpdate.Manager;
using HotUpdate.MiniGame.IceBreaker;
using HotUpdate.SceneLoad.Commands;
using HotUpdate.Utility;
using QFramework;
using UnityEngine;
using UnityEngine.UI;
    public class OpenIceBreaker : Open
    {
        private ISafire2DCamera cam;
        private GameObject playerObj;

        protected override IEnumerator InitializeSequence()
        {
            //播放bgm
            this.SendCommand(new PlayMusicCommand(AddressableKeys.Fruit_Machine_Mp3));
            //初始化管理器
            yield return StartCoroutine(InitializeIceBreakerManager());
            //初始化摄像机
            yield return StartCoroutine(InitializeCameraPlayerLevel());
            //全部加载完毕。拉开帷幕
            this.SendCommand(new HideLoadingScreenCommand());
        }

        IEnumerator InitializeIceBreakerManager()
        {
            bool initCompleted = false;
            AddressablesManager.Instance.LoadAssetAsync<GameObject>(AddressableKeys.IceBreakerManager_Prefab,
                (handle) =>
                {
                    Instantiate(handle.Result);
                    initCompleted = true;
                });
            yield return new WaitUntil(() => initCompleted);
        }

        IEnumerator InitializeCameraPlayerLevel()
        {
            bool initCompleted = false;
            AddressablesManager.Instance.LoadAssetAsync<GameObject>(AddressableKeys.IceBreakerCamera_Prefab,
                (handle) =>
                {
                    GameObject camObj = Instantiate(handle.Result);
                    cam = camObj.GetComponent<ISafire2DCamera>();
                    //初始化玩家
                    AddressablesManager.Instance.LoadAssetAsync<GameObject>(
                        AddressableKeys.IceBreakerPlayer_Prefab,
                        (handle2) =>
                        {
                            playerObj = Instantiate(handle2.Result);
                            cam.ChangeTargetTransform(playerObj.transform);
                            IceBreakerPlayerController playerController =
                                playerObj.GetComponent<IceBreakerPlayerController>();
                            playerController.Init();

                            //初始化关卡
                            AddressablesManager.Instance.LoadAssetAsync<GameObject>(
                                AddressableKeys.LevelGenerator_Prefab,
                                (handle3) =>
                                {
                                    GameObject levelGenObj = Instantiate(handle3.Result);
                                    LevelGenerator levelGenerator = levelGenObj.GetComponent<LevelGenerator>();
                                    levelGenerator.Init(playerObj.transform);
                                    initCompleted = true;
                                });
                        });
                });
            yield return new WaitUntil(() => initCompleted);
        }
    }