#region

using System;
using UnityEngine;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

#endregion

namespace TwoBitMachines.FlareEngine.ThePlayer
{
    [AddComponentMenu("")]
    public class Hide : Ability
    {
        public enum HideExitType
        {
            Automatic,
            ButtonToggle,
            Both
        }

        [SerializeField] public string button = "";
        [SerializeField] public LayerMask hideMask;

        [SerializeField] public HideExitType exit;
        [SerializeField] public int hideOrder;
        [SerializeField] [SortingLayer] public string hideLayer;

        [SerializeField] public string onHideWE;
        [SerializeField] public string onUnhideWE;
        [SerializeField] public UnityEventEffect onHide;
        [SerializeField] public UnityEventEffect onUnhide;
        [NonSerialized] private bool isHiding;
        [NonSerialized] private int sortingLayerOriginal;
        [NonSerialized] private int sortingOrderOriginal;

        [NonSerialized] private SpriteRenderer sprite;

        public override void Reset(AbilityManager player)
        {
            SetRendering(sortingLayerOriginal, sortingOrderOriginal);
            isHiding = false;
        }

        public override void Initialize(Player player)
        {
            sprite = gameObject.GetComponent<SpriteRenderer>();
            if (sprite != null)
            {
                sortingLayerOriginal = sprite.sortingLayerID;
                sortingOrderOriginal = sprite.sortingOrder;
            }
        }

        public override bool TurnOffAbility(AbilityManager player)
        {
            Reset(player);
            return true;
        }

        public override bool IsAbilityRequired(AbilityManager player, ref Vector2 velocity)
        {
            if (pause || sprite == null) return false;

            if (isHiding)
            {
                if (exit != HideExitType.ButtonToggle && !CanHide(player))
                {
                    OnUnhideEvent(player);
                    Reset(player);
                    return false;
                }

                if (exit != HideExitType.Automatic && player.inputs.Pressed(button))
                {
                    OnUnhideEvent(player);
                    Reset(player);
                    return false;
                }

                return true;
            }

            if (player.inputs.Pressed(button) && CanHide(player)) return true;
            return false;
        }

        public override void ExecuteAbility(AbilityManager player, ref Vector2 velocity,
            bool isRunningAsException = false)
        {
            OnHideEvent(player);
            isHiding = true;
            SetRendering(hideLayer, hideOrder);
            if (CanHide(player)) player.world.isHiding = true;
        }

        private void OnHideEvent(AbilityManager player)
        {
            if (isHiding)
                return;

            var impact = ImpactPacket.impact.Set(onHideWE, transform, player.world.boxCollider, transform.position,
                null, -player.world.box.right * player.playerDirection, player.playerDirection, 0);
            onHide.Invoke(impact);
        }

        private void OnUnhideEvent(AbilityManager player)
        {
            var impact = ImpactPacket.impact.Set(onUnhideWE, transform, player.world.boxCollider, transform.position,
                null, -player.world.box.right * player.playerDirection, player.playerDirection, 0);
            onUnhide.Invoke(impact);
        }

        private void SetRendering(int layer, int order)
        {
            if (sprite != null)
            {
                sprite.sortingLayerID = layer;
                sprite.sortingOrder = order;
            }
        }

        private void SetRendering(string layer, int order)
        {
            if (sprite != null)
            {
                sprite.sortingLayerName = layer;
                sprite.sortingOrder = order;
            }
        }

        private bool CanHide(AbilityManager player)
        {
            return player.world.boxCollider.IsTouchingLayers(hideMask);
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] public bool eventsFoldOut;
        [SerializeField] [HideInInspector] public bool onHideFoldOut;
        [SerializeField] [HideInInspector] public bool onUnhideFoldOut;
        public override bool OnInspector(SerializedObject controller, SerializedObject parent, string[] inputList,
            Color barColor, Color labelColor)
        {
            if (Open(parent, "Hide", barColor, labelColor))
            {
                FoldOut.Box(3, FoldOut.boxColorLight, offsetY: -2);
                {
                    parent.DropDownList(inputList, "Hide Button", "button");
                    parent.Field("Target Layer", "hideMask");
                    parent.Field("Exit", "exit");
                }
                Layout.VerticalSpacing(3);

                FoldOut.Box(2, FoldOut.boxColorLight, 3);
                {
                    parent.Field("Player Hide Sorting", "hideLayer");
                    parent.Field("Player Hide Order", "hideOrder");
                }

                if (FoldOut.FoldOutButton(parent.Get("eventsFoldOut")))
                {
                    Fields.EventFoldOutEffect(parent.Get("onHide"), parent.Get("onHideWE"), parent.Get("onHideFoldOut"),
                        "On Hide", FoldOut.boxColor);
                    Fields.EventFoldOutEffect(parent.Get("onUnhide"), parent.Get("onUnhideWE"),
                        parent.Get("onUnhideFoldOut"), "On Unhide", FoldOut.boxColor);
                }
            }

            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}