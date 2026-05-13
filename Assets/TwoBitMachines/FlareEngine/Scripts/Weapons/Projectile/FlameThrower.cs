using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TwoBitMachines.FlareEngine
{
    [AddComponentMenu("Flare Engine/一Weapons/FlameThrower")]
    public class FlameThrower : MonoBehaviour
    {
        [SerializeField] public int particles = 50;
        [SerializeField] public Sprite sprite;
        [SerializeField] public Material material;
        [SerializeField] public FlameParticleProperties properties;
        [NonSerialized] private Vector4[] colors;
        [NonSerialized] private float counter;

        [NonSerialized] private Mesh mesh;
        [NonSerialized] private ProjectileBase projectile;
        [NonSerialized] private readonly List<FlameParticle> projectiles = new();
        [NonSerialized] private MaterialPropertyBlock propertyBlock;
        [NonSerialized] private readonly List<Matrix4x4> tempData = new();

        public void Awake() // create mesh quad and material instance
        {
            mesh = QuadMesh.Create();
            if (material != null)
            {
                material.mainTexture = sprite.texture;
                material.enableInstancing = true;
            }

            propertyBlock = new MaterialPropertyBlock();
            colors = new Vector4[particles];
            for (var i = 0; i < particles; i++)
            {
                var newParticle = new FlameParticle();
                newParticle.sleep = true;
                newParticle.Set(properties);
                projectiles.Add(newParticle);
            }
        }

        private void Start()
        {
            projectile = gameObject.GetComponent<ProjectileBase>();
        }

        public void Update()
        {
            if (projectiles.Count == 0) return;

            if (projectile != null && projectile.triggerReleased)
            {
                if (!projectile.gameObject.activeInHierarchy) projectile.gameObject.SetActive(true);
                UpdateProjectiles();
                var allSleeping = true;
                for (var i = 0; i < projectiles.Count; i++)
                    if (!projectiles[i].sleep)
                    {
                        allSleeping = false;
                        break;
                    }

                if (allSleeping)
                {
                    gameObject.SetActive(false);
                    projectile.triggerReleased = false;
                }

                return;
            }

            if (Clock.Timer(ref counter, 0.031f))
                for (var i = 0; i < projectiles.Count; i++)
                    if (projectiles[i].sleep)
                    {
                        var startingVel = projectile != null ? projectile.playerVelocity : Vector2.zero;
                        projectiles[i].Reset(transform.position, transform, startingVel);
                        break;
                    }

            UpdateProjectiles();
        }

        private void OnEnable()
        {
            for (var i = 0; i < projectiles.Count; i++)
                if (!projectiles[i].sleep)
                    projectiles[i].sleep = true;
        }

        private void UpdateProjectiles()
        {
            tempData.Clear();
            var deltaTime = Time.deltaTime;
            for (var i = 0; i < projectiles.Count; i++)
            {
                colors[i] = projectiles[i].Execute(transform, deltaTime, i);
                tempData.Add(projectiles[i].transformData); // recheck if sleeping
            }

            propertyBlock.SetVectorArray("colors", colors);
            if (mesh != null && material != null)
                Graphics.DrawMeshInstanced(mesh, 0, material, tempData, propertyBlock);
        }
    }

    [Serializable]
    public class FlameParticle
    {
        public bool sleep;
        private float counter;
        private Vector3 position;
        private FlameParticleProperties properties;
        private Quaternion rotation = Quaternion.identity;
        private Vector3 scale = Vector3.one;
        private Vector2 startVel;
        private float variance;
        private float velAngle;

        public Matrix4x4 transformData
        {
            get
            {
                Vector3 offset = Compute.RotateVector(scale * 0.5f, rotation.eulerAngles.z + 180f);
                return Matrix4x4.TRS(position + offset, rotation, scale);
            }
        }

        public void Set(FlameParticleProperties properties)
        {
            this.properties = properties;
        }

        public void Reset(Vector3 position, Transform transform, Vector3 startVel)
        {
            this.position = position + transform.up * Random.Range(-0.25f, 0.25f) +
                            transform.right * Random.Range(-0.25f, 0.25f);
            variance = Random.Range(-properties.lifeTime * 0.2f, properties.lifeTime * 0.2f);
            velAngle = Compute.AngleDirection(Vector2.right, transform.right);
            this.startVel = transform.rotation * Compute.Abs(startVel);
            sleep = false;
            counter = 0;
        }

        public Color Execute(Transform transform, float deltaTime, int i)
        {
            var life = properties.lifeTime + variance;
            if (Clock.TimerExpired(ref counter, life))
            {
                sleep = true;
                return Color.clear;
            }

            scale.x = scale.y = properties.scaleCurve.Evaluate(counter / life) * properties.scale;
            rotation = Quaternion.Euler(0, 0, properties.angleCurve.Evaluate(counter / life) * properties.angle);
            var velocity = properties.velocityCurve.Evaluate(counter / life) * properties.velocity * deltaTime;
            var v = Compute.RotateVector(new Vector2(velocity, 0), velAngle);
            position.x += v.x + (Compute.SameSign(v.x, startVel.x) ? startVel.x : 0);
            position.y += v.y;
            return properties.colorGradient.Evaluate(Mathf.Clamp(counter / life, 0f, 1f));
        }
    }

    [Serializable]
    public class FlameParticleProperties
    {
        public float lifeTime;
        public float velocity;
        public float scale;
        public float angle;
        public AnimationCurve velocityCurve;
        public AnimationCurve scaleCurve;
        public AnimationCurve angleCurve;

        [SerializeField] public Gradient colorGradient = new()
        {
            alphaKeys = new[]
            {
                new GradientAlphaKey(0, 0f),
                new GradientAlphaKey(1, 1f)
            },

            colorKeys = new[]
            {
                new GradientColorKey(Color.red, 0f),
                new GradientColorKey(Color.cyan, 0.5f),
                new GradientColorKey(Color.green, 1f)
            }
        };
    }
}