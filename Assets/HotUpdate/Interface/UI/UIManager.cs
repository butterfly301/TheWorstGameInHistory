using System.Collections.Generic;
using HotUpdate.Dialogue.View;
using HotUpdate.Enums;
using HotUpdate.Interface;
using HotUpdate.Utility;
using QFramework;

namespace HotUpdate.UI
{
    public abstract class UIManager : MonoSingleton<UIManager>,IAutoBind
    {
        public abstract List<IInventory> GetInventory();
        public abstract void AdjustGlitchEffect(float changeValue);
        public abstract void OpenGlitchWindow();
        public abstract void OpenMapPanel();
        public abstract void CloseMapPanel();
        public abstract void OpenPausePanel();
        public abstract void ClosePausePanel();
        public abstract DialogueViewBase ShowDialogueView(DialogueViewType viewType);
    }
}