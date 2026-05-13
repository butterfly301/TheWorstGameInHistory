using HotUpdate.Interface;
using HotUpdate.UI;
using HotUpdate.UI.MainMenuUI;
using HotUpdate.Utility;
using UnityEngine;
using UnityEngine.Localization.Components;

public class ConfirmCrackWindow : ConfirmWindow,IAutoBind
{
    private int count;
    [SerializeField]private LocalizeStringEvent text;

    public override void Init(MainMenu mainMenuVar)
    {
        base.Init(mainMenuVar);
        confirm.onClick.AddListener(OnConfirmButtonClick);
        text.SetEntry(LocalizationKeys.ConfirmCrackText1);
    }

    private void OnConfirmButtonClick()
    {
        mainMenu.OpenWindow(AddressableKeys.Prefabs.UI.MainMenu.Window.WindowName.AdWindow);
        count++;
        switch (count)
        {
            case 1:
                text.SetEntry(LocalizationKeys.ConfirmCrackText2);
                break;
            case 2:
                text.SetEntry(LocalizationKeys.ConfirmCrackText3);
                break;
            case 3:
                gameObject.SetActive(false);
                break;
        }
    }
}