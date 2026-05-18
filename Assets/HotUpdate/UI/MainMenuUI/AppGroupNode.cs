using System.Collections.Generic;
using System.IO;
using HotUpdate.Browser;
using HotUpdate.Core;
using HotUpdate.Data.Commands;
using HotUpdate.Data.Model;
using HotUpdate.SceneLoad.Commands;
using HotUpdate.UI;
using HotUpdate.Utility;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

public class AppGroupNode : MonoBehaviour, IController
{
    private Button[] buttons;
    private FileDragAndDrop fileDragAndDrop;
    private GameData gameData;
    private MainMenuForm mainMenu;

    public IArchitecture GetArchitecture()
    {
        return TheWorstGameInHistory.Interface;
    }

    public void Init(MainMenuForm mainMenuVar)
    {
        mainMenu = mainMenuVar;
        gameData = mainMenu.GameData;

        fileDragAndDrop = gameObject.AddComponent<FileDragAndDrop>();
        fileDragAndDrop.OnFilesDropped += OnFilesDropped;

        buttons = GetComponentsInChildren<Button>();
        buttons[0].GetComponent<TheLastKnight1789AppButton>()?.Init(mainMenu);
        buttons[1].onClick.AddListener(() => mainMenu.OpenWindow(AddressableKeys.SettingWindow_Prefab, _ => { }));
        buttons[2].onClick.AddListener(() => mainMenu.OpenWindow(AddressableKeys.CreditWindow_Prefab, _ => { }));
        buttons[3].onClick.AddListener(() => mainMenu.OpenWindow(AddressableKeys.FutureUpdatePlanWindow_Prefab, _ => { }));
        buttons[4].onClick.AddListener(() => mainMenu.OpenWindow(AddressableKeys.RubbishCanWindow_Prefab, _ => { }));
        buttons[5].onClick.AddListener(() => this.SendCommand(new OpenURLCommand(URLKeys.WorldBrowser)));

        buttons[6].gameObject.SetActive(gameData.software.Contains(SoftwareName.IceBreaker));
        buttons[6].onClick.AddListener(() => mainMenu.OpenWindow(AddressableKeys.IceBreakerPreWindow_Prefab, _ => { }));

        buttons[7].gameObject.SetActive(gameData.software.Contains(SoftwareName.TLH_1793));
        buttons[7].onClick.AddListener(() => this.SendCommand(new LoadSceneCommand(AddressableKeys.OpenVideo1_Unity, false)));

        if (!Application.isEditor)
        {
            buttons[8].gameObject.SetActive(false);
        }
        else
        {
            buttons[8].onClick.AddListener(() => this.SendCommand(new LoadSceneCommand(AddressableKeys.Login_Unity, false)));
        }
    }

    private void OnFilesDropped(List<string> files)
    {
        foreach (var file in files)
        {
            var fileName = Path.GetFileName(file);
            switch (fileName)
            {
                case "矩阵破冰者软件安装包.png":
                case "IceBreakerSoftwareDownloadPackage.png":
                    mainMenu.OpenWindow(AddressableKeys.DownloadIcebreakerWindow_Prefab, _ => { });
                    return;

                case "最后的勇者2游戏包体.png":
                case "TheLastHero17893GameDownloadPackage.png":
                    mainMenu.OpenWindow(AddressableKeys.DownloadTLHWindow_Prefab, _ => { });
                    return;
            }
        }
    }

    public void EnableSoftware(SoftwareName softwareName)
    {
        var index = GetSoftwareIndex(softwareName);
        buttons[index].gameObject.SetActive(true);
        this.SendCommand(new AddSoftwareCommand(softwareName));
    }

    private int GetSoftwareIndex(SoftwareName softwareName)
    {
        switch (softwareName)
        {
            case SoftwareName.IceBreaker:
                return 6;

            case SoftwareName.TLH_1793:
                return 7;

            default:
                return 0;
        }
    }
}


