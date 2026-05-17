using UnityEngine;
using HotUpdate.Manager;
using HotUpdate.Utility;
using Object = UnityEngine.Object;

public class PopUpUI
{
    private readonly Transform parentTransform;
    private GameObject popUpFormPrefab;
    private PopUpForm popUpForm;

    public PopUpUI(Transform parent)
    {
        parentTransform = parent;
    }

    public void Init()
    {
        AddressablesManager.Instance.LoadAssetAsync<GameObject>(
            AddressableKeys.GetPrefabs_UI_Playthrough3("PopUpForm"),
            handle =>
            {
                popUpFormPrefab = handle.Result;
                var popUpFormObj = Object.Instantiate(popUpFormPrefab, parentTransform);
                popUpForm = popUpFormObj.GetComponent<PopUpForm>();
                popUpForm.Init();
            }
        );
    }
}
