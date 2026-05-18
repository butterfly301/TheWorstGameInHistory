using DG.Tweening;
using HotUpdate.Interface;
using TMPro;
using UnityEngine;

public class TipItemNode : MonoBehaviour,IAutoBind
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private TextMeshProUGUI txtTip;
    [SerializeField] private CanvasGroup cgpTip;

    public RectTransform RectTransform => rectTransform;
    public TextMeshProUGUI TxtTip => txtTip;
    public CanvasGroup CgpTip => cgpTip;
    public Sequence Sequence { get; set; }

    public bool IsValid()
    {
        return rectTransform != null && txtTip != null && cgpTip != null;
    }

    public void SetContent(string content)
    {
        if (txtTip == null) return;
        txtTip.text = content;
    }

    public void ResetState(Vector2 originPosition, float scale)
    {
        if (!IsValid()) return;
        Sequence?.Kill(false);
        Sequence = null;
        RectTransform.anchoredPosition = originPosition;
        RectTransform.localScale = Vector3.one * scale;
        CgpTip.alpha = 1f;
        gameObject.SetActive(true);
    }

    public void Play(Vector2 targetPosition, float moveDuration, float bornDuration, float fadeDuration, System.Action onComplete)
    {
        if (!IsValid()) return;
        Sequence?.Kill(false);
        Sequence = DOTween.Sequence()
            .Join(RectTransform.DOScale(1f, bornDuration).SetEase(Ease.OutCubic))
            .Join(RectTransform.DOAnchorPos(targetPosition, moveDuration).SetEase(Ease.Linear))
            .Append(CgpTip.DOFade(0f, fadeDuration))
            .OnComplete(() => onComplete?.Invoke());
    }

    public void Clear(Vector2 originPosition)
    {
        if (!IsValid()) return;
        Sequence?.Kill(false);
        Sequence = null;
        RectTransform.anchoredPosition = originPosition;
        RectTransform.localScale = Vector3.one;
        CgpTip.alpha = 1f;
        TxtTip.text = string.Empty;
        gameObject.SetActive(false);
    }
}
