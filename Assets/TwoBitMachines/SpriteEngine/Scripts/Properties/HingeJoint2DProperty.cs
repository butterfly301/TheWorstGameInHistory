using System;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.TwoBitSprite
{
    [Serializable]
    public class HingeJoint2DProperty : ExtraProperty
    {
        public bool useAnchorX;
        public bool useAnchorY;
        public bool useLowerAngle;
        public bool useUpperAngle;
        public bool useMaxMotorForce;
        public bool useMotorSpeed;
        public bool useConfigAnchor;
        public bool useEnableCollision;
        public bool useLimits;
        public bool useMotor;
        public bool useEnabled;

        public bool interpolateAnchor;
        public bool interpolateLowerAngle;
        public bool interpolateUpperAngle;
        public bool interpolateMaxMotorForce;
        public bool interpolateMotorSpeed;

        public HingeJoint2D property;
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
            if (useAnchorX || useAnchorY)
            {
                var a = property.anchor;
                property.anchor = new Vector2(useAnchorX ? d.anchorX : a.x, useAnchorY ? d.anchorY : a.y);
            }

            if (useLowerAngle)
            {
                var limits = property.limits;
                limits.min = d.lowerAngle;
                property.limits = limits;
            }

            if (useUpperAngle)
            {
                var limits = property.limits;
                limits.max = d.upperAngle;
                property.limits = limits;
            }

            if (useMaxMotorForce)
            {
                var motor = property.motor;
                motor.maxMotorTorque = d.maximumMotorForce;
                property.motor = motor;
            }

            if (useMotorSpeed)
            {
                var motor = property.motor;
                motor.motorSpeed = d.motorSpeed;
                property.motor = motor;
            }

            if (useConfigAnchor) property.autoConfigureConnectedAnchor = d.configAnchor;
            if (useEnableCollision) property.enableCollision = d.enableCollision;
            if (useLimits) property.useLimits = d.useLimits;
            if (useMotor) property.useMotor = d.useMotor;
            if (useEnabled) property.enabled = d.enabled;
        }

        public void SaveOriginalState()
        {
            if (property == null) return;
            original.anchorX = property.anchor.x;
            original.anchorY = property.anchor.y;
            original.lowerAngle = property.limits.min;
            original.upperAngle = property.limits.max;
            original.maximumMotorForce = property.motor.maxMotorTorque;
            original.motorSpeed = property.motor.motorSpeed;
            original.configAnchor = property.autoConfigureConnectedAnchor;
            original.enableCollision = property.enableCollision;
            original.useLimits = property.useLimits;
            original.useMotor = property.useMotor;
            original.enabled = property.enabled;
            originalSaved = true;
        }

        public override void Interpolate(int frameIndex, float duration, float timer)
        {
            if (property == null || frameIndex >= data.Count || duration == 0) return;

            var data1 = data[frameIndex];
            var data2 = data[frameIndex + 1 >= data.Count ? 0 : frameIndex + 1];

            if ((useAnchorX || useAnchorY) && interpolateAnchor)
            {
                var a = property.anchor;
                if (useAnchorX) a.x = Mathf.Lerp(data1.anchorX, data2.anchorX, timer / duration);
                if (useAnchorY) a.y = Mathf.Lerp(data1.anchorY, data2.anchorY, timer / duration);
            }

            if (useLowerAngle && interpolateLowerAngle)
            {
                var limits = property.limits;
                limits.min = Mathf.Lerp(data1.lowerAngle, data2.lowerAngle, timer / duration);
                property.limits = limits;
            }

            if (useLowerAngle && interpolateUpperAngle)
            {
                var limits = property.limits;
                limits.max = Mathf.Lerp(data1.upperAngle, data2.upperAngle, timer / duration);
                property.limits = limits;
            }

            if (useMaxMotorForce && interpolateMaxMotorForce)
            {
                var motor = property.motor;
                motor.maxMotorTorque = Mathf.Lerp(data1.maximumMotorForce, data2.maximumMotorForce, timer / duration);
                property.motor = motor;
            }

            if (useMotorSpeed && interpolateMotorSpeed)
            {
                var motor = property.motor;
                motor.motorSpeed = Mathf.Lerp(data1.motorSpeed, data2.motorSpeed, timer / duration);
                property.motor = motor;
            }
        }

        [Serializable]
        public class Data
        {
            public float anchorX;
            public float anchorY;
            public float lowerAngle;
            public float upperAngle;
            public float maximumMotorForce;
            public float motorSpeed;
            public bool configAnchor;
            public bool enableCollision;
            public bool useMotor;
            public bool useLimits;
            public bool enabled;
        }
    }
}