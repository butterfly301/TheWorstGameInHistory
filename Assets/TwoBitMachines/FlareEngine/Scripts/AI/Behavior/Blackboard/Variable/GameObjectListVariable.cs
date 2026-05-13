using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI.BlackboardData
{
    [AddComponentMenu("")]
    public class GameObjectListVariable : Blackboard
    {
        public List<GameObject> value = new();

        public override Vector2 GetNearestTarget(Vector2 position)
        {
            Vector2 newTarget = transform.position;
            var sqrMagnitude = Mathf.Infinity;
            for (var i = 0; i < value.Count; i++)
            {
                if (value[i] == null)
                    continue;

                var squareDistance = (position - (Vector2)value[i].transform.position).sqrMagnitude;
                if (squareDistance < sqrMagnitude)
                {
                    sqrMagnitude = squareDistance;
                    newTarget = value[i].transform.position;
                }
            }

            return newTarget;
        }

        public override GameObject GetNearestGameObjectTarget(Vector2 position)
        {
            var newGameObject = gameObject;
            var sqrMagnitude = Mathf.Infinity;
            for (var i = 0; i < value.Count; i++)
            {
                if (value[i] == null)
                    continue;

                var squareDistance = (position - (Vector2)value[i].transform.position).sqrMagnitude;
                if (squareDistance < sqrMagnitude)
                {
                    sqrMagnitude = squareDistance;
                    newGameObject = value[i];
                }
            }

            return newGameObject;
        }

        public override Vector2 GetRandomTarget()
        {
            if (value.Count > 0)
            {
                var randomIndex = Random.Range(0, value.Count);
                return value[randomIndex] != null ? value[randomIndex].transform.position : transform.position;
            }

            return transform.position;
        }

        public override GameObject GetRandomGameObjectTarget()
        {
            if (value.Count > 0)
            {
                var randomIndex = Random.Range(0, value.Count);
                return value[randomIndex] != null ? value[randomIndex] : gameObject;
            }

            return gameObject;
        }

        public override GameObject GetGameObject()
        {
            return value.Count > 0 ? value[value.Count - 1] : null;
        }

        public override Transform GetTransform()
        {
            return GetGameObject() != null ? GetGameObject().transform : null;
        }

        public override bool AddToList(GameObject newItem)
        {
            if (newItem == null)
                return false;
            value.Add(newItem);
            return true;
        }

        public override bool RemoveFromList(GameObject item)
        {
            if (value.Contains(item))
            {
                value.Remove(item);
                return true;
            }

            return false;
        }

        public override int ListCount()
        {
            return value.Count;
        }
    }
}