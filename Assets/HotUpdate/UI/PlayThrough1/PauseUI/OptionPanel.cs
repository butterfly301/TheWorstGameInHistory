using HotUpdate.Interface;
using UnityEngine;
using UnityEngine.UI;

public class OptionPanel : MonoBehaviour,IAutoBind
{
    private Button[] buttons;
    private CanvasGroup[] panels;

private PauseForm m_PauseForm;
    [SerializeField]private Button quit;

public void Init(PauseForm varPauseForm)
    {
        buttons = new Button[3];
        buttons[0] = transform.Find("Bag").GetComponent<Button>();
        buttons[1] = transform.Find("Graphics").GetComponent<Button>();
        buttons[2] = transform.Find("Audio").GetComponent<Button>();
        panels = new CanvasGroup[3];
        panels[0] = transform.Find("BagPanel").GetComponent<CanvasGroup>();
        panels[1] = transform.Find("GraphicsPanel").GetComponent<CanvasGroup>();
        panels[2] = transform.Find("AudioPanel").GetComponent<CanvasGroup>();

// 初始化每个按钮
        for (var i = 0; i < buttons.Length; i++)
        {
            var index = i;
            buttons[i].onClick.AddListener(() => SwitchPanel(index));
        }

for (var j = 0; j < panels.Length; j++) panels[j].GetComponent<OptionPanelChildren>()?.Init();

//设置退出按钮
        m_PauseForm = varPauseForm;
        quit = transform.Find("Quit").GetComponent<Button>();
        quit.onClick.AddListener(() => m_PauseForm.SwitchPanel(0));

buttons[0].onClick.Invoke();
    }

public void SwitchPanel(int index)
    {
        foreach (var panel in panels) panel.gameObject.SetActive(false);
        panels[index].gameObject.SetActive(true);
    }
}