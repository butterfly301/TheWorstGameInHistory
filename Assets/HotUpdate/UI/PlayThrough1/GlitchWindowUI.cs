using HotUpdate.Enums;
using HotUpdate.Manager;
using HotUpdate.Utility;
using UnityEngine;

namespace HotUpdate.UI
{
    /// <summary>
    /// 故障窗口管理器
    /// 负责管理故障窗口的显示
    /// </summary>
    public class GlitchWindowUI
    {
        private readonly Transform parentTransform;
        private GameObject glitchWindowPrefab;
        private GameObject glitchWindowObj;
        private GlitchWindow glitchWindow;

        public GlitchWindowUI(Transform parent)
        {
            parentTransform = parent;
        }

        /// <summary>
        /// 初始化故障窗口系统
        /// </summary>
        public void Init()
        {
            // 故障窗口的Prefab会在需要时加载
        }

        /// <summary>
        /// 加载故障窗口Prefab
        /// </summary>
        public void LoadGlitchWindowPrefab()
        {
            AddressablesManager.Instance.LoadAssetAsync<GameObject>(
                AddressableKeys.Prefabs.UI.Playthrough1.GlitchWindow_Prefab,
                handle => { glitchWindowPrefab = handle.Result; }
            );
        }

        /// <summary>
        /// 打开故障窗口
        /// </summary>
        public void OpenGlitchWindow()
        {
            if (glitchWindowPrefab == null)
            {
                LoadGlitchWindowPrefab();
                return;
            }

            glitchWindowObj = Object.Instantiate(glitchWindowPrefab, parentTransform);
            glitchWindow = glitchWindowObj.GetComponent<GlitchWindow>();
            glitchWindow.Init();
        }
    }
}
