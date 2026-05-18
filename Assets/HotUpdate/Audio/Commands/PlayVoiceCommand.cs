using System;
using HotUpdate.Audio.System;
using QFramework;

namespace HotUpdate.Audio.Commands
{
    public class PlayVoiceCommand : AbstractCommand
    {
        private readonly string voiceName;
        private readonly Action onEnded;

        public PlayVoiceCommand(string name, Action onEnded = null)
        {
            voiceName = name;
            this.onEnded = onEnded;
        }

        protected override void OnExecute()
        {
            this.GetSystem<AudioSystem>().PlayVoice(voiceName, onEnded);
        }
    }
}
