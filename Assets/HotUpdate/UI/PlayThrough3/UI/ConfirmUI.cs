using UnityEngine;
using HotUpdate.Manager;
using HotUpdate.Utility;
using Object = UnityEngine.Object;

public class ConfirmUI
{
    private readonly Transform parentTransform;
    private GameObject confirmFormPrefab;
    private ConfirmForm confirmForm;

    public ConfirmUI(Transform parent)
    {
        parentTransform = parent;
    }

    public void Init()
    {
        AddressablesManager.Instance.LoadAssetAsync<GameObject>(
            AddressableKeys.ConfirmForm_Prefab,
            handle =>
            {
                confirmFormPrefab = handle.Result;
                var confirmFormObj = Object.Instantiate(confirmFormPrefab, parentTransform);
                confirmForm = confirmFormObj.GetComponent<ConfirmForm>();
                confirmForm.Init();
            }
        );
    }

    public void Open(ConfirmWindowData data)
    {
        confirmForm?.Open(data);
    }
}
