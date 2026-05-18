using HotUpdate.Enums;
using HotUpdate.Manager;
using HotUpdate.Utility;
using UnityEngine;

namespace HotUpdate.UI
{
    /// <summary>
    /// 故障特效管理器
    /// 负责管理故障特效的淡入淡出
    /// </summary>
    public class GlitchEffectUI
    {
        private readonly Transform parentTransform;
        private GameObject glitchEffectPrefab;
        private GameObject glitchEffectObj;
        private GlitchEffectForm glitchEffectForm;

        public GlitchEffectUI(Transform parent)
        {
            parentTransform = parent;
        }

        /// <summary>
        /// 初始化故障特效系统
        /// </summary>
        public void Init()
        {
            if (glitchEffectObj != null) return;

            AddressablesManager.Instance.LoadAssetAsync<GameObject>(
                AddressableKeys.GlitchEffectForm_Prefab,
                handle =>
                {
                    glitchEffectPrefab = handle.Result;
                    glitchEffectObj = Object.Instantiate(glitchEffectPrefab, parentTransform);
                    glitchEffectForm = glitchEffectObj.GetComponent<GlitchEffectForm>();
                    glitchEffectForm.Init();
                    Close();
                }
            );
        }

        public void Open()
        {
            glitchEffectObj?.SetActive(true);
        }

        public void Close()
        {
            glitchEffectObj?.SetActive(false);
        }

        /// <summary>
        /// 调整故障特效强度
        /// </summary>
        public void AdjustGlitchEffect(float changeValue)
        {
            glitchEffectForm?.AdjustGlitchEffect(changeValue);
        }
    }
}
