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

            AddressablesManager.Instance.LoadAssetAsync<GameObject>(

                AddressableKeys.GlitchWindow_Prefab,

                handle =>

                {

                    glitchWindowPrefab = handle.Result;

                    glitchWindowObj = Object.Instantiate(glitchWindowPrefab, parentTransform);

                    glitchWindow = glitchWindowObj.GetComponent<GlitchWindow>();

                    glitchWindow.Init();

                }

            );

        }



        /// <summary>

        /// 打开故障窗口

        /// </summary>

        public void OpenGlitchWindow()

        {

            if (glitchWindowObj != null)

                glitchWindowObj.SetActive(true);

        }

    }

}

