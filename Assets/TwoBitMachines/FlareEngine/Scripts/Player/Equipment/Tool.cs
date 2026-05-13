using System;
using System.Collections.Generic;
using TwoBitMachines.FlareEngine.ThePlayer;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TwoBitMachines.FlareEngine
{
    public class Tool : MonoBehaviour
    {
        public enum PickUpType
        {
            Set,
            SetIdle,
            SetAndSwap
        }

        [NonSerialized] public static List<Tool> tools = new();
        [SerializeField] public string toolName = "Name";
        [SerializeField] public InputButtonSO input;
        [SerializeField] public Vector2 localPosition;
        [SerializeField] public ButtonTrigger buttonTrigger;
        [SerializeField] public LayerMask pickUpLayer;
        [SerializeField] public PickUpType pickUpType;
        [SerializeField] public bool pause;
        [SerializeField] public bool pickUp;
        [NonSerialized] private Transform currentParent;

        [NonSerialized] public Character equipment;
        [NonSerialized] public bool wasHolding;

        private void Awake()
        {
            WorldManager.RegisterInput(input);
            if (pickUp)
            {
                var pickUpRef = gameObject.AddComponent<PickUp>();
                pickUpRef.tool = this;
                pickUpRef.hideFlags = HideFlags.HideInInspector;
            }
        }

        public void OnEnable()
        {
            if (!tools.Contains(this))
                tools.Add(this);
            ToolOnEnable();
        }

        public void OnDisable()
        {
            if (tools.Contains(this))
                tools.Remove(this);
        }

        public static void ResetTools()
        {
            for (var i = 0; i < tools.Count; i++)
                if (tools[i] != null)
                    tools[i].ResetAll();
        }

        public bool EquipToolToParent()
        {
            if (transform.parent == null)
            {
                if (equipment != null)
                    equipment.RemoveTool(this);
                equipment = null;
                currentParent = null;
                return false;
            }

            if (equipment != null && transform.parent != equipment.transform)
            {
                equipment.RemoveTool(this);
                equipment = null;
                currentParent = null;
                return false;
            }

            if (transform.parent == currentParent) return equipment != null;

            currentParent = transform.parent;
            equipment = transform.parent
                .GetComponent<Character>(); // a miss will only create garbage in the editor,not in the build
            if (equipment != null)
                transform.localPosition = localPosition;
            if (equipment != null)
                equipment.RegisterTool(this);
            return equipment != null;
        }

        public bool HasParent()
        {
            return equipment != null && equipment.canUseTool;
        }

        public bool MouseOverUI(InputButtonSO input)
        {
            if (input == null)
                return false;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject() &&
                EventSystem.current.currentSelectedGameObject != null)
                if (!EventSystem.current.currentSelectedGameObject.gameObject.CompareTag("UIControl"))
                {
                    if (wasHolding &&
                        input.Active(buttonTrigger))
                        return
                            false; // If button is hold type (maybe a laser), keep weapon on if player went over ui while holding button
                    wasHolding = false;
                    return true;
                }

            wasHolding = input.Active(buttonTrigger);
            return false;
        }

        public static bool PointerOverUI()
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject() &&
                EventSystem.current.currentSelectedGameObject != null)
                if (!EventSystem.current.currentSelectedGameObject.gameObject.CompareTag("UIControl"))
                    return true;

            return false;
        }

        public virtual void Recoil(ref Vector2 velocity, AnimationSignals signals)
        {
        }

        public virtual bool IsRecoiling()
        {
            return false;
        }

        public virtual void ResetAll()
        {
        }

        public virtual void AnimationComplete()
        {
        }

        public virtual void ShootAndAnimationComplete()
        {
        }

        public virtual void ShootAndWaitForAnimation()
        {
        }

        public virtual bool ChangeProjectile(string name)
        {
            return false;
        }

        public virtual void ChangeFirearmProjectile(ItemEventData itemDataEvent)
        {
            itemDataEvent.success = ChangeProjectile(itemDataEvent.genericString);
        }

        public virtual void TurnOff(AbilityManager player)
        {
        }

        public virtual void LateExecute(AbilityManager player, ref Vector2 velocity)
        {
        }

        public virtual void ToolOnEnable()
        {
        }

        public virtual float ToolValue()
        {
            return 0;
        }

        public virtual bool ToolActive()
        {
            return false;
        }

        public void Pause(bool value)
        {
            pause = value;
            ResetAll();
        }

        #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] private bool foldOut;
        [SerializeField] [HideInInspector] private bool mainFoldOut;
        [SerializeField] [HideInInspector] private bool eventFoldOut;
        [SerializeField] [HideInInspector] private bool rotateFoldOut;
        [SerializeField] [HideInInspector] private bool autoseekFoldOut;
        [SerializeField] [HideInInspector] private bool projectileFoldOut;
        [SerializeField] [HideInInspector] private bool optionsFoldOut;
        [SerializeField] [HideInInspector] private bool latchFoldOut;
        [SerializeField] [HideInInspector] private bool lineOfSightFoldOut;
        [SerializeField] [HideInInspector] private string buttonName = "";
#pragma warning restore 0414
#endif

        #endregion
    }
}