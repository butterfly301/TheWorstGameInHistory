using HotUpdate.Audio.System;
using QFramework;

namespace HotUpdate.Audio.Commands
{
    public class PauseMusicCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.GetSystem<AudioSystem>().PauseMusic();
        }
    }
}