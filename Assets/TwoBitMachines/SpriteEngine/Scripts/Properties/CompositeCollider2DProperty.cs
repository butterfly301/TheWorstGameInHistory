using System;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.TwoBitSprite
{
    [Serializable]
    public class CompositeCollider2DProperty : ExtraProperty
    {
        public bool useOffsetX;
        public bool useOffsetY;
        public bool useDensity;
        public bool useIsTrigger;
        public bool usedByEffector;
        public bool useEnabled;

        public bool interpolateOffset;
        public bool interpolateDensity;

        public CompositeCollider2D property;
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
            if (useDensity) property.density = d.density;
            if (useIsTrigger) property.isTrigger = d.isTrigger;
            if (usedByEffector) property.usedByEffector = d.usedByEffector;
            if (useEnabled) property.enabled = d.enabled;
            if (useOffsetX || useOffsetY)
            {
                var o = property.offset;
                property.offset = new Vector2(useOffsetX ? d.offsetX : o.x, useOffsetY ? d.offsetY : o.y);
            }
        }

        public void SaveOriginalState()
        {
            if (property == null) return;
            original.offsetX = property.offset.x;
            original.offsetY = property.offset.y;
            original.density = property.density;
            original.isTrigger = property.isTrigger;
            original.usedByEffector = property.usedByEffector;
            original.enabled = property.enabled;
            originalSaved = true;
        }

        public override void Interpolate(int frameIndex, float duration, float timer)
        {
            if (property == null || frameIndex >= data.Count || duration == 0) return;

            var data1 = data[frameIndex];
            var data2 = data[frameIndex + 1 >= data.Count ? 0 : frameIndex + 1];

            if ((useOffsetX || useOffsetY) && interpolateOffset)
            {
                var a = property.offset;
                if (useOffsetX) a.x = Mathf.Lerp(data1.offsetX, data2.offsetX, timer / duration);
                if (useOffsetY) a.y = Mathf.Lerp(data1.offsetY, data2.offsetY, timer / duration);
            }

            if (useDensity && interpolateDensity)
                property.density = Mathf.Lerp(data1.density, data2.density, timer / duration);
        }

        [Serializable]
        public class Data
        {
            public float offsetX;
            public float offsetY;
            public float density;
            public bool isTrigger;
            public bool usedByEffector;
            public bool enabled;
        }
    }
}