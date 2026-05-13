using TwoBitMachines.FlareEngine.ThePlayer;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI.BlackboardData
{
    [AddComponentMenu("")]
    public class TargetPlayer : Blackboard
    {
        [SerializeField] public PlayerFindType type;
        [SerializeField] public Vector2 offset;
        [SerializeField] [HideInInspector] public int randomIndex;

        public override Vector2 GetTarget(int index = 0)
        {
            Vector2 position = transform.position;
            if (type == PlayerFindType.IsSinglePlayer)
            {
                return Player.PlayerPosition(position) + offset;
                ;
            }

            if (type == PlayerFindType.FindNearestPlayer) return NearestPlayerPosition(position);

            return RandomPlayerPosition(position);
        }

        public Vector2 NearestPlayerPosition(Vector2 returnPosition)
        {
            var players = Player.players;

            if (players.Count > 0)
            {
                var distance = float.MaxValue;
                var position = returnPosition;
                for (var i = 0; i < players.Count; i++)
                {
                    if (players[i] == null)
                        continue;
                    var sqrMag = (returnPosition - (Vector2)players[i].transform.position).sqrMagnitude;
                    if (sqrMag < distance)
                    {
                        distance = sqrMag;
                        position = players[i].transform.position;
                    }
                }

                return position + offset;
            }

            return returnPosition;
        }

        public Vector2 RandomPlayerPosition(Vector2 returnPosition)
        {
            var players = Player.players;

            if (players.Count > 0)
            {
                if (randomIndex < 0 || randomIndex >= players.Count) randomIndex = Random.Range(0, players.Count);
                if (randomIndex >= 0 && randomIndex < players.Count)
                    return players[randomIndex] != null
                        ? (Vector2)players[randomIndex].transform.position + offset
                        : returnPosition;
            }

            return returnPosition;
        }

        public override Transform GetTransform()
        {
            if (type == PlayerFindType.IsSinglePlayer) return Player.PlayerTransform();

            if (type == PlayerFindType.FindNearestPlayer) return NearestPlayerTransform(transform.position);

            return RandomPlayerTransform();
        }

        private Transform GetPlayerTransform()
        {
            if (type == PlayerFindType.IsSinglePlayer) return Player.PlayerTransform();

            if (type == PlayerFindType.FindNearestPlayer) return NearestPlayerTransform(transform.position);

            return RandomPlayerTransform();
        }

        private Transform NearestPlayerTransform(Vector2 position)
        {
            var players = Player.players;

            if (players.Count > 0)
            {
                var distance = float.MaxValue;
                Transform newTransform = null;
                for (var i = 0; i < players.Count; i++)
                {
                    if (players[i] == null)
                        continue;
                    var sqrMag = (position - (Vector2)players[i].transform.position).sqrMagnitude;
                    if (sqrMag < distance)
                    {
                        distance = sqrMag;
                        newTransform = players[i].transform;
                    }
                }

                return newTransform;
            }

            return null;
        }

        private Transform RandomPlayerTransform()
        {
            var players = Player.players;

            if (players.Count > 0)
            {
                if (randomIndex < 0 || randomIndex >= players.Count) randomIndex = Random.Range(0, players.Count);
                if (randomIndex >= 0 && randomIndex < players.Count)
                    return players[randomIndex] != null ? players[randomIndex].transform : null;
            }

            return null;
        }

        public override void ResetIndex()
        {
            randomIndex = -1;
        }

        public override void Set(Vector3 vector3)
        {
            var playerTransform = GetPlayerTransform();
            if (playerTransform != null)
                playerTransform.position = vector3;
        }

        public override void Set(Vector2 vector2)
        {
            var playerTransform = GetPlayerTransform();
            if (playerTransform != null)
                playerTransform.position = vector2;
        }

        public override Vector2 GetOffset()
        {
            return offset;
        }
    }

    public enum PlayerFindType
    {
        IsSinglePlayer,
        FindRandomPlayer,
        FindNearestPlayer
    }
}