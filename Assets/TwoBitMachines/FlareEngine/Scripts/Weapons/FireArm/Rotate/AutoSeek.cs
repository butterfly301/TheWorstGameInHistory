using System;
using UnityEngine;
using UnityEngine.Events;

namespace TwoBitMachines.FlareEngine
{
    [Serializable]
    public class AutoSeek
    {
        [SerializeField] public LayerMask layer;
        [SerializeField] public float maxRadius = 10f;
        [SerializeField] public float searchRate = 0.5f;
        [SerializeField] public bool autoShoot;
        [SerializeField] public UnityEvent onFoundTarget;
        [NonSerialized] private float counter;
        [NonSerialized] private Collider2D currentTarget;
        [NonSerialized] private ContactFilter2D filter;
        [NonSerialized] private bool found;
        [NonSerialized] private Collider2D[] list = new Collider2D[50];

        [NonSerialized] public bool newTarget;

        public void Initiate()
        {
            filter.useLayerMask = true;
            filter.layerMask = layer;
        }

        public void Reset()
        {
            currentTarget = null;
            newTarget = false;
            found = false;
            counter = 0;
        }

        public bool Rotate()
        {
            return found && currentTarget != null;
        }

        public Vector3 Position(Vector3 defaultPosition)
        {
            return currentTarget != null ? currentTarget.transform.position : defaultPosition;
        }

        public void Seek(Vector3 position, ref bool fire)
        {
            if (Clock.Timer(ref counter, searchRate))
            {
                found = false;
                newTarget = false;
                Initiate();
                var size = Physics2D.OverlapCircle(position, maxRadius, filter,
                    list); // if collider 2D is set as Is Trigger, this will not work

                if (size > 0)
                {
                    var distance = Mathf.Infinity;
                    for (var i = 0; i < size; i++)
                    {
                        var squareDistance = (list[i].transform.position - position).sqrMagnitude;
                        if (squareDistance < distance)
                        {
                            if (currentTarget != list[i]) newTarget = true;
                            found = true;
                            distance = squareDistance;
                            currentTarget = list[i];
                        }
                    }
                }

                if (newTarget) onFoundTarget.Invoke();
                if (!found)
                {
                    currentTarget = null;
                    newTarget =
                        true; //         even if nothing found, set true so that rotate can smoothly rotate to normal position
                }
            }

            fire = fire || (autoShoot && found);
        }

        #region

#if UNITY_EDITOR
#pragma warning disable 0414
        public bool eventsFoldOut;
        public bool onFoundTargetFoldOut;
#pragma warning restore 0414
#endif

        #endregion
    }
}