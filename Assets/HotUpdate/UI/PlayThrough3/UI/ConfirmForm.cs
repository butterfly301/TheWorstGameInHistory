using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using HotUpdate.Interface;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmForm : MonoBehaviour, IAutoBind
{
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private float moveDuration = 0.3f;
    [SerializeField] private float windowOffsetY = 80f;
    [SerializeField] private float buttonOffsetY = 50f;
    [SerializeField] private float buttonMoveDuration = 0.22f;
    [SerializeField] private float confirmButtonDelay = 0.06f;
    [SerializeField] private float cancelButtonDelay = 0.12f;
    [SerializeField] private CanvasGroup cgpBg;
    [SerializeField] private CanvasGroup cgpWindow;
    [SerializeField] private TextMeshProUGUI txtTitle;
    [SerializeField] private TextMeshProUGUI txtContent;
    [SerializeField] private Button btnConfirm;
    [SerializeField] private Button btnCancel;

    private RectTransform windowRectTransform;
    private RectTransform confirmButtonRectTransform;
    private RectTransform cancelButtonRectTransform;
    private CanvasGroup confirmButtonCanvasGroup;
    private CanvasGroup cancelButtonCanvasGroup;
    private Vector2 windowTargetPosition;
    private Vector2 confirmButtonTargetPosition;
    private Vector2 cancelButtonTargetPosition;
    private Sequence showSequence;
    private bool hasCachedTargetPositions;

    public void Init()
    {
        CacheTargetPositions();
        gameObject.SetActive(false);
    }

    public void Open(ConfirmWindowData data)
    {
        txtTitle.text = data.title;
        txtContent.text = data.content;
        btnConfirm.onClick.RemoveAllListeners();
        btnCancel.onClick.RemoveAllListeners();
        btnConfirm.onClick.AddListener(() =>
        {
            data.onConfirm?.Invoke();
            Hide();
        });
        btnCancel.onClick.AddListener(() =>
        {
            data.onCancel?.Invoke();
            Hide();
        });
        Show();
    }

    public void Show()
    {
        CacheTargetPositions();
        showSequence?.Kill();

        gameObject.SetActive(true);
        cgpBg.alpha = 0f;
        cgpBg.blocksRaycasts = true;
        cgpWindow.alpha = 0f;
        cgpWindow.blocksRaycasts = false;
        
        confirmButtonCanvasGroup.alpha = 0f;
        cancelButtonCanvasGroup.alpha = 0f;

        windowRectTransform.anchoredPosition = windowTargetPosition + Vector2.down * windowOffsetY;
        confirmButtonRectTransform.anchoredPosition = confirmButtonTargetPosition + Vector2.down * buttonOffsetY;
        cancelButtonRectTransform.anchoredPosition = cancelButtonTargetPosition + Vector2.down * buttonOffsetY;

        showSequence = DOTween.Sequence()
            .Join(cgpBg.DOFade(1f, fadeDuration))
            .Join(cgpWindow.DOFade(1f, fadeDuration))
            .Join(windowRectTransform.DOAnchorPos(windowTargetPosition, moveDuration).SetEase(Ease.OutCubic))
            .Insert(confirmButtonDelay, confirmButtonRectTransform.DOAnchorPos(confirmButtonTargetPosition, buttonMoveDuration).SetEase(Ease.OutCubic))
            .Insert(confirmButtonDelay, confirmButtonCanvasGroup.DOFade(1f, buttonMoveDuration))
            .Insert(cancelButtonDelay, cancelButtonRectTransform.DOAnchorPos(cancelButtonTargetPosition, buttonMoveDuration).SetEase(Ease.OutCubic))
            .Insert(cancelButtonDelay, cancelButtonCanvasGroup.DOFade(1f, buttonMoveDuration))
            .OnComplete(() =>
            {
                cgpWindow.blocksRaycasts = true;
            });
    }

    public void Hide()
    {
        showSequence?.Kill();
        cgpWindow.blocksRaycasts = false;
        cgpBg.blocksRaycasts = false;
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        windowRectTransform = cgpWindow.transform as RectTransform;
        confirmButtonRectTransform = btnConfirm.transform as RectTransform;
        cancelButtonRectTransform = btnCancel.transform as RectTransform;
        confirmButtonCanvasGroup = btnConfirm.GetComponent<CanvasGroup>();
        cancelButtonCanvasGroup = btnCancel.GetComponent<CanvasGroup>();
    }

    private void OnDestroy()
    {
        showSequence?.Kill();
    }

    private void CacheTargetPositions()
    {
        if (hasCachedTargetPositions || windowRectTransform == null || confirmButtonRectTransform == null || cancelButtonRectTransform == null)
        {
            return;
        }

        windowTargetPosition = windowRectTransform.anchoredPosition;
        confirmButtonTargetPosition = confirmButtonRectTransform.anchoredPosition;
        cancelButtonTargetPosition = cancelButtonRectTransform.anchoredPosition;
        hasCachedTargetPositions = true;
    }
}

public struct ConfirmWindowData
{
    public string title;
    public string content;
    public Action onConfirm;
    public Action onCancel;
}
