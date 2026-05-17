using HotUpdate.Interface;
using HotUpdate.UI;
using HotUpdate.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SystemAttribute : MonoBehaviour, IAutoBind
{
    private WindowBase audioWindow;
    private bool isAudioWindowOpen;
    private MainMenu mainMenu;
    [SerializeField] private Button sound;
    [SerializeField] private TextMeshProUGUI date;

public void Init(MainMenu mainMenuVar)
    {
        mainMenu = mainMenuVar;
        sound.onClick.AddListener(ToggleAudioWindow);
        date.SetText(CurrentTimeUtility.GetCurrentDateString());
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
