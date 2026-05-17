using HotUpdate.Dialogue.View;
using HotUpdate.Manager;
using HotUpdate.Utility;
using UnityEngine;

namespace HotUpdate.UI
{
    /// <summary>
    /// 对话UI管理器
    /// </summary>
    public class NarratorDialogueUI
    {
        private Transform trans;
        private GameObject narratorViewPrefab;
        private GameObject narratorViewObj;
        private DialogueViewBase narratorView;
        private GameObject choiceButtonPrefab;

public NarratorDialogueUI(Transform layerTrans)
        {
            trans = layerTrans;
        }

public void Init()
        {
            LoadChoiceButtonPrefab();
            LoadNarratorView();
        }

private void LoadChoiceButtonPrefab()
        {
            AddressablesManager.Instance.LoadAssetAsync<GameObject>(
                AddressableKeys.Choice_Prefab,
                handle => choiceButtonPrefab = handle.Result
            );
        }

private void LoadNarratorView()
        {
            AddressablesManager.Instance.LoadAssetAsync<GameObject>(
                AddressableKeys.NarratorView_Prefab,
                handle =>
                {
                    narratorViewPrefab = handle.Result;
                    narratorViewObj = Object.Instantiate(narratorViewPrefab, trans);
                    narratorView = narratorViewObj.GetComponent<DialogueViewBase>();
                    narratorView.SetChoiceButtonPrefab(choiceButtonPrefab);
                    narratorView.Init();
                    narratorViewObj.SetActive(false);
                }
            );
        }

public DialogueViewBase GetDialogueView()
        {
            narratorViewObj.SetActive(true);
            return narratorView;
        }
    }
}

