using HotUpdate.Interface;
using HotUpdate.UI;
using HotUpdate.Utility;
using UnityEngine;
using UnityEngine.UI;

public class BottomMenuNode : MonoBehaviour, IAutoBind
{
    [SerializeField] private Button shutdownButton;
    [SerializeField] private SystemAttributeNode systemAttribute;

    public void Init(MainMenuForm mainMenu)
    {
        shutdownButton = transform.Find("ShutdownButton").GetComponent<Button>();
        shutdownButton.onClick.AddListener(() => mainMenu.OpenWindow(AddressableKeys.QuitWindow_Prefab));
        systemAttribute = transform.Find("SystemAttribute").GetComponent<SystemAttributeNode>();
        systemAttribute.Init(mainMenu);
    }
}

