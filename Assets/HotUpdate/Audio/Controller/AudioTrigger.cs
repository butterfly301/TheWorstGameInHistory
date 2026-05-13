using HotUpdate.Audio.Commands;
using HotUpdate.Core;
using QFramework;
using UnityEngine;

namespace HotUpdate.Audio.Controller
{
    public class AudioTrigger : MonoBehaviour, IController
    {
        public IArchitecture GetArchitecture()
        {
            return TheWorstGameInHistory.Interface;
        }

        public void PlayMusic(string musicName)
        {
            this.SendCommand(new PlayMusicCommand(musicName));
        }

        public void PlaySound(string soundName)
        {
            this.SendCommand(new PlaySoundCommand(soundName));
        }

        public void PlayVoice(string voiceName)
        {
            this.SendCommand(new PlayVoiceCommand(voiceName));
        }
    }
}