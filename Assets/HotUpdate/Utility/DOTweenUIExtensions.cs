using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace DG.Tweening
{
    public static class DOTweenUIExtensions
    {
        public static TweenerCore<float, float, FloatOptions> DOFade(this CanvasGroup target, float endValue, float duration)
        {
            var tween = DOTween.To(() => target.alpha, value => target.alpha = value, endValue, duration);
            tween.SetTarget(target);
            return tween;
        }

        public static TweenerCore<Vector2, Vector2, VectorOptions> DOAnchorPos(this RectTransform target, Vector2 endValue, float duration, bool snapping = false)
        {
            var tween = DOTween.To(() => target.anchoredPosition, value => target.anchoredPosition = value, endValue, duration);
            tween.SetOptions(snapping).SetTarget(target);
            return tween;
        }
    }
}
