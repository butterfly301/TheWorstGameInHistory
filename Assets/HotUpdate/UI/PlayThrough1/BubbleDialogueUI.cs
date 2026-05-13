using System.Collections;
using System.Collections.Generic;
using HotUpdate.Dialogue.View;
using HotUpdate.Manager;
using HotUpdate.Utility;
using UnityEngine;

public class BubbleDialogueUI
{
    private Transform trans;
    // 对话视图对象
    private GameObject bubbleViewPrefab;
    private GameObject bubbleViewObj;
    private DialogueViewBase bubbleView;

    public BubbleDialogueUI(Transform layerTrans)
    {
        trans = layerTrans;
    }
    public void Init(GameObject varChoiceButtonPrefab)
    {
        LoadBubbleView(varChoiceButtonPrefab);
    }

    private void LoadBubbleView(GameObject varChoiceButtonPrefab)
    {
        AddressablesManager.Instance.LoadAssetAsync<GameObject>(
            AddressableKeys.Prefabs.UI.Playthrough1.DialogueViewType.BubbleView_Prefab,
            handle =>
            {
                bubbleViewPrefab = handle.Result;
                bubbleViewObj = Object.Instantiate(bubbleViewPrefab, trans);
                bubbleView = bubbleViewObj.GetComponent<DialogueViewBase>();
                bubbleView.Init(varChoiceButtonPrefab);
                bubbleViewObj.SetActive(false);
            }
        );
    }

    public DialogueViewBase GetDialogueView()
    {
        bubbleViewObj.SetActive(true);
        return bubbleView;
    }
}
