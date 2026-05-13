using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TwoBitMachines.FlareEngine
{
    [Serializable]
    public class WorldEffectModify
    {
        [SerializeField] public WorldEffectType type;
        [SerializeField] public EffectPosition position;
        [SerializeField] public float yOffset;
        [SerializeField] public bool useRandomX;
        [SerializeField] public bool useRandomY;
        [SerializeField] public bool useRandomRotation;
        [SerializeField] public bool flipX;
        [SerializeField] public float randomRotationMin;
        [SerializeField] public float randomRotationMax;
        [SerializeField] public float randomXOffsetMin;
        [SerializeField] public float randomXOffsetMax;
        [SerializeField] public float randomYOffsetMin;
        [SerializeField] public float randomYOffsetMax;
        [SerializeField] public bool checkForWalls;

        public void Activate(GameObject gameObject, ImpactPacket impact)
        {
            var transform = gameObject.transform;
            if (type == WorldEffectType.TextMeshPro)
            {
                var text = gameObject.GetComponent<TextMeshPro>();
                if (text != null)
                    text.SetText(impact.damageValue.ToString());
            }
            else if (type == WorldEffectType.TextMeshProNoSign)
            {
                var text = gameObject.GetComponent<TextMeshPro>();
                if (text != null)
                    text.SetText(Mathf.Abs(impact.damageValue).ToString());
            }
            else if (type == WorldEffectType.LetsWiggle)
            {
                var wiggle = gameObject.GetComponent<LetsWiggle>();
                if (wiggle != null)
                    wiggle.Activate(impact);
            }

            // position
            if (position == EffectPosition.Bottom)
                transform.position = impact.bottomPosition;
            else if (position == EffectPosition.Center)
                transform.position = impact.Center();
            else
                transform.position = impact.Top();
            if (yOffset != 0) transform.position += Vector3.up * yOffset;
            if (useRandomX)
            {
                var xDirection = impact.direction.x == 0 ? 1f : impact.direction.x;
                transform.position += Vector3.right * xDirection * Random.Range(randomXOffsetMin, randomXOffsetMax);
            }

            if (useRandomY) transform.position += Vector3.up * Random.Range(randomYOffsetMin, randomYOffsetMax);
            if (useRandomRotation)
            {
                var randomRotation = Quaternion.Euler(0, 0, Random.Range(randomRotationMin, randomRotationMax));
                transform.rotation *= randomRotation;
            }

            if (flipX) transform.localEulerAngles = new Vector3(0, impact.directionX > 0 ? 0 : 180f, 0);
            if (checkForWalls)
                if (Physics2D.OverlapPoint(transform.position, WorldManager.collisionMask))
                    transform.position = impact.Center();
        }
    }

    public enum EffectPosition
    {
        Bottom,
        Center,
        Top
    }

    public enum WorldEffectType
    {
        Normal,
        TextMeshPro,
        TextMeshProNoSign,
        LetsWiggle
    }
}