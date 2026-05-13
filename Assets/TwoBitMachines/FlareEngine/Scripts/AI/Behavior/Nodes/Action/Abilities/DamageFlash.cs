#region

using System;
using UnityEngine;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

#endregion

namespace TwoBitMachines.FlareEngine.AI
{
    [AddComponentMenu("")]
    public class DamageFlash : Action
    {
        [SerializeField] public DamageFlashType type;
        [SerializeField] public SpriteRenderer spriteRenderer;
        [SerializeField] public int flashes = 2;
        [SerializeField] public float interval = 0.1f;
        [SerializeField] public bool useImmediately = true;
        [SerializeField] public Color color = Color.white;
        [SerializeField] public Material material;
        [NonSerialized] public bool activate;
        [NonSerialized] public float counter;
        [NonSerialized] public int flash;
        [NonSerialized] public Color originColor;

        [NonSerialized] public Material originMaterial;
        [NonSerialized] public bool toggle;

        private void Awake()
        {
            if (spriteRenderer != null)
                originColor = spriteRenderer.color;
            if (spriteRenderer != null)
                originMaterial = spriteRenderer.material;
        }

        public void Activate()
        {
            activate = true;
        }

        public void Deactivate()
        {
            activate = false;
        }

        public override void OnReset(bool skip = false, bool enteredState = false)
        {
            if (spriteRenderer != null)
                spriteRenderer.color = originColor; // reset
            if (spriteRenderer != null)
                spriteRenderer.material = originMaterial;
        }

        public override NodeState RunNodeLogic(Root root)
        {
            if (!useImmediately && !activate) return NodeState.Failure;

            if (spriteRenderer == null) return NodeState.Failure;
            if (nodeSetup == NodeSetup.NeedToInitialize)
            {
                toggle = false;
                counter = 10000000; // put large number to execute timer immediately
                flash = -1;
            }

            if (TwoBitMachines.Clock.Timer(ref counter, interval))
            {
                toggle = !toggle;
                flash++;
                Flash();
                if (flash >= flashes * 2)
                {
                    activate = false;
                    return NodeState.Success;
                }
            }

            return NodeState.Running;
        }

        private void Flash()
        {
            if (type == DamageFlashType.SpriteRenderer)
            {
                spriteRenderer.color = toggle ? color : originColor;
            }
            else if (material != null)
            {
                material.color = color;
                spriteRenderer.material = toggle ? material : originMaterial;
            }
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        public override bool OnInspector(AIBase ai, SerializedObject parent, Color color, bool onEnable)
        {
            if (parent.Bool("showInfo"))
                Labels.InfoBoxTop(55, "Show damage flash. The time of each flash will be equal to the interval value." +
                                      "\n \nReturns Running, Success, Failure");

            var type = parent.Enum("type");
            var height = type == 1 ? 1 : 0;
            FoldOut.Box(6 + height, color, offsetY: -2);
            parent.Field("Type", "type");
            parent.Field("Sprite Renderer", "spriteRenderer");
            parent.Field("Material", "material", type == 1);
            parent.Field("Color", "color");
            parent.Field("Flashes", "flashes");
            parent.Field("Interval", "interval");
            parent.FieldToggle("Use Immediately", "useImmediately");
            Layout.VerticalSpacing(3);
            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }

    public enum DamageFlashType
    {
        SpriteRenderer,
        Material
    }
}