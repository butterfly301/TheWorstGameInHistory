using System;
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

        public void PlayMusic(string musicName, float volumeScale = 1.0f)
        {
            this.SendCommand(new PlayMusicCommand(musicName, volumeScale));
        }

        public void SetCurrentMusicVolumeScale(float volumeScale)
        {
            this.SendCommand(new SetCurrentMusicVolumeScaleCommand(volumeScale));
        }

        public void PlaySound(string soundName)
        {
            this.SendCommand(new PlaySoundCommand(soundName));
        }

        public void PlayVoice(string voiceName, Action onEnded = null)
        {
            this.SendCommand(new PlayVoiceCommand(voiceName, onEnded));
        }
    }
}
