#region

using UnityEngine;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

#endregion

namespace TwoBitMachines.FlareEngine.AI
{
    [AddComponentMenu("")]
    public class FindWorldFloat : Conditional
    {
        private static readonly Collider2D[] results = new Collider2D[25];
        [SerializeField] public LayerMask layerMask;
        [SerializeField] public float radius = 2f;
        [SerializeField] public FloatLogicType logic;
        [SerializeField] public CompareTo compareTo;
        [SerializeField] public float compareFloat;
        [SerializeField] public WorldFloat compareVariable;
        private ContactFilter2D filter2d;

        public override NodeState RunNodeLogic(Root root)
        {
            if (!SearchWorldFloat(out var worldFloat)) return NodeState.Failure;

            var compareValue = compareTo == CompareTo.Value ? compareFloat :
                compareTo == CompareTo.OtherVariable && compareVariable != null ? compareVariable.GetValue() : 0;
            return WorldFloatLogic.Compare(logic, worldFloat.GetValue(), compareValue);
        }

        private bool SearchWorldFloat(out WorldFloat worldFloat)
        {
            worldFloat = null;
            var found = false;
            var distance = Mathf.Infinity;
            filter2d.useTriggers = true;
            filter2d.useLayerMask = true;
            filter2d.layerMask = layerMask;
            var length = Physics2D.OverlapCircle(transform.position, radius, filter2d, results);

            for (var i = 0; i < length; i++)
            {
                var distanceSqr = (transform.position - results[i].transform.position).sqrMagnitude;
                if (distanceSqr < distance)
                {
                    var newWorldFloat = results[i].GetComponent<WorldFloat>();
                    if (newWorldFloat != null)
                    {
                        distance = distanceSqr;
                        worldFloat = newWorldFloat;
                        found = true;
                    }
                }
            }

            return found;
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        public override bool OnInspector(AIBase ai, SerializedObject parent, Color color, bool onEnable)
        {
            if (parent.Bool("showInfo"))
                Labels.InfoBoxTop(40, "Find the nearest World Float and test its value. This will work with Health.");

            var logic = parent.Enum("logic");
            var height = logic <= 4 ? 1 : 0;
            var type = parent.Enum("compareTo");
            FoldOut.Box(3 + height, color, offsetY: -2);
            {
                parent.Field("Layer", "layerMask");
                parent.Field("Radius", "radius");
                parent.Field("Logic", "logic");
                parent.FieldDouble("Compare To", "compareTo", "compareFloat", type == 0 && height == 1);
                parent.FieldDouble("Compare To", "compareTo", "compareVariable", type == 1 && height == 1);
            }
            Layout.VerticalSpacing(3);
            return true;
        }

        public override void OnSceneGUI(Editor editor)
        {
            Draw.GLCircleInit(transform.position, radius, Color.blue);
        }

#pragma warning restore 0414
#endif

        #endregion
    }
}