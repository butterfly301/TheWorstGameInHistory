using System.Collections;
using System.Collections.Generic;
using HotUpdate.Interface;
using Kamgam.UGUIBlurredBackground;
using DG.Tweening;
using UnityEngine;

public class BlurForm : MonoBehaviour,IAutoBind
{
    private const float BlurRaycastThreshold = 0.001f;

    [SerializeField]private BlurredBackgroundImage blurredBackgroundImage;
    private Tweener blurStrengthTween;

    public void Init()
    {
        if (blurredBackgroundImage != null)
        {
            UpdateRaycastState(blurredBackgroundImage.Strength);
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
            .OnStart(() => UpdateRaycastState(target))
            .OnUpdate(() => UpdateRaycastState(blurredBackgroundImage.Strength))
            .OnComplete(() => UpdateRaycastState(blurredBackgroundImage.Strength))
            .SetEase(Ease.Linear);
    }

    private void UpdateRaycastState(float strength)
    {
        blurredBackgroundImage.raycastTarget = strength > BlurRaycastThreshold;
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
