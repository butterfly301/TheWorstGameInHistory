using System;
using HotUpdate.Manager;
using QFramework;
using UnityEngine;

namespace HotUpdate.Audio.System
{
    public class AudioSystem : AbstractSystem
    {
        public void PlayMusic(string musicName, float volumeScale = 1.0f)
        {
            AddressablesManager.Instance.LoadAssetAsync<AudioClip>(musicName, handle =>
            {
                var audioClip = handle.Result;
                AudioKit.PlayMusic(audioClip, volume: volumeScale);
            });
        }

        public void SetCurrentMusicVolumeScale(float volumeScale)
        {
            AudioKit.MusicPlayer.VolumeScale(volumeScale);
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
