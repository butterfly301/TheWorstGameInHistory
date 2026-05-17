using System.Collections;
using System.Collections.Generic;
using HotUpdate.Interface;
using Kamgam.UGUIBlurredBackground;
using DG.Tweening;
using UnityEngine;

public class BlurForm : MonoBehaviour,IAutoBind
{
    [SerializeField]private BlurredBackgroundImage blurredBackgroundImage;
    private Tweener blurStrengthTween;

    public void Init()
    {
        // ensure initial state if needed
        if (blurredBackgroundImage != null)
        {
            // no-op for now; strength can be adjusted via AdjustBlurStrength
        }
    }

    public void AdjustBlurStrength(float changeValue, float duration = 0.5f)
    {
        if (blurredBackgroundImage == null)
            return;

        blurStrengthTween?.Kill();
        blurredBackgroundImage.DOKill();

        var target = Mathf.Clamp(blurredBackgroundImage.Strength + changeValue, 0f, 300f);
        blurStrengthTween = DOTween.To(
                () => blurredBackgroundImage.Strength,
                v => blurredBackgroundImage.Strength = v,
                target,
                duration)
            .SetEase(Ease.Linear);
    }

    private void OnDestroy()
    {
        blurStrengthTween?.Kill();
        if (blurredBackgroundImage != null)
        {
            blurredBackgroundImage.DOKill();
        }
    }
}
