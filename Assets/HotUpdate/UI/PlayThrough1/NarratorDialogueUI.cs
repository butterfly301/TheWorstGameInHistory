using System.Collections.Generic;
using HotUpdate.Dialogue.View;
using HotUpdate.Enums;
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
        // 对话视图对象
        private GameObject narratorViewPrefab;
        private GameObject narratorViewObj;
        private DialogueViewBase narratorView;

        public NarratorDialogueUI(Transform layerTrans)
        {
            trans = layerTrans;
        }
        public void Init(GameObject varChoiceButtonPrefab)
        {
            LoadNarratorView(varChoiceButtonPrefab);
        }

        private void LoadNarratorView(GameObject varChoiceButtonPrefab)
        {
            AddressablesManager.Instance.LoadAssetAsync<GameObject>(
                AddressableKeys.Prefabs.UI.Playthrough1.DialogueViewType.NarratorView_Prefab,
                handle =>
                {
                    narratorViewPrefab = handle.Result;
                    narratorViewObj = Object.Instantiate(narratorViewPrefab, trans);
                    narratorView = narratorViewObj.GetComponent<DialogueViewBase>();
                    narratorView.Init(varChoiceButtonPrefab);
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
