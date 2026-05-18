using UnityEngine;
using HotUpdate.Manager;
using HotUpdate.Utility;
using Object = UnityEngine.Object;

public class BlurUI
{
    private readonly Transform parentTransform;
    private GameObject blurFormPrefab;
    private GameObject blurFormObj;
    private BlurForm blurForm;

    public BlurUI(Transform parent)
    {
        parentTransform = parent;
    }

    public void Init()
    {
        if (blurFormObj != null)     return;

        AddressablesManager.Instance.LoadAssetAsync<GameObject>(
            AddressableKeys.BlurForm_Prefab,
            handle =>
            {
                blurFormPrefab = handle.Result;
                blurFormObj = Object.Instantiate(blurFormPrefab, parentTransform);
                blurForm = blurFormObj.GetComponent<BlurForm>();
                blurForm.Init();
                Close();
            }
        );
    }

    public void Open()
    {
        blurFormObj?.SetActive(true);
    }

    public void Close()
    {
        blurFormObj?.SetActive(false);
    }

    public void AdjustBlurStrength(float changeValue, float duration = 0.5f)
    {
        blurForm?.AdjustBlurStrength(changeValue, duration);
    }
}
