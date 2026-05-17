using HotUpdate.UI;
using UnityEngine;

namespace HotUpdate.Utility
{
    public class CallGlitchEffect : MonoBehaviour
    {
        public void Call(float value)
        {
            if (UIManager.Instance is UIManager1 uiManager1)
            {
                uiManager1.GlitchEffect.AdjustGlitchEffect(value);
            }
        }
    }
}