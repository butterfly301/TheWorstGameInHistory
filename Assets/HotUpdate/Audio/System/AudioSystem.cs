using System;
using HotUpdate.Manager;
using QFramework;
using UnityEngine;

namespace HotUpdate.Audio.System
{
    public class AudioSystem : AbstractSystem
    {
        public void PlayMusic(string musicName)
        {
            AddressablesManager.Instance.LoadAssetAsync<AudioClip>(musicName, handle =>
            {
                var audioClip = handle.Result;
                AudioKit.PlayMusic(audioClip);
            });
        }

        public void PlaySound(string soundName)
        {
            AddressablesManager.Instance.LoadAssetAsync<AudioClip>(soundName, handle =>
            {
                var audioClip = handle.Result;
                AudioKit.PlaySound(audioClip);
            });
        }

        public void PlayVoice(string voiceName, Action onEnded = null)
        {
            AddressablesManager.Instance.LoadAssetAsync<AudioClip>(voiceName, handle =>
            {
                var audioClip = handle.Result;
                AudioKit.PlayVoice(audioClip, onEndedCallback: onEnded);
            });
        }

        public void PauseMusic()
        {
            AudioKit.PauseMusic();
        }

        protected override void OnInit()
        {
        }
    }
}
