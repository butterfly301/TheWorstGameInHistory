using DG.Tweening;
using HotUpdate.Utility;
using UnityEngine;

public class GlitchEffectForm : MonoBehaviour
{
    private CanvasGroup glitchEffectCanvasGroup;
    private Tweener glitchEffectTween;

    public void Init()
    {
        glitchEffectCanvasGroup = GetComponent<CanvasGroup>();
        if (glitchEffectCanvasGroup != null)
        {
            glitchEffectCanvasGroup.alpha = 0;
        }
    }

    public void AdjustGlitchEffect(float changeValue)
    {
        if (glitchEffectCanvasGroup == null)
        {
            return;
        }

        glitchEffectTween?.Kill();
        var targetAlpha = Mathf.Clamp01(glitchEffectCanvasGroup.alpha + changeValue);
        glitchEffectTween = DOTween.To(
                () => glitchEffectCanvasGroup.alpha,
                value => glitchEffectCanvasGroup.alpha = value,
                targetAlpha,
                0.5f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                if (targetAlpha == 0)
                {
                    GetComponent<DestroyAfterDelay>()?.DestroyMyself();
                }
            });
    }

    private void OnDestroy()
    {
        glitchEffectTween?.Kill();
        if (glitchEffectCanvasGroup != null)
        {
            glitchEffectCanvasGroup.DOKill();
        }
    }
}
