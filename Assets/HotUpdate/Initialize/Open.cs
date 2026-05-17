using System.Collections;
using HotUpdate.Core;
using HotUpdate.SceneLoad.System;
using QFramework;
using UnityEngine;
public class Open : MonoBehaviour, IController
{
    void Start()
    {
        StartCoroutine(WaitForSceneLoadComplete());

    }
    IEnumerator WaitForSceneLoadComplete()
    {
        // 等待场景加载完成
        var sceneLoadSystem = this.GetSystem<SceneLoadSystem>();
        yield return new WaitUntil(() => !sceneLoadSystem.IsLoading);
        // 场景加载完成后开始初始化序列
        yield return StartCoroutine(InitializeSequence());

    }

    protected virtual IEnumerator InitializeSequence()
    {
        yield return null;

    }

    public IArchitecture GetArchitecture()
    {
        return TheWorstGameInHistory.Interface;

    }

}
