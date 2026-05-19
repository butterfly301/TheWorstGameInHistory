using HotUpdate.Audio.System;
using QFramework;

namespace HotUpdate.Audio.Commands
{
    public class SetCurrentMusicVolumeScaleCommand : AbstractCommand
    {
        private readonly float volumeScale;

        public SetCurrentMusicVolumeScaleCommand(float volumeScale)
        {
            this.volumeScale = volumeScale;
        }

        protected override void OnExecute()
        {
            this.GetSystem<AudioSystem>().SetCurrentMusicVolumeScale(volumeScale);
        }
    }
}
