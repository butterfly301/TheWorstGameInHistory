using System.Collections.Generic;
using HotUpdate.Interface;
using QFramework;
using UnityEngine;

namespace HotUpdate.UI
{
    public abstract class UIManager : MonoSingleton<UIManager>, IAutoBind
    {
        /// <summary>
        /// UI图层与Transform的映射字典
        /// </summary>
        protected Dictionary<object, Transform> uiLayerTrans = new();

        /// <summary>
        /// 初始化UI管理器
        /// </summary>
        public abstract void Init();
    }
}