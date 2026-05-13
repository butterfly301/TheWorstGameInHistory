// using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.UI;

public class SPUM_ConvertView : MonoBehaviour
{
    public Button Convert;
    public Button Cancel;
    public Text PrefabVersion;
    public Text SPUM_Version;
    public Text MissingPackageNames;
    public GameObject WarningText;
    public GameObject WarningEyeText;

    private void Start()
    {
        Cancel.onClick.AddListener(() => gameObject.SetActive(false));
    }

    private void OnDisable()
    {
        Convert.onClick.RemoveAllListeners();
    }
}