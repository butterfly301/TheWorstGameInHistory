#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.Editors
{
    [CustomEditor(typeof(LetsWiggle))]
    [CanEditMultipleObjects]
    public class WiggleEditor : UnityEditor.Editor
    {
        public string[] act;
        private LetsWiggle main;
        private SerializedObject parent;

        private void OnEnable()
        {
            main = target as LetsWiggle;
            parent = serializedObject;
            Layout.Initialize();
            if (act == null || act.Length == 0)
                act = Enum.GetNames(typeof(Act)); // Operations
        }

        public override void OnInspectorGUI()
        {
            Layout.Update();

            parent.Update();
            TweenSettings(parent.Get("tween"));
            AddWiggle();
            parent.ApplyModifiedProperties();
        }

        private void CreateNewTween(object obj)
        {
            parent.Update();
            parent.Get("tween").Get("chosenIndex").intValue = (int)obj;
            var tween = parent.Get("tween");
            var children = tween.Get("children");
            var chosenIndex = tween.Get("chosenIndex");

            if (chosenIndex.intValue > -1)
            {
                children.arraySize++;
                children.LastElement().Get("act").enumValueIndex = chosenIndex.intValue;
                chosenIndex.intValue = -1;
            }

            parent.ApplyModifiedProperties();
        }

        private void TweenSettings(SerializedProperty tween)
        {
            if (FoldOut.Bar(parent, Tint.Orange).Label("Wiggle", Color.white).FoldOut())
            {
                FoldOut.Box(3, FoldOut.boxColor, offsetY: -2);
                {
                    parent.FieldToggle("Start On Awake", "startOnAwake");
                    parent.FieldToggle("Start On Enable", "useOnEnable");
                    tween.FieldToggle("Off On Complete", "deactivate");
                }
                Layout.VerticalSpacing(3);

                FoldOut.Box(4, FoldOut.boxColor);
                {
                    tween.Field("Target", "obj");
                    tween.Field("Loop All", "loopLimit");
                    tween.FieldToggle("Unscaled Time", "unscaledTime");
                    tween.FieldToggle("Use Anchors", "useAnchors");
                }
                Layout.VerticalSpacing(5);

                FoldOut.Box(3, FoldOut.boxColor);
                {
                    parent.FieldAndEnable("Start Position", "startPosition", "useStartPosition");
                    parent.FieldAndEnable("Start Rotation", "startRotation", "useStartRotation");
                    parent.FieldAndEnable("Start Scale", "startScale", "useStartScale");
                }
                Layout.VerticalSpacing(5);
            }

            tween.SetTrue("standAlone");
            var obj = tween.Get("obj").objectReferenceValue as GameObject;

            if (obj != null)
            {
                tween.Get("transform").objectReferenceValue = obj.transform;
                tween.Get("rectTransform").objectReferenceValue = obj.GetComponent<RectTransform>();
            }

            Children(tween, tween.Get("children"));
        }

        private void Children(SerializedProperty tween, SerializedProperty children)
        {
            for (var i = 0; i < children.arraySize; i++)
            {
                var child = children.Element(i);
                var name = ((Act)child.Get("act").enumValueIndex).ToString();

                if (FoldOut.Bar(Tint.BoxTwo, 0).G(tween, children, i).Label(name).BBR("Delete"))
                {
                    children.DeleteArrayElement(i);
                    break;
                }

                if (Bar.FoldOpen(child.Get("foldOut"))) WiggleUtilEditor.TweenType(child, name);
            }
        }

        private void AddWiggle()
        {
            if (FoldOut.CornerButton(Tint.Blue))
            {
                var menu = new GenericMenu();
                for (var i = 0; i < act.Length; i++) menu.AddItem(new GUIContent(act[i]), false, CreateNewTween, i);
                menu.ShowAsContext();
            }
        }
    }
}
#endif