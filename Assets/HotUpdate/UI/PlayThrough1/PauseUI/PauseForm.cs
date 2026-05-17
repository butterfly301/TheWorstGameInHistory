using System.Collections.Generic;
using HotUpdate.Core;
using HotUpdate.Interface;
using HotUpdate.SceneLoad.Commands;
using HotUpdate.Utility;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

public class PauseForm : MonoBehaviour, IController,IAutoBind
{
    [SerializeField]private MainPanel mainPanel;
    [SerializeField]private OptionPanel optionPanel;
    [SerializeField]private CreditPanel creditPanel;
    private Transform[] panels;

private void OnEnable()
    {
        if (panels != null)
            SwitchPanel(0);
    }

public IArchitecture GetArchitecture()
    {
        return TheWorstGameInHistory.Interface;
    }

public void Init()
    {
        panels = new Transform[transform.childCount];
        panels = GetAllChildren(transform);
        mainPanel?.Init(this);
        optionPanel?.Init(this); 
        creditPanel?.Init(this);
        SwitchPanel(0);
    }

private Transform[] GetAllChildren(Transform parent)
    {
        var children = new List<Transform>();

for (var i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);
            children.Add(child);
        }

return children.ToArray();
    }

public void SwitchPanel(int index)
    {
        foreach (var panel in panels) panel.gameObject.SetActive(false);
        panels[index].gameObject.SetActive(true);
    }

public void ClosePauseGame()
    {
        WorldManagerBase.Instance.Unpause();
    }

public void BackToDesktop()
    {
        this.SendCommand(new LoadSceneCommand(AddressableKeys.MainMenu_Unity, false));
    }
}
