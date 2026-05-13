using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TwoBitMachines
{
    [Serializable]
    public class UnityEventInt : UnityEvent<int>
    {
    }

    [Serializable]
    public class UnityEventFloat : UnityEvent<float>
    {
    }

    [Serializable]
    public class UnityEventBool : UnityEvent<bool>
    {
    }

    [Serializable]
    public class UnityEventVector2 : UnityEvent<Vector2>
    {
    }

    [Serializable]
    public class UnityEventVector3 : UnityEvent<Vector3>
    {
    }

    [Serializable]
    public class UnityEventString : UnityEvent<string>
    {
    }

    [Serializable]
    public class UnityEventStringBool : UnityEvent<string, bool>
    {
    }

    [Serializable]
    public class UnityEventNamePosition : UnityEvent<string, Vector3>
    {
    }

    [Serializable]
    public class UnityEventFloatBool : UnityEvent<float, bool>
    {
    }

    [Serializable]
    public class UnityEventFloatVector2 : UnityEvent<float, Vector2>
    {
    }

    [Serializable]
    public class UnityEventGameObject : UnityEvent<GameObject>
    {
    }

    [Serializable]
    public class UnityEventItem : UnityEvent<ItemEventData>
    {
    }

    [Serializable]
    public class UnityEventEffect : UnityEvent<ImpactPacket>
    {
    }

    [Serializable]
    public class UnityEventTransform : UnityEvent<Transform>
    {
    }

    public delegate void WorldUpdate(bool gameReset = false);

    public delegate void NormalCallback();

    public delegate void WorldResetAll();

    [Serializable]
    public class ImpactPacket
    {
        [NonSerialized] public static ImpactPacket impact = new();
        [NonSerialized] public int activateType = 0;
        [NonSerialized] public Transform attacker;
        [NonSerialized] public Vector2 bottomPosition;
        [NonSerialized] public Collider2D colliderRef;
        [NonSerialized] public float damageValue;
        [NonSerialized] public Vector2 direction;
        [NonSerialized] public int directionX; // character/object x direction
        [NonSerialized] public string name;
        [NonSerialized] public Transform transform;

        public ImpactPacket Set(string worldEffect, Vector2 position, Vector2 direction)
        {
            damageValue = 0;
            transform = null;
            colliderRef = null;
            name = worldEffect;
            bottomPosition = position;
            this.direction = direction;
            return this;
        }

        public ImpactPacket Set(string worldEffect, Transform targetTransform, Collider2D targetCollider,
            Vector2 targetPosition, Transform attackerTransform, Vector2 direction, int directionX, float damageValue)
        {
            name = worldEffect;
            this.damageValue = damageValue;
            bottomPosition = targetPosition;
            this.direction = direction;
            transform = targetTransform;
            colliderRef = targetCollider;
            attacker = attackerTransform;
            this.directionX = directionX;
            return this;
        }

        public void Copy(ImpactPacket copy)
        {
            if (copy == null)
                return;

            name = copy.name;
            damageValue = copy.damageValue;
            bottomPosition = copy.bottomPosition;
            direction = copy.direction;
            transform = copy.transform;
            colliderRef = copy.colliderRef;
            attacker = copy.attacker;
        }

        public Vector2 Center()
        {
            return colliderRef != null ? colliderRef.bounds.center : bottomPosition;
        }

        public Vector2 Top()
        {
            return colliderRef != null
                ? (Vector2)colliderRef.bounds.center + Vector2.up * colliderRef.bounds.extents.y
                : bottomPosition;
        }
    }

    [Serializable]
    public class ItemEventData
    {
        [SerializeField] public float genericFloat;
        [SerializeField] public string genericString = "";
        [SerializeField] public bool toggle;
        public bool? success = false;

        public void Reset(float genericFloat, string genericString, bool toggle)
        {
            this.genericFloat = genericFloat;
            this.genericString = genericString;
            this.toggle = toggle;
            success = null;
        }
    }

    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] public List<TKey> keys = new();

        [SerializeField] public List<TValue> values = new();

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (var pair in this)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        // load dictionary from lists
        public void OnAfterDeserialize()
        {
            Clear();
            //  Debug.Log("Deserializing dictinary");
            if (keys.Count != values.Count)
                throw new Exception(string.Format(
                    "there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

            for (var i = 0; i < keys.Count; i++) Add(keys[i], values[i]);
        }
    }
}