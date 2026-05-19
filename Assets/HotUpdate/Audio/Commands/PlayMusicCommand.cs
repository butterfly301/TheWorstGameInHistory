using HotUpdate.Audio.System;
using QFramework;

namespace HotUpdate.Audio.Commands
{
    public class PlayMusicCommand : AbstractCommand
    {
        private readonly string musicName;
        private readonly float volumeScale;

        public PlayMusicCommand(string name, float volumeScale = 1.0f)
        {
            musicName = name;
            this.volumeScale = volumeScale;
        }

        protected override void OnExecute()
        {
            this.GetSystem<AudioSystem>().PlayMusic(musicName, volumeScale);
        }
    }
}
