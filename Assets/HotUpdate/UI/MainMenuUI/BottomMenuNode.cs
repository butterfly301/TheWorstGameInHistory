using HotUpdate.Interface;
using HotUpdate.UI;
using HotUpdate.Utility;
using UnityEngine;
using UnityEngine.UI;

public class BottomMenuNode : MonoBehaviour, IAutoBind
{
    [SerializeField] private Button shutdownButton;
    [SerializeField] private SystemAttribute systemAttribute;

public void Init(MainMenu mainMenu)
    {
        shutdownButton = transform.Find("ShutdownButton").GetComponent<Button>();
        shutdownButton.onClick.AddListener(() => mainMenu.OpenWindow(AddressableKeys.QuitWindow_Prefab));
        systemAttribute = transform.Find("SystemAttribute").GetComponent<SystemAttribute>();
        systemAttribute.Init(mainMenu);
    }
}
