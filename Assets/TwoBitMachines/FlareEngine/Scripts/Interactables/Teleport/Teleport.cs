using System;
using System.Collections;
using System.Collections.Generic;
using TwoBitMachines.FlareEngine.ThePlayer;
using UnityEngine;
using UnityEngine.Events;

namespace TwoBitMachines.FlareEngine.Interactables
{
    [AddComponentMenu("Flare Engine/一Interactables/Teleport")]
    public class Teleport : MonoBehaviour
    {
        public enum TeleportType
        {
            Automatic,
            Button
        }

        public static List<Teleport> teleports = new();
        [SerializeField] public Transform destination;
        [SerializeField] public LayerMask layerMask;
        [SerializeField] public UnityEvent onDelayStart;
        [SerializeField] public UnityEventEffect onTriggerEnter;
        [SerializeField] public UnityEventEffect onTriggerExit;
        [SerializeField] public UnityEventEffect onTeleport;
        [SerializeField] public string teleportWE;
        [SerializeField] public string enterTriggerWE;
        [SerializeField] public string exitTriggerWE;
        [SerializeField] public float delay;
        [SerializeField] public TeleportType type;
        [SerializeField] public InputButtonSO input;
        [NonSerialized] public bool pause;
        [NonSerialized] private WaitForSecondsRealtime waitForSeconds;

        private void Awake()
        {
            WorldManager.RegisterInput(input);
            waitForSeconds = new WaitForSecondsRealtime(delay);
        }

        private void OnEnable()
        {
            if (!teleports.Contains(this)) teleports.Add(this);
        }

        private void OnDisable()
        {
            if (teleports.Contains(this)) teleports.Remove(this);
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (pause)
            {
                pause = false;
                return;
            }

            if (type == TeleportType.Automatic) ValidateTransform(other);
            onTriggerEnter.Invoke(ImpactPacket.impact.Set(enterTriggerWE, transform.position, Vector2.zero));
        }

        public void OnTriggerExit2D(Collider2D other)
        {
            onTriggerExit.Invoke(ImpactPacket.impact.Set(exitTriggerWE, transform.position, Vector2.zero));
        }

        public void OnTriggerStay2D(Collider2D other)
        {
            if (type == TeleportType.Button && input != null && input.Holding()) ValidateTransform(other);
        }

        public static void ResetAll()
        {
            for (var i = 0; i < teleports.Count; i++)
                if (teleports[i] != null)
                    teleports[i].StopAllCoroutines();
        }

        private void ValidateTransform(Collider2D other)
        {
            if (Compute.ContainsLayer(layerMask, other.gameObject.layer))
            {
                var player = other.gameObject.GetComponent<Player>();
                if (player != null)
                {
                    var playerDirection = player.abilities.playerDirection;
                    player.ResetAbilities();
                    player.abilities.playerDirection = playerDirection;
                    player.signals.SetDirection(playerDirection);
                }

                if (delay <= 0)
                {
                    TeleportNow(other.transform);
                    return;
                }

                onDelayStart.Invoke();
                StartCoroutine(TeleportDelay(other.transform));
            }
        }

        private void TeleportNow(Transform target)
        {
            if (destination == null) return;

            target.position = destination.position;
            var teleport = destination.GetComponent<Teleport>();

            if (teleport != null) teleport.pause = true;

            onTeleport.Invoke(ImpactPacket.impact.Set(teleportWE, transform.position, Vector2.zero));
        }

        private IEnumerator TeleportDelay(Transform target)
        {
            yield return waitForSeconds;
            TeleportNow(target);
        }

        #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] private bool foldOut;
        [SerializeField] [HideInInspector] private bool eventFoldOut;
        [SerializeField] [HideInInspector] private bool delayFoldOut;
        [SerializeField] [HideInInspector] private bool enterFoldOut;
        [SerializeField] [HideInInspector] private bool exitFoldOut;
#pragma warning restore 0414
#endif

        #endregion
    }
}