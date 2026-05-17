using System.Collections;
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
        private CanvasGroup glitchEffectCanvasGroup;
        private bool isFade;

        public GlitchEffectUI(Transform parent)
        {
            parentTransform = parent;
        }

        /// <summary>
        /// 初始化故障特效系统
        /// </summary>
        public void Init()
        {
            AddressablesManager.Instance.LoadAssetAsync<GameObject>(
                AddressableKeys.GlitchEffect_Prefab,
                handle => { glitchEffectPrefab = handle.Result; }
            );
        }

        /// <summary>
        /// 调整故障特效强度
        /// </summary>
        public void AdjustGlitchEffect(float changeValue)
        {
            if (isFade)
                return;

            if (glitchEffectCanvasGroup == null)
            {
                glitchEffectObj = Object.Instantiate(glitchEffectPrefab, parentTransform);
                glitchEffectCanvasGroup = glitchEffectObj.GetComponent<CanvasGroup>();
                glitchEffectCanvasGroup.alpha = 0;
                StartFadeCanvasGroup(changeValue);
            }
            else
            {
                StartFadeCanvasGroup(changeValue);
            }
        }

        private void StartFadeCanvasGroup(float changeValue)
        {
            var currentValue = glitchEffectCanvasGroup.alpha;
            CoroutineRunner.Instance.StartCoroutine(FadeCanvasGroup(currentValue, currentValue + changeValue, 0.5f));
        }

        private IEnumerator FadeCanvasGroup(float fromAlpha, float toAlpha, float duration)
        {
            isFade = true;
            var elapsedTime = 0f;

            if (glitchEffectCanvasGroup == null)
                yield break;

            glitchEffectCanvasGroup.alpha = fromAlpha;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                var currentAlpha = Mathf.Lerp(fromAlpha, toAlpha, elapsedTime / duration);
                glitchEffectCanvasGroup.alpha = currentAlpha;
                yield return null;
            }

            glitchEffectCanvasGroup.alpha = toAlpha;
            if (toAlpha == 0) glitchEffectObj.GetComponent<DestroyAfterDelay>()?.DestroyMyself();

            isFade = false;
        }
    }

    /// <summary>
    /// 用于在MonoBehaviour上下文之外运行协程的辅助类
    /// </summary>
    public class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner _instance;
        public static CoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("CoroutineRunner");
                    _instance = go.AddComponent<CoroutineRunner>();
                    Object.DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }
    }
}
