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
        btnFriend.onClick.AddListener(OnBtnFriendClick);
    }

    private void OnBtnFriendClick()
    {
        UIManager3.Instance.Tip?.Show("好友系统正在开发中，敬请期待！");
    }
}
