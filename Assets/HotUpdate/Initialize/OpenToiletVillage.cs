using System.Collections;
using HotUpdate.Audio.Commands;
using HotUpdate.Character;
using HotUpdate.Manager;
using HotUpdate.SceneLoad.Commands;
using HotUpdate.UI;
using HotUpdate.Utility;
using QFramework;
using UnityEngine;
using HotUpdate.Enemy;
public class OpenToiletVillage : Open
{
    private ISafire2DCamera cam;
    private GameObject playerObj;
    private string[] enemyNames = { "BugToiletVillage" };
    private string musicName = AddressableKeys.Circulation_Mp3;

    protected override IEnumerator InitializeSequence()
    {
        //初始化UI管理器
        yield return StartCoroutine(InitializeUIManager());
        //初始化敌人管理器
        yield return StartCoroutine(InitializeEnemyManager());
        //初始化场景
        yield return StartCoroutine(InitializeToiletVillagePlatform());
        // 初始化玩家和摄像机
        yield return StartCoroutine(InitializePlayerAndCamera());
        //删除之前的存档
        WorldManagerBase.Instance.DeleteAllSavedData();
        //播放背景音乐
        this.SendCommand(new PlayMusicCommand(musicName));
        //全部加载完毕。拉开帷幕
        this.SendCommand(new HideLoadingScreenCommand());
    }
    IEnumerator InitializeUIManager()
    {
        bool initCompleted = false;
        AddressablesManager.Instance.LoadAssetAsync<GameObject>(AddressableKeys.UIManager1_Prefab, (handle) =>
        {
            GameObject uiManagerObj = Instantiate(handle.Result);
            UIManager1 uiManager1 = uiManagerObj.GetComponent<UIManager1>();
            uiManager1.Init();
            initCompleted = true;

        });
        yield return new WaitUntil(() => initCompleted);
    }
    // **协程被简化为一个普通方法**
    IEnumerator InitializeEnemyManager()
    {
        bool initCompleted = false;
        // 加载并实例化EnemyManager的预制体
        var prefab =
        AddressablesManager.Instance.LoadAssetSynchronously<GameObject>(AddressableKeys.EnemyManager_Prefab);
        if (prefab != null)
        {
            GameObject enemyManagerObj = Instantiate(prefab);
            EnemyManager enemyManager = enemyManagerObj.GetComponent<EnemyManager>();
            // 直接调用同步的Init方法，程序会在这里等待直到加载完成
            enemyManager.Init(enemyNames);
            initCompleted = true;
        }
        yield return new WaitUntil(() => initCompleted);
    }
    IEnumerator InitializePlayerAndCamera()
    {
        bool loadingCompleted = false;
        AddressablesManager.Instance.LoadAssetAsync<GameObject>(AddressableKeys.ToiletVillageCamera_Prefab,
        (handle) =>
        {
            GameObject camObj = Instantiate(handle.Result);
            cam = camObj.GetComponent<ISafire2DCamera>();
            AddressablesManager.Instance.LoadAssetAsync<GameObject>(
            AddressableKeys.Player1_Prefab,
            (handle2) =>
            {
                playerObj = Instantiate(handle2.Result);
                CharacterReference cr = playerObj.GetComponent<CharacterReference>();
                cr.Init();
                cam.ChangeTargetTransform(playerObj.transform);
                loadingCompleted = true; // 标记加载完成
            });
        });
        // 等待加载完成
        yield return new WaitUntil(() => loadingCompleted);
    }
    IEnumerator InitializeToiletVillagePlatform()
    {
        bool initCompleted = false;
        AddressablesManager.Instance.LoadAssetAsync<GameObject>(AddressableKeys.ToiletVillagePlatform_Prefab,
        (handle) =>
        { 
        Instantiate(handle.Result); 
        initCompleted = true; // 标记加载完成
    });
    yield return new WaitUntil(() => initCompleted);

}

}
