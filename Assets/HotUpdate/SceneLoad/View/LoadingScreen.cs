using System.Collections;
using HotUpdate.Interface;
using HotUpdate.Utility;
using UnityEngine;
using UnityEngine.Serialization;

namespace HotUpdate.UI
{
    public class LoadingScreen : MonoBehaviour,IAutoBind
    {
        [SerializeField]private Camera cameraLoadingScreen;
        [SerializeField]private CanvasGroup loadingPanel;

        public virtual void Init()
        {
            DontDestroyOnLoad(gameObject);
            loadingPanel.alpha = 0;
        }

        public void FadeInCanvas()
        {
            StartCoroutine(FadeCanvasGroup(0f, 1f, 1f, false));
        }

        public void FadeOutCanvas()
        {
            cameraLoadingScreen.gameObject.SetActive(false);
            StartCoroutine(FadeCanvasGroup(1f, 0f, 1f, true));
        }

        private IEnumerator FadeCanvasGroup(float fromAlpha, float toAlpha, float duration, bool destroyOnComplete)
        {
            var elapsedTime = 0f;

            // 确保CanvasGroup存在
            if (loadingPanel == null)
                yield break;

            loadingPanel.alpha = fromAlpha;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                var currentAlpha = Mathf.Lerp(fromAlpha, toAlpha, elapsedTime / duration);
                loadingPanel.alpha = currentAlpha;
                yield return null;
            }

            // 确保最终值准确
            loadingPanel.alpha = toAlpha;
            // 如果是淡出效果且需要销毁
            if (destroyOnComplete) GetComponent<DestroyAfterDelay>()?.DestroyMyself();
        }
    }
}
