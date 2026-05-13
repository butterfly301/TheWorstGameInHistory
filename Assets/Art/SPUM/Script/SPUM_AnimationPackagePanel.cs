using UnityEngine;
using UnityEngine.UI;

public class SPUM_AnimationPackagePanel : MonoBehaviour
{
    public Text SelectedStateText;
    public Button CloseButton;
    public Transform parent;
    public SPUM_PackagePanel spumPackagePanel;
#if UNITY_EDITOR

    // 모든 패키지 생성하기
    public void CreateSpumAnimationPackagePanel(SPUM_AnimationManager manager)
    {
        var SelectedType = manager.SelectedType;
        var spumPackages = manager.unit.spumPackages;
        foreach (Transform element in parent) Destroy(element.gameObject);

        foreach (var Package in spumPackages)
        {
            var PackagePanel = Instantiate(spumPackagePanel, parent);
            PackagePanel.CreatePackageUI(Package, manager);
        }
    }
#endif
}