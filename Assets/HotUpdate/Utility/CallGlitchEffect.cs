using HotUpdate.UI;
using UnityEngine;

namespace HotUpdate.Utility
{
    public class CallGlitchEffect : MonoBehaviour
    {
        public void Call(float value)
        {
            if (UIManager1.Instance != null)
            {
                UIManager1.Instance.GlitchEffect.AdjustGlitchEffect(value);
            }
        }
    }
}
