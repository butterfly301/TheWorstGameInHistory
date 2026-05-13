using System;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
    [AddComponentMenu("Flare Engine/WorldEffects")]
    public class WorldEffects : MonoBehaviour
    {
        [NonSerialized] public static List<WorldEffects> effects = new();
        public static WorldEffects get;
        [SerializeField] public List<WorldEffectPool> effect = new();

        #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] private bool foldOut;
#pragma warning restore 0414
#endif

        #endregion

        [NonSerialized] private GameObject container;
        [NonSerialized] private readonly Dictionary<string, WorldEffectPool> particleList = new();

        private void Awake()
        {
            get = this;
            container = new GameObject("Container");
            container.transform.parent = transform;
            for (var i = 0; i < effect.Count; i++)
            {
                effect[i].Initialize(container.transform);
                particleList.Add(effect[i].gameObject.name, effect[i]);
            }
        }

        private void OnEnable()
        {
            if (!effects.Contains(this))
                effects.Add(this);
        }

        private void OnDisable()
        {
            if (effects.Contains(this))
                effects.Remove(this);
            get = null;
        }

        public static void ResetEffects()
        {
            for (var i = 0; i < effects.Count; i++)
            {
                if (effects[i] == null)
                    continue;
                for (var j = 0; j < effects[i].effect.Count; j++)
                {
                    if (effects[i].effect[j] == null)
                        continue;
                    effects[i].effect[j].ResetAll();
                }
            }
        }

        // Sprites should be pointing updward
        public void Activate(ImpactPacket impact)
        {
            if (particleList.TryGetValue(impact.name, out var effect)) effect.Activate(impact, impact.bottomPosition);
        }

        public void ActivateWithDirection(ImpactPacket impact)
        {
            if (particleList.TryGetValue(impact.name, out var effect))
                effect.Activate(impact, impact.bottomPosition,
                    Quaternion.LookRotation(Vector3.forward, impact.direction)); //
        }

        public void ActivateWithInvertedDirection(ImpactPacket impact)
        {
            if (particleList.TryGetValue(impact.name, out var effect))
                effect.Activate(impact, impact.bottomPosition,
                    Quaternion.LookRotation(Vector3.forward, -impact.direction));
        }

        public void ActivateAndClearDirection(ImpactPacket impact)
        {
            if (particleList.TryGetValue(impact.name, out var effect))
            {
                effect.Activate(impact, impact.bottomPosition);
                WorldEffectPool.currentGameObject.transform.localEulerAngles = Vector3.zero;
            }
        }

        // private void FlipScaleY (ImpactPacket impact)
        // {
        //         // make sure sprite's orientation is correct in they axis
        //         Vector3 ls = WorldEffectPool.currentGameObject.transform.localScale;
        //         if (impact.transform.up.y >= 0)
        //         {
        //                 WorldEffectPool.currentGameObject.transform.localScale = new Vector3(ls.x, impact.direction.x > 0 ? Mathf.Abs(ls.y) : -Mathf.Abs(ls.y), ls.z);
        //         }
        //         else
        //         {
        //                 WorldEffectPool.currentGameObject.transform.localScale = new Vector3(ls.x, impact.direction.x > 0 ? -Mathf.Abs(ls.y) : Mathf.Abs(ls.y), ls.z);
        //         };
        // }

        // private void FlipScaleX (ImpactPacket impact)
        // {
        //         Vector3 ls = WorldEffectPool.currentGameObject.transform.localScale;
        //         if (impact.transform.right.y < 0)
        //         {
        //                 WorldEffectPool.currentGameObject.transform.localScale = new Vector3(-Mathf.Abs(ls.x), Mathf.Abs(ls.y), ls.z);
        //         }
        //         else
        //         {
        //                 WorldEffectPool.currentGameObject.transform.localScale = new Vector3(Mathf.Abs(ls.x), Mathf.Abs(ls.y), ls.z);
        //         };
        // }
    }

    [Serializable]
    public class WorldEffectPool
    {
        public static GameObject currentGameObject;
        public static ImpactPacket currentImpact;
        [SerializeField] public GameObject gameObject;
        [NonSerialized] private List<GameObject> list = new();
        [NonSerialized] private Transform parent;

        public void Initialize(Transform parent)
        {
            this.parent = parent;
            list.Add(gameObject);
        }

        public void ResetAll()
        {
            for (var i = 0; i < list.Count; i++)
                if (list[i] != null)
                    list[i].SetActive(false);
        }

        public void Activate(ImpactPacket impact, Vector3 position, Quaternion rotation)
        {
            for (var i = 0; i < list.Count; i++)
                if (list[i] != null && !list[i].activeInHierarchy)
                {
                    var transform = list[i].transform;
                    currentGameObject = transform.gameObject;
                    currentImpact = impact;
                    transform.position = position;
                    transform.rotation = rotation;
                    transform.gameObject.SetActive(true);
                    return;
                }

            var newEffect = MonoBehaviour.Instantiate(gameObject, position, rotation, parent);
            currentGameObject = newEffect;
            currentImpact = impact;
            list.Add(newEffect);
            newEffect.gameObject.SetActive(true);
        }

        public void Activate(ImpactPacket impact, Vector3 position)
        {
            for (var i = 0; i < list.Count; i++)
                if (list[i] != null && !list[i].activeInHierarchy)
                {
                    var transform = list[i].transform;
                    currentGameObject = transform.gameObject;
                    currentImpact = impact;
                    transform.position = position;
                    transform.gameObject.SetActive(true);
                    return;
                }

            var newEffect = MonoBehaviour.Instantiate(gameObject, position, Quaternion.identity, parent);
            currentGameObject = newEffect;
            currentImpact = impact;
            list.Add(newEffect);
            newEffect.gameObject.SetActive(true);
        }
    }
}