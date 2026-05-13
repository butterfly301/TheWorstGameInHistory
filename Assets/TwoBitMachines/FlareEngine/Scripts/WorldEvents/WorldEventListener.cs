using UnityEngine;
using UnityEngine.Events;

namespace TwoBitMachines.FlareEngine
{
    [AddComponentMenu("Flare Engine/一WorldEvents/WorldEventListener")]
    public class WorldEventListener : MonoBehaviour
    {
        [SerializeField] public UnityEvent onWorldEvent;
        [SerializeField] public WorldEventSO worldEvent;

        private void Start()
        {
            if (worldEvent != null) worldEvent.RegisterListener(this);
        }

        private void OnDestroy()
        {
            UnregisterListener();
        }

        public void EventTriggered()
        {
            if (onWorldEvent != null) onWorldEvent.Invoke();
        }

        public void UnregisterListener()
        {
            if (worldEvent != null) worldEvent.UnregisterListener(this);
        }

        #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] private bool foldOut;
        [SerializeField] private string eventName = "";
#pragma warning restore 0414
#endif

        #endregion
    }
}