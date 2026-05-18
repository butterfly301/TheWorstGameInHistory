using System.Collections.Generic;
using HotUpdate.Core;
using HotUpdate.Interface;
using HotUpdate.Manager;
using HotUpdate.Utility;
using QFramework;
using UnityEngine;
using Object = UnityEngine.Object;

public class PopUpForm : MonoBehaviour, IController, ICanSendEvent, IAutoBind
{
    private readonly List<PopUpFormBase> popUpForms = new();
    private int currentPopUpIndex = -1;

    public void Init(IReadOnlyList<string> popUpFormNames)
    {
        ClearRuntimePopUps();

        if (popUpFormNames == null || popUpFormNames.Count == 0)
        {
            this.SendEvent<PopUpSequenceFinishedEvent>();
            return;
        }

        LoadPopUpForms(popUpFormNames, 0);
    }

    public void HandlePopUpClosed(PopUpFormBase closedPopUp)
    {
        closedPopUp?.gameObject.SetActive(false);
        ShowNextPopUp();
    }

    private void ClearRuntimePopUps()
    {
        currentPopUpIndex = -1;

        foreach (var popUpForm in popUpForms)
        {
            if (popUpForm == null)
            {
                continue;
            }

            popUpForm.gameObject.SetActive(false);
            Destroy(popUpForm.gameObject);
        }

        popUpForms.Clear();
    }

    private void LoadPopUpForms(IReadOnlyList<string> popUpFormNames, int index)
    {
        if (index >= popUpFormNames.Count)
        {
            if (popUpForms.Count > 0)
            {
                ShowNextPopUp();
            }

            return;
        }

        var popUpFormName = popUpFormNames[index];
        var address = AddressableKeys.GetPrefabs_UI_Playthrough3_PopUpForms(popUpFormName);
        AddressablesManager.Instance.LoadAssetAsync<GameObject>(address, handle =>
        {
            var popUpFormObj = Instantiate(handle.Result, transform);
            popUpFormObj.SetActive(false);

            var popUpFormBase = popUpFormObj.GetComponent<PopUpFormBase>();
            if (popUpFormBase != null)
            {
                popUpFormBase.Init(this);
                popUpForms.Add(popUpFormBase);
            }

            LoadPopUpForms(popUpFormNames, index + 1);
        });
    }

    private void ShowNextPopUp()
    {
        currentPopUpIndex++;

        if (currentPopUpIndex >= popUpForms.Count)
        {
            currentPopUpIndex = popUpForms.Count;
            this.SendEvent<PopUpSequenceFinishedEvent>();
            return;
        }

        popUpForms[currentPopUpIndex].gameObject.SetActive(true);
    }

    public IArchitecture GetArchitecture()
    {
        return TheWorstGameInHistory.Interface;
    }
}
