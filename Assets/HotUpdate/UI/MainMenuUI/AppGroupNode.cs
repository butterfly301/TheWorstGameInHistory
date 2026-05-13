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
    private MainMenu mainMenu;

    public IArchitecture GetArchitecture()
    {
        return TheWorstGameInHistory.Interface;
    }

    public void Init(MainMenu mainMenuVar)
    {
        mainMenu = mainMenuVar;
        gameData = mainMenu.GameData;

        fileDragAndDrop = gameObject.AddComponent<FileDragAndDrop>();
        fileDragAndDrop.OnFilesDropped += OnFilesDropped;

        buttons = GetComponentsInChildren<Button>();
        buttons[0].GetComponent<TheLastKnight1789AppButton>()?.Init(mainMenu);
        buttons[1].onClick.AddListener(() => { mainMenu.OpenWindow(AddressableKeys.Prefabs.UI.MainMenu.Window.WindowName.SettingWindow, _ => { }); }); //设置图标
        buttons[2].onClick.AddListener(() => { mainMenu.OpenWindow(AddressableKeys.Prefabs.UI.MainMenu.Window.WindowName.CreditWindow, _ => { }); }); //制作人员名单
        buttons[3].onClick.AddListener(() =>
        {
            mainMenu.OpenWindow(AddressableKeys.Prefabs.UI.MainMenu.Window.WindowName.FutureUpdatePlanWindow, _ => { });
        }); //后续更新计划
        buttons[4].onClick.AddListener(() =>//回收站
        {
            mainMenu.OpenWindow(AddressableKeys.Prefabs.UI.MainMenu.Window.WindowName.RubbishCanWindow, _ => { });
        }); 
        buttons[5].onClick.AddListener(() => //浏览器
        {
            this.SendCommand(new OpenURLCommand(URLKeys.WorldBrowser));
        });
        
        buttons[6].gameObject.SetActive(gameData.software.Contains(SoftwareName.IceBreaker));
        buttons[6].onClick.AddListener(() => //矩阵破冰者
        {
            mainMenu.OpenWindow(AddressableKeys.Prefabs.UI.MainMenu.Window.WindowName.IceBreakerPreWindow, _ => { });
        });
        
        buttons[7].gameObject.SetActive(gameData.software.Contains(SoftwareName.TLH_1793));
        buttons[7].onClick.AddListener(() => //最后的勇者2
        { this.SendCommand(new LoadSceneCommand(AddressableKeys.Scenes.OpenVideo1_Unity, false)); });

        if (!Application.isEditor)//只在编辑器情况出现
            buttons[8].gameObject.SetActive(false);
        else
            buttons[8].onClick.AddListener(() => //三国：天意侵蚀
                { this.SendCommand(new LoadSceneCommand(AddressableKeys.Scenes.Playthrough3.Login_Unity,false)); });
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
                    mainMenu.OpenWindow(AddressableKeys.Prefabs.UI.MainMenu.Window.WindowName.DownloadIcebreakerWindow, _ => { });
                    return;
                case "最后的勇者2游戏包体.png":
                case "TheLastHero17893GameDownloadPackage.png":
                    mainMenu.OpenWindow(AddressableKeys.Prefabs.UI.MainMenu.Window.WindowName.DownloadTLHWindow, _ => { });
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