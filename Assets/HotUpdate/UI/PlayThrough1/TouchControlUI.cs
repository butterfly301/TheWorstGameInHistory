using HotUpdate.Enums;
using HotUpdate.Manager;
using HotUpdate.Utility;
using UnityEngine;

namespace HotUpdate.UI
{
    /// <summary>
    /// 触屏控制管理器
    /// 负责管理移动端触屏控制器的显示
    /// </summary>
    public class TouchControlUI
    {
        private readonly Transform parentTransform;
        private GameObject touchControlsPrefab;
        private GameObject touchControlsObj;

public TouchControlUI(Transform parent)
        {
            parentTransform = parent;
        }

/// <summary>
        /// 初始化触屏控制系统
        /// </summary>
        public void Init()
        {
            if (!Application.isMobilePlatform)
                return;

AddressablesManager.Instance.LoadAssetAsync<GameObject>(
                AddressableKeys.TouchControls_Prefab,
                handle =>
                {
                    touchControlsPrefab = handle.Result;
                    touchControlsObj = Object.Instantiate(touchControlsPrefab, parentTransform);
                }
            );
        }

/// <summary>
        /// 显示触屏控制器
        /// </summary>
        public void ShowTouchControls()
        {
            if (touchControlsObj != null)
                touchControlsObj.SetActive(true);
        }

/// <summary>
        /// 隐藏触屏控制器
        /// </summary>
        public void HideTouchControls()
        {
            if (touchControlsObj != null)
                touchControlsObj.SetActive(false);
        }
    }
}
