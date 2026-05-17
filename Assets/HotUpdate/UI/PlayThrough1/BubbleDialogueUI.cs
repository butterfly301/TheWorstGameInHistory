using HotUpdate.Dialogue.View;
using HotUpdate.Manager;
using HotUpdate.Utility;
using UnityEngine;

public class BubbleDialogueUI
{
    private Transform trans;
    private GameObject bubbleViewPrefab;
    private GameObject bubbleViewObj;
    private DialogueViewBase bubbleView;
    private GameObject choiceButtonPrefab;

    public BubbleDialogueUI(Transform layerTrans)
    {
        trans = layerTrans;
    }

    public void Init()
    {
        LoadChoiceButtonPrefab();
        LoadBubbleView();
    }

    private void LoadChoiceButtonPrefab()
    {
        AddressablesManager.Instance.LoadAssetAsync<GameObject>(
            AddressableKeys.Choice_Prefab,
            handle => choiceButtonPrefab = handle.Result
        );
    }

    private void LoadBubbleView()
    {
        AddressablesManager.Instance.LoadAssetAsync<GameObject>(
            AddressableKeys.BubbleView_Prefab,
            handle =>
            {
                bubbleViewPrefab = handle.Result;
                bubbleViewObj = Object.Instantiate(bubbleViewPrefab, trans);
                bubbleView = bubbleViewObj.GetComponent<DialogueViewBase>();
                bubbleView.SetChoiceButtonPrefab(choiceButtonPrefab);
                bubbleView.Init();
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


