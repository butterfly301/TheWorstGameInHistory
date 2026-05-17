using System.Collections;
using System.Collections.Generic;
using HotUpdate.Interface;
using UnityEngine;

public class LobbyWorldNode : MonoBehaviour,IAutoBind
{
    [SerializeField]private LobbyRoleNode role;
    public void Init()
    {
        role?.Init();
    }
    public LobbyRoleNode GetRole()
    {
        return role;
    }
}