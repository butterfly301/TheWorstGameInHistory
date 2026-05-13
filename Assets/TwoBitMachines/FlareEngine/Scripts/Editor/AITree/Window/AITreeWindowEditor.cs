using System;
using System.Collections.Generic;
using TwoBitMachines.Editors;
using TwoBitMachines.FlareEngine.AI;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Editors
{
    public class AITreeWindowEditor : EditorWindow
    {
        public static List<string> nodeList = new();
        public AITree tree;
        public DrawNodes draw = new();
        public WindowZoom zoom = new();
        public BranchTemplate template;
        public Matrix4x4 oldGUIMatrix;
        public Material lineMaterial;
        public Node isParentNode;
        public string[] results;
        public Vector2 mousePosition;
        public Color backgroundColor = Tint.WarmWhite;

        public ContextMenus menu = new();
        public Vector2 gridOffset => new(tree.panX, tree.panY);

        private void OnEnable()
        {
            GetProperties();
            var r = PlayerPrefs.GetFloat("TreeEditorColorR", Tint.WarmWhite.r);
            var g = PlayerPrefs.GetFloat("TreeEditorColorG", Tint.WarmWhite.g);
            var b = PlayerPrefs.GetFloat("TreeEditorColorB", Tint.WarmWhite.b);
            var a = PlayerPrefs.GetFloat("TreeEditorColorA", Tint.WarmWhite.a);
            backgroundColor = new Color(r, g, b, a);
        }

        private void OnDisable()
        {
            SaveBackgroundColor();
        }

        public void OnGUI()
        {
            menu.Set(this, template);
            if (SafetyCheckFailed())
            {
                GetProperties();
                TreeNotDetectedScreen();
                return;
            }

            if (template == null)
                template = AssetDatabase.LoadAssetAtPath<BranchTemplate>(
                    "Assets/TwoBitMachines/FlareEngine/Scripts/AI/Behavior/Tree/Template.asset");

            var debug = Application.isPlaying;
            ResetVariables();
            Clock.SimulateTimeEditor();

            DragWindow(); // has a small jump when value is being wrapped
            DrawBackground();
            // zoom.Begin(position, tree.scaling, tree.ScrollPosition, tree.oldGUIMatrix);


            zoom.Begin(position, tree.scaling, tree.ScrollPosition, tree.oldGUIMatrix);

            DrawGrid();
            ImplementNode(tree.root, debug, 0, null, Event.current, Vector2.zero, false);
            TraverseTree(tree.tempChildren, debug, 0, Event.current, Vector2.zero,
                false); // orphans, children without parents, not connected to main tree
            menu.NodeMenu();

            zoom.End(position);
            zoom.SetVariables(ref tree.scaling, ref tree.ScrollPosition, ref tree.oldGUIMatrix);
            var nameRect = new Rect(10, 28, 1000, 20); // size
            GUI.Label(nameRect, tree.gameObject.name, EditorStyles.boldLabel);

            var colorRect = new Rect(10, 52, 50, 20); // size
            var previousColor = backgroundColor;
            backgroundColor = EditorGUI.ColorField(colorRect, backgroundColor);
            if (previousColor != backgroundColor)
                SaveBackgroundColor();

            if (GUI.changed || debug) Repaint();
        }

        [MenuItem("Window/Flare Behavior Tree")]
        private static void ShowEditor()
        {
            var editor = GetWindow<AITreeWindowEditor>();
            editor.GetProperties();
        }

        private void Save()
        {
            var saveTree = new SaveTree();
            saveTree.tree = tree;
            Storage.Save(saveTree, "TwoBitMachinesTreeEditor", "AITree");
        }

        private void SaveBackgroundColor()
        {
            PlayerPrefs.SetFloat("TreeEditorColorR", backgroundColor.r);
            PlayerPrefs.SetFloat("TreeEditorColorG", backgroundColor.g);
            PlayerPrefs.SetFloat("TreeEditorColorB", backgroundColor.b);
            PlayerPrefs.SetFloat("TreeEditorColorA", backgroundColor.a);
        }

        public void GetProperties()
        {
            lineMaterial = lineMaterial == null ? new Material(Shader.Find("Sprites/Default")) : lineMaterial;
            if (nodeList == null || nodeList.Count == 0)
                Util.GetFolderStructure("TwoBitMachines", "/FlareEngine/Scripts/AI/Behavior/Nodes", "Nodes", nodeList);
            Layout.Initialize();
            draw.Initialize();
        }

        public void ResetVariables()
        {
            Layout.Update();
            tree.snapToGrid = 10;
            menu.Set(this, template);
            if (Mouse.up || Mouse.down)
                tree.aNodeIsBeingDragged = false;
        }

        public void TreeNotDetectedScreen()
        {
            Layout.Update();
            DrawBackground();
            zoom.Begin(position, 1, Vector2.zero, oldGUIMatrix);
            zoom.Grid(lineMaterial, 10, 10);
            zoom.End(position);
            var titleRect = new Rect(10, 28, 1000, 20); // size
            GUI.Label(titleRect, "Tree Not Detected - Select GameObject", EditorStyles.boldLabel);
            if (GUI.changed)
                Repaint();
        }

        public bool SafetyCheckFailed()
        {
            if (Selection.activeGameObject != null)
            {
                var newTree = Selection.activeGameObject.GetComponent<AITree>();
                if (newTree != null)
                {
                    tree = newTree;
                    Save();
                }
            }

            if (tree == null) tree = Storage.Load(new SaveTree(), "TwoBitMachinesTreeEditor", "AITree").tree;
            if (tree != null && tree.root == null)
            {
                var tempRoot = tree.GetComponent<Root>();
                if (tempRoot == null)
                {
                    var center = new Vector2(Screen.width / 2f - 30f, Screen.height / 4f);
                    if (menu.CreateNode("Root", Compute.Round(center, 15f), out var newNode))
                        tree.root = newNode as Root;
                }
                else
                {
                    tree.root = tempRoot;
                }
            }

            if (tree != null && tree.tempChildren.Contains(tree.root)) tree.tempChildren.Remove(tree.root);
            if (tree != null) tree.window = this;
            AIBase.windowStatic = this;
            return tree == null || tree.root == null || lineMaterial == null;
        }

        private void DragWindow()
        {
            var rootPosition = new Rect(position)
                { x = 0, y = 0 }; // regular position is offset by unknown amount, use origin
            if (rootPosition.ContainsScrollWheelDrag())
            {
                var panLimit = 2500f; // Arbitrary
                tree.panX = Mathf.Clamp(Event.current.delta.x / zoom.scaling + tree.panX, -panLimit, panLimit);
                tree.panY = Mathf.Clamp(Event.current.delta.y / zoom.scaling + tree.panY, -panLimit, panLimit);
                GUI.changed = true;
            }
        }

        public void DrawBackground()
        {
            var previousColor = GUI.color;
            GUI.color = backgroundColor;
            var background = new Rect(0, 0, maxSize.x, maxSize.y);
            GUI.DrawTexture(background, Texture2D.whiteTexture);
            GUI.color = previousColor;
        }

        public void DrawGrid()
        {
            zoom.Grid(lineMaterial, tree.panX, tree.panY);
        }

        public void TraverseTree(List<Node> currentChildren, bool debug, int index, Event eventCurrent, Vector2 delta,
            bool hide)
        {
            for (var i = 0; i < currentChildren.Count; i++)
                ImplementNode(currentChildren[i], debug, i, currentChildren, eventCurrent, delta, hide);
        }

        public void ImplementNode(Node node, bool debug, int index, List<Node> siblings, Event currentEvent,
            Vector2 parentMoved, bool hide)
        {
            if (node == null)
                return;

            node.rectPosition += parentMoved; // always update if parentMoved
            MoveNotes(node, parentMoved);

            if (!hide) // Node can be seen
            {
                MoveNode(node, gridOffset, currentEvent, ref parentMoved);
                var visualRect = draw.Draw(tree, node, debug, parentMoved, gridOffset, currentEvent);

                if (node.isDragged) tree.aNodeIsBeingDragged = true;
                if (node.isDragged && siblings != null &&
                    siblings.Count > 0) // if node has moved, reorder all siblings in list
                    siblings.Sort((x, y) => x.rectPosition.x.CompareTo(y.rectPosition.x)); // reorder list by position.x
                if (visualRect.ContainsMouseDown() || visualRect.ContainsMouseRightDown(false))
                {
                    if (tree.inspectNode != null) tree.inspectNode.inspectSelected = false;
                    if (Selection.activeGameObject != null && Selection.activeGameObject != tree.gameObject)
                        Selection.activeGameObject = tree.gameObject;
                    node.inspectSelected = true;
                    tree.showNodeMessage = false;
                    tree.nodeMessage = null;
                    tree.inspectNode = node;
                    tree.inspectNameType = node.nameType;
                    tree.barIndex = 0;
                }

                if (visualRect.ContainsMouseRightDown()) menu.UtilityMenu(node);
                if (node.lookingForChild) isParentNode = node;
                if (node.childActive)
                {
                    if (node.nodeParent != null)
                        DisconnectChildFromParent(node);
                    else if (isParentNode != null && node != isParentNode) ConnectChildToParent(node);
                    node.childActive = node.lookingForChild = false;
                    if (isParentNode != null)
                    {
                        isParentNode.lookingForChild = isParentNode.childActive = false;
                        isParentNode = null;
                    }
                }
            }

            if (node.Children() != null && node.Children().Count > 0)
                TraverseTree(node.Children(), debug, index, currentEvent, parentMoved, hide || node.hideChildren);
        }

        public void MoveNode(Node node, Vector2 gridOffset, Event currentEvent, ref Vector2 parentMoved)
        {
            node.wasDragged = node.isDragged;
            var childMoved = EditorTools.DragRect(node.GetRect(gridOffset), currentEvent, ref node.isDragged);
            node.rectPosition += childMoved;
            parentMoved += childMoved;
            MoveNotes(node, childMoved);
            DragNotes(node, gridOffset, currentEvent);

            if (node.wasDragged && !node.isDragged)
            {
                var oldRectPosition = node.rectPosition;
                node.rectPosition = Compute.Round(node.rectPosition, 15f);
                parentMoved += node.rectPosition - oldRectPosition;
                RoundNotes(node);
            }
        }

        public void DragNotes(Node node, Vector2 gridOffset, Event currentEvent)
        {
            for (var i = 0; i < node.message.Count; i++)
            {
                var wasDragged = node.message[i].isDragged;
                var rect = node.message[i].GetRect(gridOffset);
                var move = EditorTools.DragRect(rect, currentEvent, ref node.message[i].isDragged);
                node.message[i].position += move;
                if (rect.ContainsMouseDown())
                {
                    tree.barIndex = 0;
                    tree.showNodeMessage = true;
                    tree.nodeMessage = node.message[i];
                }

                if (rect.ContainsMouseRightDown()) menu.NoteMenu(node, node.message[i]);
                if (wasDragged && !node.message[i].isDragged) RoundNotes(node);
            }
        }

        public void MoveNotes(Node node, Vector2 move)
        {
            for (var i = 0; i < node.message.Count; i++) node.message[i].position += move;
        }

        public void RoundNotes(Node node)
        {
            for (var i = 0; i < node.message.Count; i++)
                node.message[i].position = Compute.Round(node.message[i].position, 15f);
        }

        public void DisconnectChildFromParent(Node node)
        {
            if (!tree.tempChildren.Contains(node))
                tree.tempChildren.Add(node);
            node.nodeParent.Children().Remove(node);
            node.nodeParent.Children().Sort((x, y) => x.rectPosition.x.CompareTo(y.rectPosition.x)); // reorder list
            node.SetParent(null);
        }

        public void ConnectChildToParent(Node node)
        {
            if (!isParentNode.Children().Contains(node))
            {
                if (isParentNode is Decorator &&
                    isParentNode.Children().Count == 1) // Decorators can only  have one child
                    return;
                node.SetParent(isParentNode);
                isParentNode.Children().Add(node);
                tree.tempChildren.Remove(node);
                isParentNode.Children().Sort((x, y) => x.rectPosition.x.CompareTo(y.rectPosition.x)); // reorder list
            }
        }
    }

    [Serializable]
    public class SaveTree
    {
        public AITree tree;
    }
}