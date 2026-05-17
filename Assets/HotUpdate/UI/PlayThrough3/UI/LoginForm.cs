using HotUpdate.Audio.Commands;
using HotUpdate.Core;
using HotUpdate.Interface;
using HotUpdate.SceneLoad.Commands;
using HotUpdate.SceneLoad.System;
using HotUpdate.Utility;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LoginForm : MonoBehaviour, IAutoBind, IController
{
    private const string StringTableName = "String Table";

private bool isIn;
    private int currentServerIndex;

private static readonly int Click = Animator.StringToHash("click");

[SerializeField] private Animator canvas;
    [SerializeField] private Button start;
    [SerializeField] private Button select;
    [SerializeField] private Button left;
    [SerializeField] private Button right;
    [SerializeField] private Button quit;
    [SerializeField] private TextMeshProUGUI serverName;

private readonly string[] serverNames =
    {
        "XuzhouCity",
        "JingXiangNineCommanderies",
        "YanzhouCity",
        "ChenliuGrandHotel",
        "MangdangMountain"
    };

public void Init()
    {
        isIn = false;
        currentServerIndex = 0;

start.onClick.AddListener(EnterGame);
        left.onClick.AddListener(SelectPreviousServer);
        right.onClick.AddListener(SelectNextServer);
        quit.onClick.AddListener(QuitGame);
        LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChanged;

RefreshServerName();
    }

private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;
    }

private void Update()
    {
        if (Input.anyKeyDown)
        {
            EnterRealLogin();
        }
    }

private void EnterRealLogin()
    {
        if (!isIn)
        {
            canvas.SetTrigger(Click);
            isIn = true;
        }
    }

private void EnterGame()
    {
        this.SendCommand(new LoadSceneCommand(AddressableKeys.Lobby_Unity, true, LoadingScreenType.PlayThrough3));
    }

private void QuitGame()
    {
        this.SendCommand(new LoadSceneCommand(AddressableKeys.MainMenu_Unity, false));
    }

private void SelectPreviousServer()
    {
        if (serverNames.Length == 0)
        {
            return;
        }

currentServerIndex = (currentServerIndex - 1 + serverNames.Length) % serverNames.Length;
        RefreshServerName();
    }

private void SelectNextServer()
    {
        if (serverNames.Length == 0)
        {
            return;
        }

currentServerIndex = (currentServerIndex + 1) % serverNames.Length;
        RefreshServerName();
    }

private void RefreshServerName()
    {
        if (serverName == null || serverNames.Length == 0)
        {
            return;
        }

var stringTable = LocalizationSettings.StringDatabase.GetTable(StringTableName, LocalizationSettings.SelectedLocale);
        if (stringTable == null)
        {
            serverName.text = serverNames[currentServerIndex];
            return;
        }

var entry = stringTable.GetEntry(serverNames[currentServerIndex]);
        serverName.text = entry == null ? serverNames[currentServerIndex] : entry.GetLocalizedString();
    }

private void OnSelectedLocaleChanged(UnityEngine.Localization.Locale locale)
    {
        RefreshServerName();
    }

public IArchitecture GetArchitecture()
    {
        return TheWorstGameInHistory.Interface;
    }
}

