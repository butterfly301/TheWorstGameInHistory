using HotUpdate.Audio.System;
using QFramework;

namespace HotUpdate.Audio.Commands
{
    public class PlayMusicCommand : AbstractCommand
    {
        private readonly string musicName;

        public PlayMusicCommand(string name)
        {
            musicName = name;
        }

        protected override void OnExecute()
        {
            this.GetSystem<AudioSystem>().PlayMusic(musicName);
        }
    }
}