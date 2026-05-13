using HotUpdate.Enums;
using HotUpdate.Manager;
using HotUpdate.UI;
using UnityEngine;

namespace HotUpdate.Utility
{
    public class TriggerDialogueBubble : MonoBehaviour
    {
        public int dialogueBubbleIndex;
        public CharacterName[] speakerNames;
        private IDialogueBubble dialogueBubble;

        private IBubble[] dialogueBubbles;

        private string totalDialogueBubblePath;

        public void Trigger()
        {
            totalDialogueBubblePath = "Assets/Prefabs/DialogueBubbles/DialogueBubble" + dialogueBubbleIndex + ".prefab";
            dialogueBubbles = new IBubble[speakerNames.Length];

            //for (var i = 0; i < speakerNames.Length; i++)
                //dialogueBubbles[i] = UIManager.Instance.GetSpeakerBubble(speakerNames[i]);

            AddressablesManager.Instance.LoadAssetAsync<GameObject>(totalDialogueBubblePath, handle =>
            {
                var dialogueBubbleObj = Instantiate(handle.Result);
                dialogueBubble = dialogueBubbleObj.GetComponent<IDialogueBubble>();
                for (var i = 0; i < dialogueBubbles.Length; i++) dialogueBubble.SetBubble(i, dialogueBubbles[i]);
                dialogueBubble.BeginConversation();
            });
        }
    }
}