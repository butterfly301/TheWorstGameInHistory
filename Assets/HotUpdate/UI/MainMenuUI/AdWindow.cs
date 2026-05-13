using HotUpdate.Audio.Commands;
using HotUpdate.Browser;
using HotUpdate.Manager;
using HotUpdate.Utility;
using QFramework;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace HotUpdate.UI.MainMenuUI
{
    public class AdWindow : WindowBase
    {
        public override void Init(MainMenu mainMenuVar)
        {
            base.Init(mainMenuVar);
            quit.onClick.AddListener(PlayIceBreakerVideo);
        }

        private void PlayIceBreakerVideo()
        {
            this.SendCommand(new PauseMusicCommand());
            this.SendCommand(new OpenURLCommand(URLKeys.IceBreakerDownloadPage));
        }
    }
}