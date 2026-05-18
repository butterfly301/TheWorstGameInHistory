using System.Collections.Generic;
using DG.Tweening;
using HotUpdate.Interface;
using UnityEngine;

public class TipForm : MonoBehaviour, IAutoBind
{
    [SerializeField] private TipItemNode tipItemNode;
    [SerializeField] private int maxTipCount = 5;
    [SerializeField] private float floatDistance = 200f;
    [SerializeField] private float floatDuration = 1f;
    [SerializeField] private float fadeDuration = 0.15f;
    [SerializeField] private float stackSpacing = 50f;
    [SerializeField] private float bornScale = 1.3f;
    [SerializeField] private float bornDuration = 0.3f;

    private readonly Queue<TipItemNode> activeItems = new();
    private readonly Stack<TipItemNode> pooledItems = new();
    private RectTransform rootRectTransform;
    private Vector2 originPosition;
    private bool initialized;

    public void Init()
    {
        if (initialized) return;

        rootRectTransform = transform as RectTransform;
        if (tipItemNode == null || !tipItemNode.IsValid()) return;

        originPosition = tipItemNode.RectTransform.anchoredPosition;
        tipItemNode.gameObject.SetActive(false);
        initialized = true;
    }

    public void Show(string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return;
        if (!initialized) Init();
        if (!initialized) return;

        gameObject.SetActive(true);

        if (activeItems.Count >= maxTipCount) Recycle(activeItems.Dequeue());

        var item = GetItem();
        item.SetContent(content);
        item.ResetState(originPosition, bornScale);
        activeItems.Enqueue(item);
        RefreshPositions();
    }

    private TipItemNode GetItem()
    {
        if (pooledItems.Count > 0) return pooledItems.Pop();

        return Instantiate(tipItemNode, rootRectTransform);
    }

    private void RefreshPositions()
    {
        var items = activeItems.ToArray();
        for (var i = 0; i < items.Length; i++)
        {
            var item = items[i];
            var targetY = originPosition.y + floatDistance + (items.Length - 1 - i) * stackSpacing;
            var targetPosition = new Vector2(originPosition.x, targetY);
            item.Play(targetPosition, floatDuration, bornDuration, fadeDuration, () => HandleItemFinished(item));
        }
    }

    private void HandleItemFinished(TipItemNode finishedItem)
    {
        if (activeItems.Count == 0) return;

        var items = activeItems.ToArray();
        activeItems.Clear();

        foreach (var item in items)
        {
            if (item == finishedItem)
            {
                Recycle(item);
                continue;
            }

            activeItems.Enqueue(item);
        }

        if (activeItems.Count == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        RefreshPositions();
    }

    private void Recycle(TipItemNode item)
    {
        item.Clear(originPosition);
        pooledItems.Push(item);
    }

    private void OnDestroy()
    {
        foreach (var item in activeItems) item.Sequence?.Kill(false);
        foreach (var item in pooledItems) item.Sequence?.Kill(false);
    }
}
