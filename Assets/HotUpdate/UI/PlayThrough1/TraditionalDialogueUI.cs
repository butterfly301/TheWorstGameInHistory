using HotUpdate.Dialogue.View;
using HotUpdate.Manager;
using HotUpdate.Utility;
using UnityEngine;

public class TraditionalDialogueUI
{
    private Transform trans;
    private GameObject traditionalViewPrefab;
    private GameObject traditionalViewObj;
    private DialogueViewBase traditionalView;
    private GameObject choiceButtonPrefab;

public TraditionalDialogueUI(Transform transform)
    {
        trans = transform;
    }

public void Init()
    {
        if (traditionalViewObj != null) return;

        LoadChoiceButtonPrefab();
        LoadTraditionalView();
    }

private void LoadChoiceButtonPrefab()
    {
        AddressablesManager.Instance.LoadAssetAsync<GameObject>(
            AddressableKeys.Choice_Prefab,
            handle => choiceButtonPrefab = handle.Result
        );
    }

private void LoadTraditionalView()
    {
        AddressablesManager.Instance.LoadAssetAsync<GameObject>(
            AddressableKeys.TraditionalView_Prefab,
            handle =>
            {
                traditionalViewPrefab = handle.Result;
                traditionalViewObj = Object.Instantiate(traditionalViewPrefab, trans);
                traditionalView = traditionalViewObj.GetComponent<DialogueViewBase>();
                traditionalView.SetChoiceButtonPrefab(choiceButtonPrefab);
                traditionalView.Init();
                traditionalViewObj.SetActive(false);
            }
        );
    }

    public DialogueViewBase GetDialogueView()
    {
        Open();
        return traditionalView;
    }

public void Open()
    {
        traditionalViewObj?.SetActive(true);
    }

public void Close()
    {
        traditionalViewObj?.SetActive(false);
    }
}

