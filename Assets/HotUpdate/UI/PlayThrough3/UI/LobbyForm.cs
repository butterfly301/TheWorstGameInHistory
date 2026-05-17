using System.Collections;
using System.Collections.Generic;
using HotUpdate.Interface;
using UnityEngine;
using UnityEngine.UI;

public class LobbyForm : MonoBehaviour,IAutoBind
{
    [SerializeField]private Button btnFriend;
    public void Init()
    {
        btnFriend.onClick.AddListener(() =>
        {
            UIManager3.Instance.Confirm.Open(new ConfirmWindowData
            {
                title = "好友功能",
                content = "好友功能正在开发中，敬请期待！",
                onConfirm = () => { Debug.Log("玩家点击了确认按钮"); },
                onCancel = () => { Debug.Log("玩家点击了取消按钮"); }
            });
        });
    }
}
