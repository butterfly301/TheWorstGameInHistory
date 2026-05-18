using System.Collections;
using System.Collections.Generic;
using HotUpdate.Interface;
using UnityEngine;

public class LobbyWorldNode : MonoBehaviour,IAutoBind
{
    [SerializeField]private LobbyRoleNode lobbyRoleNode;
    public void Init()
    {
        lobbyRoleNode?.Init();
    }
}