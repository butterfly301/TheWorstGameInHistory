using UnityEngine;
using HotUpdate.Manager;
using HotUpdate.Utility;
using Object = UnityEngine.Object;

public class ConfirmUI
{
    private readonly Transform parentTransform;
    private GameObject confirmFormPrefab;
    private GameObject confirmFormObj;
    private ConfirmForm confirmForm;

    public ConfirmUI(Transform parent)
    {
        parentTransform = parent;
    }

    public void Init()
    {
        if (confirmFormObj != null) return;

        AddressablesManager.Instance.LoadAssetAsync<GameObject>(
            AddressableKeys.ConfirmForm_Prefab,
            handle =>
            {
                confirmFormPrefab = handle.Result;
                confirmFormObj = Object.Instantiate(confirmFormPrefab, parentTransform);
                confirmForm = confirmFormObj.GetComponent<ConfirmForm>();
                confirmForm.Init();
                Close();
            }
        );
    }

    public void Open()
    {
        confirmFormObj?.SetActive(true);
    }

    public void Open(ConfirmWindowData data)
    {
        confirmForm?.Open(data);
    }

    public void Close()
    {
        confirmForm?.Hide();
    }
}
