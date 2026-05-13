using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Interactables
{
    [AddComponentMenu("Flare Engine/一Interactables/Bridge")]
    public class Bridge : MonoBehaviour //* Has partial class BridgeState, no signals
    {
        public static List<Bridge> bridges = new();
        [SerializeField] private int stiffness = 5;
        [SerializeField] private float bounce = 0.5f;
        [SerializeField] private float plankOffset;
        [SerializeField] private bool createOnAwake;
        [SerializeField] private Sprite plankSprite;

        [SerializeField] private Stick[] stick;
        [SerializeField] private Particle[] particle;
        [SerializeField] private Rect rect;
        [SerializeField] private List<Tether> plankList = new();

        [SerializeField] [HideInInspector] private Vector3 endOffset;
        [SerializeField] [HideInInspector] private int planks = 2;
        [SerializeField] [HideInInspector] private float gravity = 0.05f;
        [SerializeField] [HideInInspector] private float areaHeight = 10f;
        [SerializeField] [HideInInspector] private float areaOffset = 5f;

        #region Physics

        private void FixedUpdate()
        {
            for (var i = 0; i < particle.Length; i++) particle[i].FixedUpdate();
            for (var i = 0; i < stiffness; i++)
            for (var j = 0; j < stick.Length; j++)
                stick[j].FixedUpdate(particle);

            for (var i = 0; i < stick.Length; i++)
                if (i < plankList.Count)
                    plankList[i].BridgeRotate(particle[stick[i].first].position, particle[stick[i].second].position,
                        plankOffset);
        }

        #endregion

        #region Find

        public static void Find(WorldCollision world, Vector2 center, bool hasJumped, ref Vector2 velocity)
        {
            if (bridges.Count == 0 || Time.deltaTime == 0) return;
            for (var i = bridges.Count - 1; i >= 0; i--)
                if (bridges[i] != null && bridges[i].enabled)
                    bridges[i].Execute(i, world, center, hasJumped, ref velocity);
        }

        #endregion

        #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] private bool foldOut;
        [SerializeField] [HideInInspector] private bool view = true;

        private void OnDrawGizmos()
        {
            if (particle == null || !view) return;

            Draw.GLStart();
            for (var i = 0; i < particle.Length; i++)
            {
                Draw.GLCircle(new Vector2(particle[i].x, particle[i].y), 0.05f, Color.green, 2);
                if (i < particle.Length - 1)
                    Debug.DrawLine(particle[i].position, particle[i + 1].position, Color.yellow);
            }

            Draw.GLEnd();
            Draw.Square(rect, Color.yellow);
        }
#pragma warning restore 0414
#endif

        #endregion

        #region Initialize

        private void Awake()
        {
            if (createOnAwake) CreateBridge();
        }

        private void OnEnable()
        {
            if (!bridges.Contains(this)) bridges.Add(this);
        }

        private void OnDisable()
        {
            if (bridges.Contains(this)) bridges.Remove(this);
        }

        public void CreateBridge()
        {
            planks = planks < 2 ? 2 : planks;
            var endPoint = transform.position + endOffset;
            var distance = (transform.position - endPoint).magnitude;
            Vector2 direction = (endPoint - transform.position) / (distance == 0 ? 1f : distance);
            var plankLength = distance / planks - 0f;
            var particles = planks + 1;

            particle = new Particle[particles];
            for (var i = 0; i < particle.Length; i++)
            {
                particle[i] = new Particle((Vector2)transform.position + direction * plankLength * i, -gravity,
                    i == 0 || i == particle.Length - 1);
                if (i == particle.Length - 1) particle[i].SetPosition(endPoint);
            }

            stick = new Stick[planks];
            for (var i = 0; i < stick.Length; i++) stick[i] = new Stick(i, i + 1, plankLength);

            Vector2 p = transform.position;
            rect = new Rect(p.x, p.y - areaOffset, Mathf.Abs(endPoint.x - p.x), areaHeight);
            CreateBridgeGameObjects(plankLength, endPoint);
        }

        private void CreateBridgeGameObjects(float plankLength, Vector3 endPoint)
        {
            if (plankSprite == null)
            {
                Debug.LogWarning("Bridge requires a plank sprite.");
                return;
            }

            var gameObject = new GameObject();
            gameObject.name = "Plank";
            gameObject.transform.parent = transform;
            gameObject.AddComponent<Tether>();
            gameObject.AddComponent<SpriteRenderer>().sprite = plankSprite;

            for (var i = 0; i < plankList.Count; i++)
            {
                if (plankList[i] == null || plankList[i].gameObject == null) continue;
                if (i == 0)
                {
                    var renderer = plankList[i].gameObject.GetComponent<SpriteRenderer>();
                    if (renderer != null) gameObject.GetComponent<SpriteRenderer>().color = renderer.color;
                    gameObject.transform.localScale = plankList[i].transform.localScale;
                }

                DestroyImmediate(plankList[i].gameObject);
            }

            plankList.Clear();

            var startPosition = transform.position;
            var direction = (endPoint - startPosition).normalized;
            startPosition += direction * plankLength * 0.5f;
            plankList.Add(gameObject.GetComponent<Tether>());
            gameObject.transform.position = startPosition;

            for (var i = 1; i < planks; i++)
            {
                Vector2 position = startPosition + direction * plankLength * i;
                var newPlank = Instantiate(gameObject, position, Quaternion.identity, transform);
                newPlank.name = plankSprite.name + "_" + (i + 1);
                plankList.Add(newPlank.GetComponent<Tether>());
            }
        }

        #endregion

        #region Character

        public void Execute(int index, WorldCollision world, Vector2 center, bool hasJumped, ref Vector2 velocity)
        {
            if (rect.Contains(center))
                RunBridgeState(index, world, hasJumped, ref velocity);
            else if (world.bridge.TryGetValue(index, out var bridgeState))
                if (bridgeState != PlankState.BeginSearch)
                {
                    world.bridge[index] = PlankState.BeginSearch;
                    velocity.y -=
                        velocity.y < 0
                            ? 0.1f
                            : 0; //   character walked off bridge. Decrease velocity y to get to ground sooner and avoid the jump/falling state
                }
        }

        private bool TetherIntersection(Vector2 characterPosition, bool longSearch, float characterHeight, float velY,
            out Vector2 intersectionPoint, out int index)
        {
            index = 0;
            intersectionPoint = Vector2.zero;
            characterPosition.x =
                Mathf.Clamp(characterPosition.x, rect.x + 0.01f, rect.x + rect.width - 0.01f); //  keep center in bounds
            var characterTop = longSearch
                ? new Vector2(characterPosition.x, rect.max.y)
                : characterPosition + Vector2.up * characterHeight;
            var characterBottom = longSearch
                ? new Vector2(characterPosition.x, rect.min.y)
                : characterPosition + Vector2.up * (velY - 0.2f); //  0.2f to find bridge quicker 

            for (var i = 0; i < stick.Length; i++)
                if (Compute.LineIntersection(characterTop, characterBottom, particle[stick[i].first].position,
                        particle[stick[i].second].position, out intersectionPoint))
                {
                    index = i;
                    return true;
                }

            return false;
        }

        private Vector2 CornerHop(WorldCollision world, Vector2 bottomCenter, Vector2 intersect, Vector2 adjust,
            Vector2 corner, Vector2 start, Vector2 end, ref Vector2 velocity)
        {
            bottomCenter.x = world.box.BottomExactCorner(velocity.x).x + adjust.x;
            intersect.y = Compute.PointOnLine(start, end, bottomCenter.x, intersect.y);
            var minimize = Mathf.Abs(bottomCenter.x - corner.x) > 0.75f && intersect.y > bottomCenter.y ? 0.1f : 0;
            intersect.y = intersect.y > corner.y ? corner.y : intersect.y - minimize;
            return intersect;
        }

        private void RunBridgeState(int index, WorldCollision world, bool hasJumped, ref Vector2 velocity)
        {
            if (!world.bridge.ContainsKey(index)) world.bridge.Add(index, PlankState.BeginSearch);

            var state = world.bridge[index];
            var velocityAdjust = world.box.right * velocity.x;
            var bottomCenter = world.oldPosition + velocityAdjust;

            if (state == PlankState.BeginSearch) // character top over bridge on initial contact
                if (TetherIntersection(bottomCenter, true, 0, 0, out var intersectionPoint, out var i))
                    state = bottomCenter.y + world.box.sizeY > intersectionPoint.y
                        ? PlankState.LerpToTether
                        : PlankState.ThresholdCheck;

            if (state == PlankState.ThresholdCheck)
                if (TetherIntersection(bottomCenter, true, 0, 0, out var intersectionPoint, out var i))
                    state = bottomCenter.y > intersectionPoint.y ? PlankState.LerpToTether : state;

            if (state == PlankState.LerpToTether)
                if (velocity.y <= 0 && TetherIntersection(bottomCenter, false, world.box.sizeY, velocity.y,
                        out var intersectionPoint, out var i))
                {
                    stick[i].ApplyAcceleration(particle,
                        Vector2.down * bounce * Mathf.Abs(velocity.y / Time.deltaTime) * 0.075f);
                    velocity.y = intersectionPoint.y - bottomCenter.y;
                    state = PlankState.Latched;
                    world.onBridge = true;
                }

            if (state == PlankState.Latched)
                if (TetherIntersection(bottomCenter, true, 0, 0, out var intersectionPoint, out var i))
                {
                    if (hasJumped)
                    {
                        stick[i].ApplyAcceleration(particle,
                            Vector2.down * bounce * Mathf.Abs(velocity.y / Time.deltaTime) *
                            0.075f); // Better landing feel
                        state = PlankState.BeginSearch;
                    }
                    else
                    {
                        if (velocity.x != 0)
                        {
                            var nearDistance = world.box.sizeX * 2f;
                            var start = particle[stick[i].first].position;
                            var end = particle[stick[i].second].position;
                            if (i == stick.Length - 1 && velocity.x > 0 &&
                                Mathf.Abs(bottomCenter.x - end.x) <
                                nearDistance) //  raise character corner near bridge exit
                                intersectionPoint = CornerHop(world, bottomCenter, intersectionPoint, velocityAdjust,
                                    end, start, end, ref velocity);
                            if (i == 0 && velocity.x < 0 && Mathf.Abs(bottomCenter.x - start.x) < nearDistance)
                                intersectionPoint = CornerHop(world, bottomCenter, intersectionPoint, velocityAdjust,
                                    start, start, end, ref velocity);
                            stick[i].ApplyAcceleration(particle,
                                Vector2.down * bounce * 0.05f * (Mathf.Abs(velocity.x) / Time.deltaTime));
                        }

                        velocity.y = intersectionPoint.y + 0.02f - bottomCenter.y;
                        state = PlankState.Latched;
                        world.onBridge = true;
                    }
                }

            world.bridge[index] = state;
        }

        #endregion
    }
}

namespace TwoBitMachines.FlareEngine
{
    public enum PlankState
    {
        BeginSearch,
        LerpToTether,
        ThresholdCheck,
        Latched,
        Jumped
    }
}