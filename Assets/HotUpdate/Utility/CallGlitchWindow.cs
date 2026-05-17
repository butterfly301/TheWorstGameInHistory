using HotUpdate.UI;
using UnityEngine;

namespace HotUpdate.Utility
{
    public class CallGlitchWindow : MonoBehaviour
    {
        public void Call()
        {
            if (UIManager1.Instance != null)
            {
                UIManager1.Instance.GlitchWindow.OpenGlitchWindow();
            }
        }
    }
}
