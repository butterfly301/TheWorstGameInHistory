using System.Collections.Generic;
using TwoBitMachines.Editors;
using TwoBitMachines.FlareEngine.AI;
using TwoBitMachines.FlareEngine.AI.BlackboardData;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Editors
{
    [CustomEditor(typeof(AITree))]
    public class AITreeEditor : UnityEditor.Editor
    {
        public static List<string> dataList = new();
        public AITree tree;
        public string[] barNames = { "Inspect", "Target", "Territory", "Variable" };
        private GameObject objReference;
        private bool onEnable;
        private SerializedObject parent;

        public SerializedProperty rootChildren;

        private void OnEnable()
        {
            onEnable = true;
            tree = target as AITree;
            objReference = tree.gameObject;
            parent = serializedObject;

            if (dataList == null || dataList.Count == 0)
            {
                Util.GetFolderStructure("TwoBitMachines", "/FlareEngine/Scripts/AI/Behavior/Blackboard", "Blackboard",
                    dataList);
                Util.GetFolderStructure("", UserFolderPaths.Path(UserFolder.Blackboard),
                    UserFolderPaths.FolderName(UserFolder.Blackboard), dataList, false, true);
            }

            Layout.Initialize();
            if (tree.root != null) HideNodes(tree.root.children);
            if (tree.window != null)
                tree.window.Repaint();
            else if (AIBase.windowStatic != null) AIBase.windowStatic.Repaint();
        }

        private void OnDisable()
        {
            if (tree == null && objReference != null && !EditorApplication.isPlayingOrWillChangePlaymode)
                objReference.AddComponent<AIClean>();
        }

        private void OnSceneGUI()
        {
            if (tree == null || tree.root == null)
                return;
            Mouse.Update();
            for (var i = 0; i < tree.data.Count; i++)
            {
                if (tree.data[i] == null)
                {
                    tree.data.RemoveAt(i);
                    continue;
                }

                tree.data[i].OnSceneGUI(this);
                PrefabUtility.RecordPrefabInstancePropertyModifications(tree.data[i]);
            }

            RunOnSceneGUI(tree.tempChildren);
            RunOnSceneGUI(tree.root.children);

            // if (Event.current.type == EventType.Layout) UnityEditor.HandleUtility.AddDefaultControl (GUIUtility.GetControlID (FocusType.Passive)); // keep object selected, default control
        }

        public void HideNodes(List<Node> children)
        {
            if (children == null)
                return;
            for (var i = 0; i < children.Count; i++)
            {
                if (children[i] == null)
                    continue;
                children[i].hideFlags = HideFlags.HideInInspector;
                HideNodes(children[i].Children());
            }
        }

        public override void OnInspectorGUI()
        {
            Layout.VerticalSpacing(10);
            Layout.Update();
            var createUnits = false;

            if (tree.root != null)
                tree.root.hideFlags = HideFlags.HideInInspector;
            parent.Update();
            {
                AIBase.AIType(tree.transform, parent, ref createUnits);
                var bar = parent.Get("barIndex");
                FoldOut.TabBarString(Icon.Get("BackgroundLight"), FoldOut.boxColor, FoldOut.boxColor * Tint.LightGrey,
                    barNames, bar, LabelType.White);
                AIBase.BlackboardDisplay(parent, bar, tree, dataList, 0);
                if (bar.intValue != 0 && FoldOut.CornerButton(Tint.Delete))
                    BlackboardMenu.Open(tree, dataList, bar.intValue + 1);
                InspectNode(bar);
            }
            parent.ApplyModifiedProperties();

            if (tree != null && tree.reset == null)
            {
                BlackboardMenu.ai = tree;
                var newData = BlackboardMenu.CreateBlackboard(tree.data, "Variable", "BoolVariable");
                if (newData != null)
                {
                    newData.dataName = "Reset";
                    tree.reset = newData as BoolVariable;
                }
            }

            if (createUnits)
                AIBase.CreateUnits(tree);
        }

        private void InspectNode(SerializedProperty bar)
        {
            if (bar.intValue != 0)
                return;

            if (tree.inspectNode != null)
            {
                var element = new SerializedObject(tree.inspectNode);
                element.Update();
                {
                    var color = Tint.Orange;
                    FoldOut.Bar(element, Tint.Orange * Tint.WarmWhiteB)
                        .Label(Util.ToProperCase(tree.inspectNameType), Color.black, false)
                        .BR("showInfo", "Info");

                    if (!tree.inspectNode.OnInspector(tree, element, color, onEnable))
                        AIBase.IterateObject(element, tree.data, tree.inspectNode,
                            true); // if not custom inspector, display it raw
                }
                element.ApplyModifiedProperties();
            }

            if (tree.nodeMessage != null && tree.showNodeMessage)
            {
                var message = parent.Get("nodeMessage");
                FoldOut.Box(5, FoldOut.boxColor);
                {
                    message.Field("Message Size", "size");
                    Layout.VerticalSpacing(2);
                    var messg = message.Get("message");
                    var rect = Layout.CreateRect(Layout.infoWidth + 4, 70, -6);
                    messg.stringValue = EditorGUI.TextArea(rect, messg.stringValue);
                }
                Layout.VerticalSpacing(5);
            }

            onEnable = false;
        }

        private void RunOnSceneGUI(List<Node> children)
        {
            for (var i = children.Count - 1; i >= 0; i--)
            {
                if (children[i] == null)
                {
                    children.RemoveAt(i);
                    continue;
                }

                children[i].OnSceneGUI(this);
                PrefabUtility.RecordPrefabInstancePropertyModifications(children[i]);
                if (children[i].CanHaveChildren() && children[i].Children() != null)
                    RunOnSceneGUI(children[i].Children());
            }
        }

        [DrawGizmo(GizmoType.NotInSelectionHierarchy)]
        public static void DrawWhenObjectIsNotSelected(AITree tree, GizmoType gizmoType)
        {
            if (tree == null || tree.root == null)
                return;
            for (var i = 0; i < tree.data.Count; i++)
                if (tree.data[i] != null)
                    tree.data[i].DrawWhenNotSelected();
        }
    }
}