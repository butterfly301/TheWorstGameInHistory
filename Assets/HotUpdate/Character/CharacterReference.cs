using HotUpdate.Enums;
using HotUpdate.UI;
using UnityEngine;

namespace HotUpdate.Character
{
    public class CharacterReference : MonoBehaviour
    {
        public CharacterName characterName;

private IBubble bubble;

public virtual void Init()
        {
            var bubbleObj = transform.Find("Bubble").gameObject;
            bubble = bubbleObj.GetComponent<IBubble>();
            bubbleObj.SetActive(false);
        }
    }
}