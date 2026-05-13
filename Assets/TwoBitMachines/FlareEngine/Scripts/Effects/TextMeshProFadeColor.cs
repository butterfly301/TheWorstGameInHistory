using System;
using TMPro;
using UnityEngine;

namespace TwoBitMachines
{
    public class TextMeshProFadeColor : MonoBehaviour
    {
        [SerializeField] public TextMeshPro text;
        [SerializeField] public float fadeTime = 0.5f;
        [SerializeField] public float holdTime = 0.5f;
        [SerializeField] public bool deactivate;

        [NonSerialized] private float fadeCounter;
        [NonSerialized] private float holdCounter;

        public void Update()
        {
            if (text == null) return;

            if (Clock.TimerExpired(ref holdCounter, holdTime))
            {
                text.alpha = Compute.Lerp(1f, 0, fadeTime, ref fadeCounter, out var complete);
                if (deactivate && complete) gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            if (text != null) text.alpha = 1f;
            fadeCounter = 0;
            holdCounter = 0;
        }
    }
}