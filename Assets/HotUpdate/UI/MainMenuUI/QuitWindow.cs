using HotUpdate.UI;
using UnityEngine;
using UnityEngine.UI;

public class QuitWindow : WindowBase
{
    [SerializeField]private Button no;
    [SerializeField]private Button yes;

    public override void Init(MainMenu mainMenuVar)
    {
        base.Init(mainMenuVar);
        yes = transform.Find("Yes").GetComponent<Button>();
        no = transform.Find("No").GetComponent<Button>();
        yes.onClick.AddListener(() => { Application.Quit(); });
        no.onClick.AddListener(() => { gameObject.SetActive(false); });
    }
}