using System;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.TwoBitSprite
{
    [Serializable]
    public class SpriteRendererProperty : ExtraProperty
    {
        public bool useAdaptiveMode;
        public bool useRenderPriority;
        public bool useSortingOrder;
        public bool useReceiveShadows;
        public bool useColor;
        public bool useSize;
        public bool useEnabled;
        public bool useFlipX;
        public bool useFlipY;

        public bool interpolateColor;
        public bool interpolateSize;

        public SpriteRenderer property;
        public Data original = new();
        public List<Data> data = new();

        public override void SetState(int frameIndex, bool firstFrame = false)
        {
            if (frameIndex >= data.Count) return;
            if (firstFrame) SaveOriginalState();
            Set(data[frameIndex]);
        }

        public override void ResetToOriginalState()
        {
            if (originalSaved) Set(original);
            originalSaved = false;
        }

        private void Set(Data d)
        {
            if (property == null) return;
            if (useColor) property.color = d.color;
            if (useSize) property.size = d.size;
            if (useRenderPriority) property.rendererPriority = d.renderPriority;
            if (useSortingOrder) property.sortingOrder = d.sortingOrder;
            if (useAdaptiveMode) property.adaptiveModeThreshold = d.adaptiveMode;
            if (useReceiveShadows) property.receiveShadows = d.receiveShadows;
            if (useEnabled) property.enabled = d.enabled;
            if (useFlipX) property.flipX = d.flipX;
            if (useFlipY) property.flipY = d.flipY;
        }

        public void SaveOriginalState()
        {
            if (property == null) return;
            original.color = property.color;
            original.size = property.size;
            original.renderPriority = property.rendererPriority;
            original.sortingOrder = property.sortingOrder;
            original.receiveShadows = property.receiveShadows;
            original.adaptiveMode = property.adaptiveModeThreshold;
            original.enabled = property.enabled;
            original.flipX = property.flipX;
            original.flipY = property.flipY;
            originalSaved = true;
        }

        public override void Interpolate(int frameIndex, float duration, float timer)
        {
            if (property == null || frameIndex >= data.Count || duration == 0) return;

            var data1 = data[frameIndex];
            var data2 = data[frameIndex + 1 >= data.Count ? 0 : frameIndex + 1];

            if (useColor && interpolateColor)
                property.color = Color.Lerp(data1.color, data2.color, timer / duration);
            if (useSize && interpolateSize)
                property.size = Vector2.Lerp(data1.size, data2.size, timer / duration);
        }

        [Serializable]
        public class Data
        {
            public Color color;
            public Vector2 size;
            public int renderPriority;
            public int sortingOrder;
            public float adaptiveMode;
            public bool receiveShadows;
            public bool enabled;
            public bool flipX;
            public bool flipY;
        }
    }
}