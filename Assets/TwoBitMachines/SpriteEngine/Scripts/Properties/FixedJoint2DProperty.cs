using System;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.TwoBitSprite
{
    [Serializable]
    public class FixedJoint2DProperty : ExtraProperty
    {
        public bool useAnchor;
        public bool useFrequency;
        public bool useDampingRatio;
        public bool useConfigAnchor;
        public bool useEnableCollision;
        public bool useEnabled;

        public bool interpolateAnchor;
        public bool interpolateFrequency;
        public bool interpolateDampingRatio;

        public FixedJoint2D property;
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
            if (useFrequency) property.frequency = d.frequency;
            if (useDampingRatio) property.dampingRatio = d.dampingRatio;
            if (useConfigAnchor) property.autoConfigureConnectedAnchor = d.configAnchor;
            if (useEnableCollision) property.enableCollision = d.enableCollision;
            if (useEnabled) property.enabled = d.enabled;
        }

        public void SaveOriginalState()
        {
            if (property == null) return;
            original.anchor = property.anchor;
            original.frequency = property.frequency;
            original.dampingRatio = property.dampingRatio;
            original.configAnchor = property.autoConfigureConnectedAnchor;
            original.enableCollision = property.enableCollision;
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
            if (useFrequency && interpolateFrequency)
                property.frequency = Mathf.Lerp(data1.frequency, data2.frequency, timer / duration);
            if (useDampingRatio && interpolateDampingRatio)
                property.dampingRatio = Mathf.Lerp(data1.dampingRatio, data2.dampingRatio, timer / duration);
        }

        [Serializable]
        public class Data
        {
            public Vector2 anchor;
            public float frequency;
            public float dampingRatio;
            public bool configAnchor;
            public bool enableCollision;
            public bool enabled;
        }
    }
}