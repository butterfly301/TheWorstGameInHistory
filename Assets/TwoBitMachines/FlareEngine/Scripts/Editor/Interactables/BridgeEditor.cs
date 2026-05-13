using TwoBitMachines.Editors;
using TwoBitMachines.FlareEngine.Interactables;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Editors
{
    [CustomEditor(typeof(Bridge))]
    public class BridgeEditor : UnityEditor.Editor
    {
        private Bridge main;
        private SerializedObject parent;

        private void OnEnable()
        {
            main = target as Bridge;
            parent = serializedObject;
            Layout.Initialize();
        }

        private void OnSceneGUI()
        {
            if (main == null)
                return;
            Mouse.Update();
            parent.Update();
            var endOffset = parent.Get("endOffset");
            if (endOffset.vector3Value == Vector3.zero)
                endOffset.vector3Value = new Vector3(0, 1f);
            var newPoint = SceneTools.MovePositionCircleHandle(main.transform.position + endOffset.vector3Value,
                Vector2.zero, Color.red, out var changed, 0.25f);
            endOffset.vector3Value = newPoint - (Vector2)main.transform.position;
            parent.ApplyModifiedProperties();
            main.transform.position = Compute.Round(main.transform.position, 0.25f);
            if (changed)
                Repaint();
        }

        public override void OnInspectorGUI()
        {
            Layout.Update();
            Layout.VerticalSpacing(10);
            parent.Update();

            if (FoldOut.Bar(parent, Tint.Blue).Label("Bridge", Color.white).FoldOut())
            {
                FoldOut.Box(7, FoldOut.boxColor);
                {
                    parent.Field("Planks", "planks");
                    parent.Field("Gravity", "gravity");
                    parent.Field("Bounce", "bounce");
                    parent.Field("Stiffness", "stiffness");
                    parent.FieldDouble("Plank", "plankSprite", "plankOffset");
                    Labels.FieldText("Offset");
                    parent.FieldDouble("Area", "areaHeight", "areaOffset");
                    Labels.FieldDoubleText("Height", "Offset");
                    parent.FieldToggle("Create On Awake", "createOnAwake");
                }
                Layout.VerticalSpacing(5);
            }

            var create = FoldOut.LargeButton("Create +", Tint.Orange, Tint.White, Icon.Get("BackgroundLight"), 24);
            parent.Get("view").CornerButtonLR(icon: "EyeOpen");

            parent.ApplyModifiedProperties();
            Layout.VerticalSpacing(10);

            if (create) main.CreateBridge();
        }
    }
}