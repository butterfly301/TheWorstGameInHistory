using System;
using UnityEngine;

namespace TwoBitMachines
{
    public class SpriteRendererFadeColor : MonoBehaviour
    {
        [SerializeField] public SpriteRenderer rendererRef;
        [SerializeField] public float fadeTime = 0.5f;
        [SerializeField] public float holdTime = 0.5f;
        [SerializeField] public bool reverseFade;
        [SerializeField] public bool deactivate;
        [NonSerialized] private bool exit;

        [NonSerialized] private float fadeCounter;
        [NonSerialized] private float holdCounter;

        public void Update()
        {
            if (rendererRef == null || exit) return;

            if (Clock.TimerExpired(ref holdCounter, holdTime))
            {
                var start = reverseFade ? 0f : 1f;
                var end = reverseFade ? 1f : 0f;
                var alpha = Compute.Lerp(start, end, fadeTime, ref fadeCounter, out var complete);

                var color = rendererRef.color;
                color.a = alpha;
                rendererRef.color = color;

                if (deactivate && complete) gameObject.SetActive(false);
                if (reverseFade && complete) exit = true;
            }
        }

        private void OnEnable()
        {
            if (rendererRef != null)
            {
                var color = rendererRef.color;
                color.a = reverseFade ? 0f : 1f;
                rendererRef.color = color;
            }

            fadeCounter = 0;
            holdCounter = 0;
            exit = false;
        }
    }
}