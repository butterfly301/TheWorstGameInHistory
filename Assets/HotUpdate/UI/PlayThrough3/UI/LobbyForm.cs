using System.Collections;
using System.Collections.Generic;
using HotUpdate.Interface;
using UnityEngine;
using UnityEngine.UI;

public class LobbyForm : MonoBehaviour,IAutoBind
{
    [SerializeField]private Button btnFriend;
    [SerializeField]private Button btnSignIn;
    public void Init()
    {
        btnFriend.onClick.AddListener(OnBtnFriendClick);
        btnSignIn.onClick.AddListener(OnBtnSignInClick);
    }

    private void OnBtnFriendClick()
    {
        UIManager3.Instance.Tip?.Show("好友系统正在开发中，敬请期待！");
    }
    private void OnBtnSignInClick()
    {
        ConfirmWindowData data = new ConfirmWindowData
        {
            title = "签到",
            content = "是否要进行签到？",
            confirmData = new confirmData
            {
                confirmButtonText = "签到",
                onConfirm = () =>
                {
                    UIManager3.Instance.Tip?.Show("签到成功！");
                }
            },
            cancelData = new cancelData
            {
                cancelButtonText = "算了"
            }
        };
        UIManager3.Instance.Confirm?.Open(data);
    }
}
