using UnityEngine;
using UnityEngine.UI;

public class CreditPanel : MonoBehaviour
{
    private PauseForm m_PauseForm;
    private Button quitButton;

    public void Init(PauseForm varPauseForm)
    {
        m_PauseForm = varPauseForm;
        quitButton = transform.Find("Quit").GetComponent<Button>();
        quitButton.onClick.AddListener(() => m_PauseForm.SwitchPanel(0));
    }
}