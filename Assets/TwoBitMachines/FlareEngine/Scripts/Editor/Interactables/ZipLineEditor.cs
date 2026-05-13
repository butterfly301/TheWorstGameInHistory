using TwoBitMachines.Editors;
using TwoBitMachines.FlareEngine.Interactables;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Editors
{
    [CustomEditor(typeof(Zipline))]
    public class ZipLineEditor : UnityEditor.Editor
    {
        private Zipline main;
        private SerializedObject parent;

        private void OnEnable()
        {
            main = target as Zipline;
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
                Vector2.zero, Color.red, out var changed, 0.5f);
            endOffset.vector3Value = newPoint - (Vector2)main.transform.position;

            parent.ApplyModifiedProperties();
            if (changed)
                Repaint();
        }

        public override void OnInspectorGUI()
        {
            Layout.Update();
            Layout.VerticalSpacing(10);
            parent.Update();

            if (FoldOut.Bar(parent, Tint.Blue).Label("Zipline", Color.white).FoldOut())
            {
                FoldOut.Box(8, FoldOut.boxColor);
                parent.Field("Lines", "lines");
                parent.Field("Gravity", "gravity");
                parent.Field("Up Friction", "upFriction");
                parent.Field("Bounce", "bounce");
                parent.Field("Stiffness", "stiffness");
                parent.Field("Line Renderer", "line");
                parent.FieldDouble("Area", "areaHeight", "areaOffset");
                Labels.FieldDoubleText("Height", "Offset");
                parent.FieldToggle("Create On Awake", "createOnAwake");
                Layout.VerticalSpacing(5);
            }

            var create = FoldOut.LargeButton("Create +", Tint.Orange, Tint.White, Icon.Get("BackgroundLight"), 24);
            parent.Get("view").CornerButtonLR(icon: "EyeOpen");

            parent.ApplyModifiedProperties();
            Layout.VerticalSpacing(10);

            if (create) main.CreateZipLine();
        }
    }
}