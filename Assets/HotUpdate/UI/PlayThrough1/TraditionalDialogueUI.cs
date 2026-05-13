using System.Collections;
using System.Collections.Generic;
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
    public TraditionalDialogueUI(Transform transform)
    {
        trans = transform;
    }
    public void Init(GameObject varChoiceButtonPrefab)
    {
        LoadNarratorView(varChoiceButtonPrefab);
    }

    private void LoadNarratorView(GameObject varChoiceButtonPrefab)
    {
        AddressablesManager.Instance.LoadAssetAsync<GameObject>(
            AddressableKeys.Prefabs.UI.Playthrough1.DialogueViewType.TraditionalView_Prefab,
            handle =>
            {
                traditionalViewPrefab = handle.Result;
                traditionalViewObj = Object.Instantiate(traditionalViewPrefab, trans);
                traditionalView = traditionalViewObj.GetComponent<DialogueViewBase>();
                traditionalView.Init(varChoiceButtonPrefab);
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
