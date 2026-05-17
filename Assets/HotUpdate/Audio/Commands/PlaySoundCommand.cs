using HotUpdate.Audio.System;
using QFramework;

namespace HotUpdate.Audio.Commands
{
    public class PlaySoundCommand : AbstractCommand
    {
        private readonly string soundName;

public PlaySoundCommand(string name)
        {
            soundName = name;
        }

protected override void OnExecute()
        {
            this.GetSystem<AudioSystem>().PlaySound(soundName);
        }
    }
}