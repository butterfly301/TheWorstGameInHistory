using HotUpdate.Interface;
using UnityEngine;
using UnityEngine.UI;

public class PopUpFormBase : MonoBehaviour, IAutoBind
{
    [SerializeField] protected Button btnClose;
    private PopUpForm owner;

    public virtual void Init(PopUpForm popUpForm)
    {
        owner = popUpForm;
        btnClose.onClick.RemoveListener(OnBtnCloseClick);
        btnClose.onClick.AddListener(OnBtnCloseClick);
    }

    private void OnBtnCloseClick()
    {
        owner?.HandlePopUpClosed(this);
    }
}
