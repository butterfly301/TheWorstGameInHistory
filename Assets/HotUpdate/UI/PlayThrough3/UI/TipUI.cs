using HotUpdate.Manager;
using HotUpdate.Utility;
using UnityEngine;
using Object = UnityEngine.Object;

public class TipUI
{
    private readonly Transform parentTransform;
    private GameObject tipFormPrefab;
    private GameObject tipFormObj;
    private TipForm tipForm;

    public TipUI(Transform parent)
    {
        parentTransform = parent;
    }

    public void Init()
    {
        if (tipFormObj != null) return;

        AddressablesManager.Instance.LoadAssetAsync<GameObject>(
            AddressableKeys.TipForm_Prefab,
            handle =>
            {
                tipFormPrefab = handle.Result;
                tipFormObj = Object.Instantiate(tipFormPrefab, parentTransform);
                tipForm = tipFormObj.GetComponent<TipForm>();
                tipForm?.Init();
                Close();
            }
        );
    }

    public void Open()
    {
        tipFormObj?.SetActive(true);
    }

    public void Show(string content)
    {
        if (tipForm == null) return;
        Open();
        tipForm.Show(content);
    }

    public void Close()
    {
        tipFormObj?.SetActive(false);
    }
}
