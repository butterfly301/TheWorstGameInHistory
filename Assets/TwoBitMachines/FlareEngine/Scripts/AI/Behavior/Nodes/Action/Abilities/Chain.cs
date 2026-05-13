#region

using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

#endregion

namespace TwoBitMachines.FlareEngine.AI
{
    [AddComponentMenu("")]
    public class Chain : Action
    {
        [SerializeField] private Vector2 offset = new(1f, 1f);
        [SerializeField] private Transform anchor;
        [SerializeField] private Sprite ropeSprite;
        [SerializeField] private Vector2 tetherSize = new(1f, 1f);
        [SerializeField] private List<Transform> links = new();

        public override NodeState RunNodeLogic(Root root)
        {
            if (anchor == null)
                return NodeState.Failure;
            var distance = transform.position + (Vector3)offset - anchor.position;
            for (var i = 0; i < links.Count; i++)
            {
                var localPercent = i / (float)links.Count;
                var position = anchor.transform.position + distance * localPercent; //* i
                links[i].transform.position = position;
            }

            return NodeState.Running;
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] private int segments = 3; //tethers

        public void CreateRope()
        {
            segments = segments < 1 ? 1 : segments;

            if (ropeSprite == null)
            {
                Debug.LogWarning("Chain requires a sprite.");
                return;
            }

            if (anchor == null)
            {
                Debug.LogWarning("Chain requires an anchor.");
                return;
            }

            var gameObject = new GameObject();
            gameObject.name = "Link";
            if (anchor != null)
                gameObject.transform.parent = anchor;
            gameObject.transform.localScale = tetherSize;
            gameObject.AddComponent<SpriteRenderer>().sprite = ropeSprite;

            for (var i = 0; i < links.Count; i++)
            {
                if (links[i] == null)
                    continue;
                if (i == 0)
                {
                    var renderer = links[i].gameObject.GetComponent<SpriteRenderer>();
                    if (renderer != null)
                        gameObject.GetComponent<SpriteRenderer>().color = renderer.color;
                }

                DestroyImmediate(links[i].gameObject);
            }

            links.Clear();

            links.Add(gameObject.transform);
            for (var i = 1; i < segments; i++)
            {
                var newPlank = Instantiate(gameObject, transform.position, Quaternion.identity, transform);
                newPlank.transform.parent = anchor;
                newPlank.name = "Link_" + (i + 1);
                links.Add(newPlank.transform);
            }

            for (var i = 0; i < links.Count; i++)
            {
                links[i].transform.localPosition = Vector3.zero;
                links[i].transform.localScale = gameObject.transform.localScale;
            }
        }

        public override bool OnInspector(AIBase ai, SerializedObject parent, Color color, bool onEnable)
        {
            if (parent.Bool("showInfo"))
                Labels.InfoBoxTop(85,
                    "A chain created from gameobjects. One end is anchored to a transform, the other is controlled by the by AI's position. Specify the sprite, it's size, and then press the create button." +
                    "\n \nReturns Running, Failure");

            FoldOut.Box(4, color, offsetY: -2);
            parent.Field("Links", "segments");
            parent.Field("Offset", "offset");
            parent.Field("Chain Anchor", "anchor");
            parent.FieldDouble("Rope Sprite", "ropeSprite", "tetherSize");
            Layout.VerticalSpacing(3);
            var create = FoldOut.LargeButton("Create +", Tint.Orange, Tint.White, Icon.Get("BackgroundLight"));

            if (create)
            {
                parent.ApplyModifiedProperties();
                CreateRope();
            }

            return true;
        }

        public override bool HasNextState()
        {
            return false;
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}