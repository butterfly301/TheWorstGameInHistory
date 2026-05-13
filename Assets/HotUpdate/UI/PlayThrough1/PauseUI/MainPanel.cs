using System.Collections;
using System.Collections.Generic;
using HotUpdate.Interface;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : MonoBehaviour,IAutoBind
{
    [SerializeField]private Button returnToGame;
    [SerializeField]private Button option;
    [SerializeField]private Button credits;
    [SerializeField]private Button backToDesktop;
    
    private PauseForm m_PauseForm;

    public void Init(PauseForm varPauseForm)
    {
        m_PauseForm = varPauseForm;
        returnToGame.onClick.AddListener(m_PauseForm.ClosePauseGame);
        option.onClick.AddListener(() => m_PauseForm.SwitchPanel(1));
        credits.onClick.AddListener(() => m_PauseForm.SwitchPanel(2));
        backToDesktop.onClick.AddListener(m_PauseForm.BackToDesktop);
    }
}
