using System;
using UnityEngine;

namespace TwoBitMachines
{
    public class SpriteRendererColor : MonoBehaviour
    {
        public enum SetColorType
        {
            Set,
            Interpolate,
            Flash
        }

        [SerializeField] public SetColorType colorType;
        [SerializeField] public SpriteRenderer rendererRef;
        [SerializeField] public Gradient color;
        [SerializeField] public Color colorSet = Color.red;
        [SerializeField] public float speed = 5f;

        [NonSerialized] private Color origin;
        [NonSerialized] private float timeStamp;

        public void Awake()
        {
            if (rendererRef != null) origin = rendererRef.color;
        }

        public void Reset()
        {
            if (rendererRef != null) rendererRef.color = origin;
        }

        public void Update()
        {
            if (Time.time > timeStamp)
            {
                Reset();
                enabled = false;
            }
        }

        public void Activate(float value)
        {
            if (colorType == SetColorType.Set)
                Set();
            else if (colorType == SetColorType.Interpolate)
                Interpolate(value);
            else if (colorType == SetColorType.Flash) Flash();
        }

        public void Activate()
        {
            if (colorType == SetColorType.Set)
                Set();
            else if (colorType == SetColorType.Flash) Flash();
        }

        public void Interpolate(float value)
        {
            if (rendererRef != null) rendererRef.color = color.Evaluate(value);
            enabled = true;
            timeStamp = Time.time + 0.05f;
        }

        public void Flash()
        {
            if (rendererRef != null)
            {
                var sineValue = Mathf.Sin(Time.time * speed);
                var percent = (sineValue + 1.0f) / 2.0f;
                rendererRef.color = Color.Lerp(Color.white, colorSet, percent);
            }

            enabled = true;
            timeStamp = Time.time + 0.05f;
        }

        public void Set()
        {
            if (rendererRef != null) rendererRef.color = colorSet;
        }
    }
}