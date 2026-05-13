using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TwoBitMachines.FlareEngine
{
    public class BreakableProp : PropJump
    {
        [SerializeField] public UnityEventEffect onBreak = new();
        [SerializeField] public List<Rigidbody2D> list = new();
        [NonSerialized] private readonly List<Vector3> resetPoint = new();

        public void Reset()
        {
            if (rigidBody != null)
            {
                rigidBody.isKinematic = false;
                rigidBody.constraints = RigidbodyConstraints2D.None;
            }

            if (collider2D != null) collider2D.enabled = true;
            if (renderer != null) renderer.enabled = true;

            for (var i = 0; i < list.Count; i++)
            {
                if (list[i] == null) continue;
                list[i].transform.localEulerAngles = Vector3.zero;
                list[i].transform.localPosition = resetPoint[i];
                list[i].gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i] == null) continue;
                var localPosition = list[i].transform.localPosition;
                resetPoint.Add(localPosition);
            }
        }

        public void Break(ImpactPacket impact)
        {
            if (rigidBody != null)
            {
                rigidBody.isKinematic = true;
                rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
            }

            if (collider2D != null) collider2D.enabled = false;
            if (renderer != null) renderer.enabled = false;
            onBreak.Invoke(impact);

            for (var i = 0; i < list.Count; i++)
            {
                if (list[i] == null) continue;
                list[i].gameObject.SetActive(true);
                var variance = Random.Range(0.75f, 1.75f);
                var signX = Mathf.Sign(impact.direction.x);
                list[i].AddForce(Vector3.right * signX * moveForce * variance, ForceMode2D.Impulse);
                list[i].AddForce(Vector3.up * jumpForce * variance, ForceMode2D.Impulse);
                list[i].AddTorque(torqueAngle * Mathf.Deg2Rad * -signX, ForceMode2D.Impulse);
            }
        }
#pragma warning disable 0108
        [SerializeField] public Collider2D collider2D;
        [SerializeField] public SpriteRenderer renderer;
#pragma warning restore 0108
    }
}