using HotUpdate.Dialogue.Controller;
using QFramework;
using UnityEngine;

namespace HotUpdate.Dialogue.Commands
{
    /// <summary>
    /// 启动对话命令
    /// </summary>
    public class StartDialogueCommand : AbstractCommand
    {
        private readonly string dialogueId;

public StartDialogueCommand(string dialogueId)
        {
            this.dialogueId = dialogueId;
        }

protected override void OnExecute()
        {
            var go = new GameObject($"Dialogue_{dialogueId}");
            var controller = go.AddComponent<DialogueController>();

controller.OnDialogueComplete = (id) => Object.Destroy(go, 0.5f);

controller.StartDialogue(dialogueId);
        }
    }
}
