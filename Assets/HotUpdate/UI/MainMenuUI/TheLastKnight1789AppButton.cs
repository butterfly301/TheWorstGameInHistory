using HotUpdate.Audio.Commands;
using HotUpdate.Core;
using HotUpdate.SceneLoad.Commands;
using HotUpdate.UI;
using HotUpdate.Utility;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

public class TheLastKnight1789AppButton : MonoBehaviour, IController
{
    private Button button;
    private int playThroughCount;

    public IArchitecture GetArchitecture()
    {
        return TheWorstGameInHistory.Interface;
    }

    public void Init(MainMenu mainMenu)
    {
        button = GetComponent<Button>();
        playThroughCount = mainMenu.GameData.playThrough;

        if (playThroughCount == 0)
        {
            button.onClick.AddListener(EnterGame);
        }
        else
        {
            button.onClick.AddListener(() => mainMenu.OpenWindow(AddressableKeys.ConfirmCrackWindow_Prefab));
        }
    }

    public void EnterGame()
    {
        this.SendCommand(new PauseMusicCommand());
        this.SendCommand(new LoadSceneCommand(AddressableKeys.OpenVideo1_Unity, false));
    }
}
