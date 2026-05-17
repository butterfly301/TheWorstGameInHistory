using System;
using HotUpdate.Enums;
using HotUpdate.Manager;
using HotUpdate.Utility;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HotUpdate.UI
{
    /// <summary>
    /// 暂停UI管理器
    /// 负责管理暂停面板的显示和隐藏
    /// </summary>
    public class PauseUI
    {
        private readonly Transform parentTransform;
        private GameObject pauseUIPrefab;
        private GameObject pauseUIObj;

        public PauseUI(Transform parent)
        {
            parentTransform = parent;
        }

        /// <summary>
        /// 初始化暂停UI系统
        /// </summary>
        public void Init()
        {
            AddressablesManager.Instance.LoadAssetAsync<GameObject>(
                AddressableKeys.PauseForm_Prefab,
                handle =>
                {
                    pauseUIPrefab = handle.Result;
                    pauseUIObj = Object.Instantiate(pauseUIPrefab, parentTransform);
                    pauseUIObj.GetComponent<global::PauseForm>().Init();
                    ClosePausePanel();
                }
            );

            RegisterPauseEvent();
        }

        /// <summary>
        /// 打开暂停面板
        /// </summary>
        public void OpenPausePanel()
        {
            if (pauseUIObj != null) pauseUIObj.SetActive(true);
        }

        /// <summary>
        /// 关闭暂停面板
        /// </summary>
        public void ClosePausePanel()
        {
            if (pauseUIObj != null) pauseUIObj.SetActive(false);
        }

        /// <summary>
        /// 注册暂停事件
        /// </summary>
        private void RegisterPauseEvent()
        {
            WorldManagerBase.Instance.RegisterEvent("onPause", OpenPausePanel);
            WorldManagerBase.Instance.RegisterEvent("onUnpause", ClosePausePanel);
        }
    }
}
