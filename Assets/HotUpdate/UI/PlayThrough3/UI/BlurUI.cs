using UnityEngine;
using HotUpdate.Manager;
using HotUpdate.Utility;
using Object = UnityEngine.Object;

public class BlurUI
{
    private readonly Transform parentTransform;
    private GameObject blurFormPrefab;
    private BlurForm blurForm;

    public BlurUI(Transform parent)
    {
        parentTransform = parent;
    }

    public void Init()
    {
        AddressablesManager.Instance.LoadAssetAsync<GameObject>(
            AddressableKeys.BlurForm_Prefab,
            handle =>
            {
                blurFormPrefab = handle.Result;
                var blurFormObj = Object.Instantiate(blurFormPrefab, parentTransform);
                blurForm = blurFormObj.GetComponent<BlurForm>();
                blurForm.Init();
            }
        );
    }

    public void AdjustBlurStrength(float changeValue, float duration = 0.5f)
    {
        blurForm?.AdjustBlurStrength(changeValue, duration);
    }
}