using HotUpdate.Interface;
using HotUpdate.UI;
using HotUpdate.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SystemAttributeNode : MonoBehaviour, IAutoBind
{
    private WindowBase audioWindow;
    private bool isAudioWindowOpen;
    private MainMenuForm mainMenu;
    [SerializeField] private Button btnSound;
    [SerializeField] private TextMeshProUGUI txtDate;

    public void Init(MainMenuForm mainMenuVar)
    {
        mainMenu = mainMenuVar;
        btnSound.onClick.AddListener(ToggleAudioWindow);
        txtDate.SetText(CurrentTimeUtility.GetCurrentDateString());
    }

    private void ToggleAudioWindow()
    {
        isAudioWindowOpen = !isAudioWindowOpen;
        if (isAudioWindowOpen)
        {
            audioWindow = mainMenu.OpenWindow(AddressableKeys.AudioWindow_Prefab);
        }
        else if (audioWindow != null)
        {
            audioWindow.CloseWindow();
        }
    }
}

