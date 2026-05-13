
using HotUpdate.Core;
using HotUpdate.Dialogue.Commands;
using HotUpdate.Utility;
using QFramework;
using UnityEngine;

namespace HotUpdate.Video
{
    public class SkipButton1 : SkipButton,IController
    {
        [SerializeField] private DialogueID dialogueID = DialogueID.test_dialogue;
        protected override void OnSkipButtonClicked()
        {
            base.OnSkipButtonClicked();
            this.SendCommand(new StartDialogueCommand(dialogueID.ToString()));
            GetComponent<DestroyAfterDelay>()?.DestroyMyself();
        }

        public IArchitecture GetArchitecture()
        {
            return TheWorstGameInHistory.Interface;
        }
    }
}