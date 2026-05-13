using TwoBitMachines.Editors;
using TwoBitMachines.FlareEngine.Interactables;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Editors
{
    [CustomEditor(typeof(Rope))]
    public class RopeEditor : UnityEditor.Editor
    {
        private Rope main;
        private SerializedObject parent;

        private void OnEnable()
        {
            main = target as Rope;
            parent = serializedObject;
            Layout.Initialize();
        }

        private void OnSceneGUI()
        {
            parent.Update();
            var endAnchor = parent.Get("endOffset");

            if (endAnchor.vector2Value == Vector2.zero) endAnchor.vector2Value = new Vector2(0, 1f);
            var newPoint = SceneTools.MovePositionCircleHandle(
                main.transform.position + (Vector3)endAnchor.vector2Value, Vector2.zero, Color.red, out var changed);
            endAnchor.vector2Value = newPoint - (Vector2)main.transform.position;
            parent.ApplyModifiedProperties();

            if (changed)
                Repaint();
        }

        public override void OnInspectorGUI()
        {
            Layout.Update();
            Layout.VerticalSpacing(10);
            parent.Update();

            if (FoldOut.Bar(parent, Tint.Blue).Label("Rope", Color.white).FoldOut())
            {
                var type = parent.Enum("type");
                FoldOut.Box(type == 1 ? 4 : 3, FoldOut.boxColor);
                parent.Field("Type", "type");
                parent.Field("Search Radius", "tetherRadius", type == 0);
                parent.FieldToggleAndEnable("Is Climbable", "isClimbable", execute: type == 0);
                parent.Field("Rope Radius", "ropeRadius", type == 1);
                parent.Field("Tether Radius", "tetherRadius", type == 1);
                parent.Field("Force", "force", type == 1);
                Layout.VerticalSpacing(5);

                FoldOut.Box(4, FoldOut.boxColor);
                parent.Field("Tethers", "segments");
                parent.Field("Gravity", "gravity");
                parent.Field("Stiffness", "stiffness");
                parent.FieldToggle("Double Anchor", "doubleAnchor");
                Layout.VerticalSpacing(5);

                FoldOut.Box(2, FoldOut.boxColor);
                parent.FieldDouble("Rope Sprite", "ropeSprite", "tetherSize");
                parent.Field("Rope End (Optional)", "ropeEnd");
                Layout.VerticalSpacing(5);
            }

            var create = FoldOut.LargeButton("Create +", Tint.Orange, Tint.White, Icon.Get("BackgroundLight"), 24);
            parent.Get("view").CornerButtonLR(icon: "EyeOpen");

            parent.ApplyModifiedProperties();
            Layout.VerticalSpacing(10);

            if (create) main.CreateRope();
        }
    }
}