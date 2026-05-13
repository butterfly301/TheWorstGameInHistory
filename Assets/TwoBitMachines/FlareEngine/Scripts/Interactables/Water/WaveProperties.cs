using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TwoBitMachines.FlareEngine.Interactables
{
    [Serializable]
    public class WaveProperties
    {
        [SerializeField] public float amplitude = 0.2f;
        [SerializeField] public float frequency = 5f;
        [SerializeField] public float speed = 1f;
        [SerializeField] public float spring = 0.05f;
        [SerializeField] public float damping = 0.04f;
        [SerializeField] public float turbulence;
        [SerializeField] public float randomCurrent;
        [SerializeField] public Point[] dynamicWave;
        [SerializeField] public Point[] wave;

        [SerializeField] private float[] leftDeltas;
        [SerializeField] private float[] rightDeltas;
        [NonSerialized] private float currentCounter;

        [NonSerialized] public float currentStrength;
        [NonSerialized] private float extraTimeLength;
        [NonSerialized] private bool round;
        [NonSerialized] private float speedRate;
        [NonSerialized] private float turbulenceTimer;

        public void Create(Water waves, int length, float particleLength)
        {
            wave = new Point[length];
            leftDeltas = new float[length];
            rightDeltas = new float[length];
            dynamicWave = new Point[length];

            for (var i = 0; i < length; i++)
            {
                var position = waves.transform.position + Vector3.up * waves.size.y +
                               Vector3.right * particleLength * i;
                dynamicWave[i] = new Point { x = position.x, y = position.y };
                wave[i] = new Point { x = position.x, y = position.y };
            }
        }

        public void Reset(float baseY)
        {
            if (dynamicWave == null) return;

            for (var i = 0; i < dynamicWave.Length; i++)
            {
                dynamicWave[i].velocity = 0;
                dynamicWave[i].y = baseY;
            }
        }

        public float Execute(Water water, float topY, float height)
        {
            if (dynamicWave == null) return 0;

            WaveCurrent(water);

            for (var i = 0; i < dynamicWave.Length; i++)
            {
                var force = spring * (dynamicWave[i].y - topY) + dynamicWave[i].velocity * damping;
                dynamicWave[i].velocity += -force;
                dynamicWave[i].y += dynamicWave[i].velocity;
            }

            for (var i = 0; i < dynamicWave.Length; i++)
            {
                if (i > 0)
                {
                    leftDeltas[i] = 0.2f * (dynamicWave[i].y - dynamicWave[i - 1].y); //                spread = 0.2f
                    dynamicWave[i - 1].velocity += leftDeltas[i];
                }

                if (i < dynamicWave.Length - 1)
                {
                    rightDeltas[i] = 0.2f * (dynamicWave[i].y - dynamicWave[i + 1].y);
                    dynamicWave[i + 1].velocity += rightDeltas[i];
                }
            }

            for (var i = 0; i < dynamicWave.Length; i++)
            {
                if (i > 0)
                    dynamicWave[i - 1].y += leftDeltas[i];
                if (i < dynamicWave.Length - 1)
                    dynamicWave[i + 1].y += rightDeltas[i];
            }

            var phase = dynamicWave.Length == 0 ? 0 : frequency;
            var baseLine = topY - height;

            for (var i = 0; i < dynamicWave.Length; i++)
            {
                var staticWaveY = topY + Mathf.Sin(speedRate + phase * i) * amplitude;
                wave[i].y = JoinWaves(topY, baseLine, staticWaveY, dynamicWave[i].position.y); //     combine both waves
            }

            return phase;
        }

        private void WaveCurrent(Water waves)
        {
            if (randomCurrent > 0)
            {
                if (Clock.Timer(ref currentCounter, randomCurrent + extraTimeLength))
                {
                    extraTimeLength = randomCurrent * Random.Range(-0.5f, 0.5f);
                    speed = -speed;
                }

                currentStrength = Mathf.MoveTowards(currentStrength, speed, Time.deltaTime * Mathf.Abs(speed) * 1.25f);
                speedRate += currentStrength * Time.deltaTime;
            }
            else
            {
                currentStrength = 0;
                speedRate = Time.time * speed;
            }

            if (turbulence > 0 && Clock.Timer(ref turbulenceTimer, 0.05f))
            {
                var randomWave = Random.Range(0, dynamicWave.Length - 1);
                ApplyImpact(randomWave, turbulence * 0.1f);
            }
        }

        public float JoinWaves(float baseY, float baseLine, float staticWave, float dynamicWave)
        {
            var dSW = round ? Compute.Round(staticWave - baseY, 0.125f) : staticWave - baseY;
            var dDW = round ? Compute.Round(dynamicWave - baseY, 0.125f) : dynamicWave - baseY;
            var waveTop = baseY + dSW + dDW;
            return waveTop < baseLine ? baseLine : waveTop;
        }

        public void ApplyImpact(int index, float impact, int splashRange = 4)
        {
            if (dynamicWave == null) return;

            for (var i = index - splashRange; i < index + splashRange; i++)
                if (i >= 0 && i < dynamicWave.Length)
                    dynamicWave[i].velocity = Mathf.Clamp(dynamicWave[i].velocity + impact, -1f, 1f);
        }
    }

    [Serializable]
    public class Point
    {
        public float x;
        public float y;
        public float velocity;
        public Vector2 position => new(x, y);
    }
}