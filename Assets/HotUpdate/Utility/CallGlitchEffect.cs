using HotUpdate.UI;
using UnityEngine;

namespace HotUpdate.Utility
{
    public class CallGlitchEffect : MonoBehaviour
    {
        public void Call(float value)
        {
            UIManager.Instance.AdjustGlitchEffect(value);
        }
    }
}