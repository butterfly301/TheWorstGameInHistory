using System;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
    public class SpriteTrail : MonoBehaviour
    {
        [SerializeField] public float effectTime = 0.5f;
        [SerializeField] public float spawnRate = 0.05f;
        [SerializeField] public Gradient gradient;
        [SerializeField] public GameObject template;

        [NonSerialized] private float counter;
        [NonSerialized] private readonly List<SpriteTrailEffect> list = new();
        [NonSerialized] private Vector2 oldPosition;
        [NonSerialized] private SpriteRenderer spriteRenderer;

        public void Start()
        {
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        }

        public void LateUpdate()
        {
            if (spriteRenderer == null)
            {
                enabled = false;
                return;
            }

            for (var i = 0; i < list.Count; i++) list[i].RunTrail();
            if ((oldPosition - (Vector2)transform.position).sqrMagnitude < 0.1f) return;
            if (Clock.Timer(ref counter, spawnRate)) CreateTrail(this, spriteRenderer);
            oldPosition = transform.position;
        }

        public void OnEnable()
        {
            spawnRate = Mathf.Clamp(spawnRate, 0.01f, 100f);
            counter = 1000f;
        }

        public void CreateTrail(SpriteTrail trail, SpriteRenderer spriteRenderer)
        {
            for (var i = 0; i < list.Count; i++)
                if (list[i].SetTrail(trail, spriteRenderer))
                    return;

            var newTrail = new SpriteTrailEffect(trail, WorldManager.get.gameObject);
            newTrail.SetTrail(trail, spriteRenderer);
            list.Add(newTrail);
        }
    }

    public class SpriteTrailEffect
    {
        [NonSerialized] public float counter;
        [NonSerialized] public GameObject gameObject;
        [NonSerialized] private Gradient gradient;
        [NonSerialized] public SpriteRenderer renderer;
        [NonSerialized] public bool set;
        [NonSerialized] private float time;

        public SpriteTrailEffect(SpriteTrail trail, GameObject parent)
        {
            gameObject = trail.template != null ? MonoBehaviour.Instantiate(trail.template) : new GameObject();
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.SetParent(parent.transform);
            gameObject.SetActive(true);
            gameObject.hideFlags = HideFlags.HideInHierarchy;
            var unit = gameObject.AddComponent<SpriteTrailUnit>();
            unit.effect = this;

            if (trail.template == null)
            {
                renderer = gameObject.AddComponent<SpriteRenderer>();
            }
            else
            {
                renderer = gameObject.GetComponent<SpriteRenderer>();
                if (renderer == null) renderer = gameObject.AddComponent<SpriteRenderer>();
            }
        }

        public void Reset()
        {
            counter = 0;
            set = false;
            if (gameObject != null) gameObject.SetActive(false);
        }

        public bool SetTrail(SpriteTrail trail, SpriteRenderer renderer)
        {
            if (set)
                return false;

            set = true;
            gameObject.SetActive(true);
            gameObject.transform.position = renderer.transform.position;
            gameObject.transform.rotation = renderer.transform.rotation;
            gameObject.transform.localScale = renderer.transform.localScale;
            gradient = trail.gradient;
            time = trail.effectTime;

            this.renderer.sprite = renderer.sprite;
            this.renderer.sortingLayerID = renderer.sortingLayerID;
            this.renderer.sortingOrder = renderer.sortingOrder - 1;
            this.renderer.flipX = renderer.flipX;
            this.renderer.flipY = renderer.flipY;
            return true;
        }

        public void RunTrail()
        {
            if (!set)
                return;

            counter += Time.deltaTime;
            if (renderer != null) renderer.color = gradient.Evaluate(Mathf.Clamp01(counter / time));
            if (counter >= time) Reset();
        }
    }
}