using HotUpdate.UI;
using HotUpdate.Utility;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SettingWindow : WindowBase
{
    [SerializeField]private Button graphics;
    [SerializeField]private Button language;
    [SerializeField]private CanvasGroup graphicsPanel;
    [SerializeField]private CanvasGroup languagePanel;

    private Button[] buttons;
    private CanvasGroup[] panels;

    public override void Init(MainMenu mainMenuVar)
    {
        base.Init(mainMenuVar);

        // Hide all other windows except the SettingWindow itself
        if (mainMenuVar?.Windows != null)
            foreach (var kv in mainMenuVar.Windows)
            {
                if (kv.Key == AddressableKeys.Prefabs.UI.MainMenu.Window.WindowName.SettingWindow) continue;
                if (kv.Value != null)
                    kv.Value.GetComponent<WindowBase>().CloseWindow();
            }
        
        buttons=new Button[] { graphics, language };
        for (var i = 0; i < buttons.Length; i++)
        {
            var index = i;
            buttons[i].onClick.AddListener(() => SwitchPanel(index));
        }
        
        panels=new CanvasGroup[] { graphicsPanel, languagePanel };
        for (var j = 0; j < panels.Length; j++)
        {
            panels[j].GetComponent<OptionPanelChildren>().Init();
        }

        buttons[0].onClick.Invoke();
    }

    private void SwitchPanel(int index)
    {
        foreach (var panel in panels) panel.gameObject.SetActive(false);
        panels[index].gameObject.SetActive(true);
    }
}