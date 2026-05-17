using HotUpdate.UI;
using UnityEngine;

namespace HotUpdate.Utility
{
    public class CallGlitchWindow : MonoBehaviour
    {
        public void Call()
        {
            if (UIManager.Instance is UIManager1 uiManager1)
            {
                uiManager1.GlitchWindow.OpenGlitchWindow();
            }
        }
    }
}