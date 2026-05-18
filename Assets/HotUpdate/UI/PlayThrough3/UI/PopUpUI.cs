using UnityEngine;
using HotUpdate.Manager;
using HotUpdate.Utility;
using Object = UnityEngine.Object;

public class PopUpUI
{
    private readonly Transform parentTransform;
    private GameObject popUpFormPrefab;
    private GameObject popUpFormObj;
    private PopUpForm popUpForm;

    public PopUpUI(Transform parent)
    {
        parentTransform = parent;
    }

    public void Init()
    {
        if (popUpFormObj != null) return;
        AddressablesManager.Instance.LoadAssetAsync<GameObject>(
            AddressableKeys.PopUpForm_Prefab,
            handle =>
            {
                popUpFormPrefab = handle.Result;
                popUpFormObj = Object.Instantiate(popUpFormPrefab, parentTransform);
                popUpForm = popUpFormObj.GetComponent<PopUpForm>();
                Close();
            }
        );
    }

    public void Init(PopUpData popUpData)
    {
        if (popUpForm == null)  return;
        
        popUpForm.Init(popUpData?.popUpFormNames);
        Open();
    }

    public void Open()
    {
        popUpFormObj?.SetActive(true);
    }

    public void Close()
    {
        popUpFormObj?.SetActive(false);
    }
}
