using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
#endif

namespace TwoBitMachines.FlareEngine
{
    public class AreaEffect : MonoBehaviour
    {
        [SerializeField] public AreaEffectType type;
        [SerializeField] public float time = 2f;
        [SerializeField] public float rate = 0.25f;

        [SerializeField] public float radius = 3f;
        [SerializeField] public Vector2 size = new(5f, 2f);
        [SerializeField] public int directions = 3;
        [SerializeField] public float speed = 25f;
        [SerializeField] public float angle = 15f;
        [SerializeField] public float offset;
        [SerializeField] public float gravity;
        [SerializeField] public float frequency;
        [SerializeField] public float amplitude;
        [SerializeField] public float rotationSpeed;
        [SerializeField] public bool offOnComplete;

        [SerializeField] public WorldEffects controller;
        [SerializeField] public List<string> effect = new();
        [SerializeField] public List<Vector2> fixedPosition = new();
        [SerializeField] public ImpactPacket impact = new();
        [NonSerialized] private float counterLength;
        [NonSerialized] private float counterRate;

        [NonSerialized] private float counterTime;
        [NonSerialized] private int fixedIndex;
        [NonSerialized] private float gravityEffect;
        [NonSerialized] private float rotationAngle;
        [NonSerialized] private float sineAngle;

        private void LateUpdate()
        {
            if (type == AreaEffectType.Directional)
            {
                sineAngle += Time.deltaTime * frequency;
                counterLength += Time.deltaTime * speed;
                gravityEffect += Time.deltaTime * gravity;
                rotationAngle += Time.deltaTime * rotationSpeed;
            }

            if (Clock.Timer(ref counterRate, rate)) Activate();
            if (Clock.Timer(ref counterTime, time)) gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            impact.Copy(WorldEffectPool.currentImpact);
            counterRate = rate;
            counterLength = 0;
            gravityEffect = 0;
            rotationAngle = 0;
            counterTime = 0;
            sineAngle = 0;
            fixedIndex = -1;

            if (impact == null) gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Draw.GLCircleInit(transform.position, 0.15f, Tint.Delete, 1);

            if (type == AreaEffectType.Circle)
            {
                Draw.GLCircleInit(transform.position, radius, Tint.Delete, (int)radius * 2);
            }
            else if (type == AreaEffectType.Box)
            {
                Draw.SquareCenter(transform.position, size, Tint.Delete);
            }
            else if (type == AreaEffectType.Directional)
            {
                Vector2 startPoint = transform.position;
                var startDirection = Vector2.right;
                var adjustOffset = (directions - 1f) * angle * 0.5f;

                for (var i = 0; i < directions; i++)
                {
                    var line = startDirection.Rotate(offset - adjustOffset + angle * i);
                    Debug.DrawRay(startPoint, line * 6f, Tint.Delete);
                }
            }
            else
            {
                if (Application.isPlaying)
                {
                    Vector2 startPoint = transform.position;
                    for (var i = 0; i < fixedPosition.Count; i++)
                        Draw.GLCircleInit(startPoint + fixedPosition[i], 0.15f, Tint.Delete, 1);
                }
            }
        }
#endif

        private void Activate()
        {
            var worldEffect = controller != null ? controller : WorldEffects.get;

            if (worldEffect == null || effect.Count == 0) return;

            var name = effect.Count == 1 ? effect[0] : effect[Random.Range(0, effect.Count)];
            impact.name = name;

            if (type == AreaEffectType.Circle)
                Circle(worldEffect);
            else if (type == AreaEffectType.Box)
                Box(worldEffect);
            else if (type == AreaEffectType.Directional)
                Directional(worldEffect);
            else
                Fixed(worldEffect);
        }

        private void Circle(WorldEffects worldEffect)
        {
            var localPosition = Vector2.right.Rotate(Random.Range(0, 360f)) * Random.Range(0.25f, 1f) * radius;
            impact.bottomPosition = (Vector2)transform.position + localPosition;
            worldEffect.Activate(impact);
        }

        private void Box(WorldEffects worldEffect)
        {
            var localPosition = new Vector2(Random.Range(-size.x * 0.5f, size.x * 0.5f),
                Random.Range(-size.y * 0.5f, size.y * 0.5f));
            impact.bottomPosition = (Vector2)transform.position + localPosition;
            worldEffect.Activate(impact);
        }

        private void Directional(WorldEffects worldEffect)
        {
            Vector2 startPoint = transform.position;
            var startDirection = Vector2.right;
            var adjustOffset = offset - (directions - 1f) * angle * 0.5f;

            for (var i = 0; i < directions; i++)
            {
                var line = startDirection.Rotate(adjustOffset + rotationAngle + angle * i);
                line = RotateToObjDirection(line) + Vector2.down * gravityEffect;
                var sineOffset = amplitude <= 0 ? Vector2.zero : line.Rotate(90f) * Mathf.Sin(sineAngle) * amplitude;
                impact.bottomPosition = startPoint + line * counterLength + sineOffset;
                impact.direction = line;
                worldEffect.ActivateWithDirection(impact);
            }
        }

        private void Fixed(WorldEffects worldEffect)
        {
            if (fixedPosition.Count == 0 || (offOnComplete && fixedIndex + 1 >= fixedPosition.Count))
            {
                gameObject.SetActive(false);
                return;
            }

            fixedIndex = fixedIndex + 1 >= fixedPosition.Count ? 0 : fixedIndex + 1;
            var localPosition = fixedIndex < fixedPosition.Count ? fixedPosition[fixedIndex] : Vector2.zero;
            impact.bottomPosition = (Vector2)transform.position + RotateToObjDirection(localPosition);
            worldEffect.Activate(impact);
        }

        private Vector2 RotateToObjDirection(Vector2 direction)
        {
            if (transform.eulerAngles.z != 0)
                direction = direction.Rotate(transform.eulerAngles.z +
                                             90f); // plus 90 offset because effects point up not right.
            if (Mathf.Abs(transform.eulerAngles.y) == 180f)
                direction = direction.Rotate(180f);
            return direction;
        }
    }

    public enum AreaEffectType
    {
        Circle,
        Box,
        Directional,
        Fixed
    }
}