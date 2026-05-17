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
        traditionalViewObj.SetActive(true);
        return traditionalView;
    }
}


