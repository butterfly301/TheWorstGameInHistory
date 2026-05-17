using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CommonButton : Button
{
    [SerializeField] private float pressedScaleMultiplier = 0.9f;
    [SerializeField] private float scaleDuration = 0.05f;
    [SerializeField] private Ease scaleEase = Ease.OutQuad;

private Vector3 originalScale;
    private Tween scaleTween;

protected override void Awake()
    {
        base.Awake();
        originalScale = transform.localScale;
    }

public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        PlayScaleAnimation(originalScale * pressedScaleMultiplier);
    }

public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        RestoreScale();
    }

public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

if (IsPressed())
        {
            RestoreScale();
        }
    }

protected override void OnDisable()
    {
        base.OnDisable();
        StopScaleAnimation();
        RestoreScale();
    }

private void RestoreScale()
    {
        transform.localScale = originalScale;
    }

private void PlayScaleAnimation(Vector3 targetScale)
    {
        StopScaleAnimation();

if (scaleDuration <= 0f)
        {
            transform.localScale = targetScale;
            return;
        }

scaleTween = DOTween.To(
                () => transform.localScale,
                value => transform.localScale = value,
                targetScale,
                scaleDuration)
            .SetEase(scaleEase)
            .SetUpdate(true)
            .OnKill(() => scaleTween = null);
    }

private void StopScaleAnimation()
    {
        if (scaleTween == null)
        {
            return;
        }

scaleTween.Kill();
        scaleTween = null;
    }
}
