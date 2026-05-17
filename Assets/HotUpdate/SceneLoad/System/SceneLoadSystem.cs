using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotUpdate.Manager;
using HotUpdate.UI;
using HotUpdate.Utility;
using QFramework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

// 场景加载系统
namespace HotUpdate.SceneLoad.System
{
    // 加载界面类型枚举（可扩展）
    public enum LoadingScreenType
    {
        Default,
        PlayThrough1,
        PlayThrough2,
        PlayThrough3
        // 未来新增在此扩展
    }

public class SceneLoadSystem : AbstractSystem
    {
        // 预制体缓存：不同类型的LoadingScreen预制体
        private readonly Dictionary<LoadingScreenType, GameObject> prefabCache = new();

// 类型到地址的映射（根据项目调整具体地址）
        private readonly Dictionary<LoadingScreenType, string> typeToAddress = new()
        {
            { LoadingScreenType.Default, AddressableKeys.LoadingScreenDefault_Prefab },
            { LoadingScreenType.PlayThrough1, AddressableKeys.LoadingScreenPlayThrough1_Prefab},
            { LoadingScreenType.PlayThrough2, AddressableKeys.LoadingScreenPlayThrough2_Prefab },
            { LoadingScreenType.PlayThrough3, AddressableKeys.LoadingScreenPlayThrough3_Prefab }
        };

private AsyncOperationHandle<SceneInstance> currentSceneHandle;
        private LoadingScreen loadingScreen;
        private GameObject loadingScreenObj;
        public bool IsLoading { get; private set; }

protected override void OnInit()
        {
            // 预加载所有映射的加载界面预制体
            foreach (var kv in typeToAddress)
            {
                var type = kv.Key;
                var address = kv.Value;

AddressablesManager.Instance.LoadAssetAsync<GameObject>(address, handle =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
                        prefabCache[type] = handle.Result;
                    else
                        Debug.LogError($"加载界面预制体预加载失败: {type} -> {address}");
                });
            }
        }

// 异步加载场景命令（按类型选择加载界面）
        public async void LoadSceneAsync(string sceneAddress, bool isFade = true,
            LoadingScreenType screenType = LoadingScreenType.Default, float extraWaitSeconds = 3f)
        {
            try
            {
                // 设置加载状态为true
                IsLoading = true;

if (isFade) ShowLoadingScreen(screenType);

// 如果当前有场景正在运行，先卸载
                if (currentSceneHandle.IsValid()) await Addressables.UnloadSceneAsync(currentSceneHandle).Task;

// 加载新场景
                currentSceneHandle = Addressables.LoadSceneAsync(sceneAddress, LoadSceneMode.Additive);
                await currentSceneHandle.Task;

if (isFade && screenType != LoadingScreenType.Default && extraWaitSeconds > 0f)
                    await Task.Delay(TimeSpan.FromSeconds(extraWaitSeconds));

// 设置新场景为活动场景
                if (currentSceneHandle.Result.Scene.isLoaded) SceneManager.SetActiveScene(currentSceneHandle.Result.Scene);

// 设置加载状态为false
                IsLoading = false;
            }
            catch (Exception e)
            {
                e.LogError();
            }
        }

private void ShowLoadingScreen(LoadingScreenType type)
        {
            // 从缓存中获取对应类型的预制体；若没有则回退到默认
            if (!prefabCache.TryGetValue(type, out var prefab))
                prefabCache.TryGetValue(LoadingScreenType.Default, out prefab);
            if (prefab == null) return;

loadingScreenObj = Object.Instantiate(prefab);
            loadingScreen = loadingScreenObj.GetComponent<LoadingScreen>();
            loadingScreen?.Init();
            loadingScreen?.FadeInCanvas();
        }

public void HideLoadingScreen()
        {
            if (loadingScreenObj != null)
            {
                loadingScreen?.FadeOutCanvas();
                loadingScreenObj = null;
                loadingScreen = null;
            }
        }

public void UnloadCurrentScene()
        {
            if (currentSceneHandle.IsValid())
            {
                Addressables.UnloadSceneAsync(currentSceneHandle);
                currentSceneHandle = default;
            }
        }

public string GetCurrentSceneName()
        {
            if (currentSceneHandle.IsValid() && currentSceneHandle.Result.Scene.isLoaded)
                return currentSceneHandle.Result.Scene.name;
            return string.Empty;
        }
    }
}
