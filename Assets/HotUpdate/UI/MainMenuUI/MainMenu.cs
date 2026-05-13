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
using UnityEngine.Serialization;

namespace HotUpdate.UI
{
    public class MainMenu : MonoBehaviour, IController,IAutoBind
    {
        [SerializeField]private Canvas canvas;
        [SerializeField]private BottomMenuNode bottomMenuNode;
        [SerializeField]private AppGroupNode appGroupNode;

        public GameData GameData { get; private set; }

        public MainMenuData MainMenuData { get; private set; }

        public Dictionary<AddressableKeys.Prefabs.UI.MainMenu.Window.WindowName, GameObject> Windows { get; } = new();

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                this.SendCommand(new PlaySoundCommand(AddressableKeys.Audio.Sound.MouseClick_Wav));
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
            AddressablesManager.Instance.LoadAssetAsync<TextAsset>(AddressableKeys.Data.MainMenu.MainMenuData_Json,
                handle =>
                {
                    var json = handle.Result.text;
                    MainMenuData = JsonUtility.FromJson<MainMenuData>(json);
                });
        }

        public WindowBase OpenWindow(AddressableKeys.Prefabs.UI.MainMenu.Window.WindowName windowName)
        {
            // If window already loaded, activate and return it
            if (Windows.TryGetValue(windowName, out var existingGo))
            {
                existingGo.SetActive(true);
                return existingGo.GetComponent<WindowBase>();
            }

            // Not loaded yet — start asynchronous load. Don't access windows[windowName] here.
            AddressablesManager.Instance.LoadAssetAsync<GameObject>(
                AddressableKeys.Prefabs.UI.MainMenu.Window.GetWindow(windowName.ToString()),
                handle =>
                {
                    // Validate async handle and result
                    if (!handle.IsValid() || handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
                        // Load failed or returned null; nothing to add
                        return;

                    var go = Instantiate(handle.Result, canvas.transform);
                    var windowBase = go.GetComponent<WindowBase>();
                    windowBase.Init(this);
                    // Save to dictionary for future calls
                    Windows[windowName] = go;
                });

            // Window is loading asynchronously. Return null so caller won't try to use a missing entry.
            return null;
        }

        /// <summary>
        ///     安全异步打开窗口的重载：如果窗口已存在则立即回调，否则异步加载并在加载完成后回调（失败时回调 null）。
        /// </summary>
        public void OpenWindow(AddressableKeys.Prefabs.UI.MainMenu.Window.WindowName windowName, Action<WindowBase> onLoaded)
        {
            onLoaded ??= _ => { };

            // If already loaded, activate and invoke callback immediately
            if (Windows.TryGetValue(windowName, out var existingGo))
            {
                existingGo.SetActive(true);
                onLoaded(existingGo.GetComponent<WindowBase>());
                return;
            }

            // Start async load and invoke callback when ready
            AddressablesManager.Instance.LoadAssetAsync<GameObject>(
                AddressableKeys.Prefabs.UI.MainMenu.Window.GetWindow(windowName.ToString()),
                handle =>
                {
                    if (!handle.IsValid() || handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
                    {
                        // Load failed; return null to signal failure
                        onLoaded(null);
                        return;
                    }

                    var go = Instantiate(handle.Result, canvas.transform);
                    var windowBase = go.GetComponent<WindowBase>();
                    windowBase.Init(this);

                    Windows[windowName] = go;

                    onLoaded(windowBase);
                });
        }

        public AppGroupNode GetAppGroup()
        {
            return appGroupNode;
        }
    }
}