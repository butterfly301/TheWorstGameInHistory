using HotUpdate.UI;
using UnityEngine;

namespace HotUpdate.Utility
{
    public class CallGlitchWindow : MonoBehaviour
    {
        public void Call()
        {
            UIManager.Instance.OpenGlitchWindow();
        }
    }
}