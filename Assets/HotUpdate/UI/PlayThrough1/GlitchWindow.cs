using HotUpdate.Core;
using HotUpdate.Data.Commands;
using HotUpdate.Interface;
using HotUpdate.SceneLoad.Commands;
using HotUpdate.Utility;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.UI
{
    public class GlitchWindow : MonoBehaviour, IController,IAutoBind
    {
        [SerializeField]private Button quit;
        [SerializeField]private Transform text;

        public IArchitecture GetArchitecture()
        {
            return TheWorstGameInHistory.Interface;
        }

        public void Init()
        {
            text.gameObject.SetActive(false);
            //typeWriterEffect.onTypeComplete.AddListener(()=>{quitButton.gameObject.SetActive(true);});
            
            quit.gameObject.SetActive(false);
            quit.onClick.AddListener(BackToMainMenu);
        }

        public void BackToMainMenu()
        {
            this.SendCommand<IncreasePlayThroughCommand>();
            this.SendCommand(new LoadSceneCommand(AddressableKeys.MainMenu_Unity, false));
        }
    }
}
