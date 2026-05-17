using System;
using System.Collections.Generic;
using HotUpdate.Audio.Commands;
using HotUpdate.Core;
using HotUpdate.Data.Model;
using HotUpdate.Interface;
using HotUpdate.Manager;
using HotUpdate.Utility;
using QFramework;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace HotUpdate.UI
{
    public class MainMenu : MonoBehaviour, IController, IAutoBind
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private BottomMenuNode bottomMenuNode;
        [SerializeField] private AppGroupNode appGroupNode;

public GameData GameData { get; private set; }
        public MainMenuData MainMenuData { get; private set; }
        public Dictionary<string, GameObject> Windows { get; } = new();

private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                this.SendCommand(new PlaySoundCommand(AddressableKeys.MouseClick_Wav));
            }
        }

public IArchitecture GetArchitecture()
        {
            return TheWorstGameInHistory.Interface;
        }

public void Init()
        {
            GameData = this.GetModel<GameDataModel>().CurrentGameData.Value;
            appGroupNode.Init(this);
            bottomMenuNode.Init(this);
            AddressablesManager.Instance.LoadAssetAsync<TextAsset>(
                AddressableKeys.MainMenuData_Json,
                handle =>
                {
                    var json = handle.Result.text;
                    MainMenuData = JsonUtility.FromJson<MainMenuData>(json);
                });
        }

public WindowBase OpenWindow(string windowAddress)
        {
            if (Windows.TryGetValue(windowAddress, out var existingGo))
            {
                existingGo.SetActive(true);
                return existingGo.GetComponent<WindowBase>();
            }

AddressablesManager.Instance.LoadAssetAsync<GameObject>(windowAddress, handle =>
            {
                if (!handle.IsValid() || handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
                {
                    return;
                }

var go = Instantiate(handle.Result, canvas.transform);
                var windowBase = go.GetComponent<WindowBase>();
                windowBase.Init(this);
                Windows[windowAddress] = go;
            });

return null;
        }

public void OpenWindow(string windowAddress, Action<WindowBase> onLoaded)
        {
            onLoaded ??= _ => { };

if (Windows.TryGetValue(windowAddress, out var existingGo))
            {
                existingGo.SetActive(true);
                onLoaded(existingGo.GetComponent<WindowBase>());
                return;
            }

AddressablesManager.Instance.LoadAssetAsync<GameObject>(windowAddress, handle =>
            {
                if (!handle.IsValid() || handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
                {
                    onLoaded(null);
                    return;
                }

var go = Instantiate(handle.Result, canvas.transform);
                var windowBase = go.GetComponent<WindowBase>();
                windowBase.Init(this);
                Windows[windowAddress] = go;
                onLoaded(windowBase);
            });
        }

public AppGroupNode GetAppGroup()
        {
            return appGroupNode;
        }
    }
}
