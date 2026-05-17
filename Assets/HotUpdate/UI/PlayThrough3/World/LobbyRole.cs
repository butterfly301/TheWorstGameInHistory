using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HotUpdate.Audio.Commands;
using QFramework;
using HotUpdate.Core;
using HotUpdate.Utility;
using HotUpdate.Interface;

public class LobbyRoleNode : MonoBehaviour,IController,IAutoBind
{
    private string voiceName=AddressableKeys.XunYu_1_Mp3;
    [SerializeField]private RectTransform chatBubble;
    public void Init()
    {
        chatBubble.gameObject.SetActive(false);
    }

    public void PlayCharacterVoice()
    {
        this.SendCommand(new PlaySoundCommand(voiceName));
    }
    public IArchitecture GetArchitecture()
    {
        return TheWorstGameInHistory.Interface;
    }
}