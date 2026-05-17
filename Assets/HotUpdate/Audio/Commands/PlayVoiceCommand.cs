using HotUpdate.Audio.System;
using QFramework;

namespace HotUpdate.Audio.Commands
{
    public class PlayVoiceCommand : AbstractCommand
    {
        private readonly string voiceName;

public PlayVoiceCommand(string name)
        {
            voiceName = name;
        }

protected override void OnExecute()
        {
            this.GetSystem<AudioSystem>().PlayVoice(voiceName);
        }
    }
}