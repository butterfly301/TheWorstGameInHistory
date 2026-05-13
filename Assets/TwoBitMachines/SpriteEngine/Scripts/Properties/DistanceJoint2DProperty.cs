using System;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.TwoBitSprite
{
    [Serializable]
    public class DistanceJoint2DProperty : ExtraProperty
    {
        public bool useAnchor;
        public bool useDistance;
        public bool useConfigAnchor;
        public bool useConfigDistance;
        public bool useEnableCollision;
        public bool useMaxDistanceOnly;
        public bool useEnabled;

        public bool interpolateAnchor;
        public bool interpolateDistance;

        public DistanceJoint2D property;
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
            if (useAnchor) property.anchor = d.anchor;
            if (useDistance) property.distance = d.distance;
            if (useConfigAnchor) property.autoConfigureConnectedAnchor = d.configAnchor;
            if (useConfigDistance) property.autoConfigureDistance = d.configDistance;
            if (useEnableCollision) property.enableCollision = d.enableCollision;
            if (useMaxDistanceOnly) property.maxDistanceOnly = d.maxDistanceOnly;
            if (useEnabled) property.enabled = d.enabled;
        }

        public void SaveOriginalState()
        {
            if (property == null) return;
            original.anchor = property.anchor;
            original.distance = property.distance;
            original.configAnchor = property.autoConfigureConnectedAnchor;
            original.configDistance = property.autoConfigureDistance;
            original.enableCollision = property.enableCollision;
            original.maxDistanceOnly = property.maxDistanceOnly;
            original.enabled = property.enabled;
            originalSaved = true;
        }

        public override void Interpolate(int frameIndex, float duration, float timer)
        {
            if (property == null || frameIndex >= data.Count || duration == 0) return;

            var data1 = data[frameIndex];
            var data2 = data[frameIndex + 1 >= data.Count ? 0 : frameIndex + 1];

            if (useAnchor && interpolateAnchor)
                property.anchor = Vector2.Lerp(data1.anchor, data2.anchor, timer / duration);
            if (useDistance && interpolateDistance)
                property.distance = Mathf.Lerp(data1.distance, data2.distance, timer / duration);
        }

        [Serializable]
        public class Data
        {
            public Vector2 anchor;
            public float distance;
            public bool configAnchor;
            public bool configDistance;
            public bool enableCollision;
            public bool maxDistanceOnly;
            public bool enabled;
        }
    }
}