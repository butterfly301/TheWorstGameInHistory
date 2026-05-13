using TwoBitMachines.Editors;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Editors
{
    [CustomEditor(typeof(ReactionProfile))]
    public class ReactionProfileEditor : UnityEditor.Editor
    {
        public static int reactionListSize;
        public static string[] reactionList;
        public ReactionProfile main;
        public SerializedObject parent;

        private void OnEnable()
        {
            main = target as ReactionProfile;
            parent = serializedObject;
            Layout.Initialize();

            if (reactionList == null || reactionList.Length == 0 || reactionListSize != reactionList.Length)
            {
                reactionList = Util.GetFileNames("TwoBitMachines", "/FlareEngine/Scripts/Effects/Reactions/Reactions");
                reactionListSize = reactionList.Length;
            }

            for (var i = main.reaction.Count - 1; i >= 0; i--)
                if (main.reaction[i] == null)
                    main.reaction.RemoveAt(i);
                else
                    main.reaction[i].hideFlags = HideFlags.HideInInspector;
        }

        public override void OnInspectorGUI()
        {
            Layout.Update();

            parent.Update();
            {
                var reactions = parent.Get("reaction");
                var barColor = Tint.Orange;
                var labelColor = Tint.WarmWhite;

                for (var i = 0; i < reactions.arraySize; i++)
                {
                    var obj = reactions.Element(i).objectReferenceValue;
                    if (obj == null)
                        continue;
                    var reaction = (ReactionBehaviour)obj;
                    var reactionObj = new SerializedObject(reactions.Element(i).objectReferenceValue);

                    if (reaction != null)
                        reaction.hideFlags = HideFlags.HideInInspector;
                    reactionObj.Update();
                    if (!reaction.OnInspector(reactionObj, barColor, labelColor))
                        if (ReactionBehaviour.Open(reactionObj, Util.ToProperCase(reaction.GetType().Name), barColor,
                                labelColor))
                        {
                            var fields = EditorTools.CountObjectFields(reactionObj);
                            if (fields != 0)
                                FoldOut.Box(fields, FoldOut.boxColorLight, offsetY: -2);
                            EditorTools.IterateObject(reactionObj, fields);
                            if (fields == 0)
                                Layout.VerticalSpacing(3);
                        }

                    ListReorder.Grip(parent, reactions, Bar.barStart.CenterRectHeight(), i, Tint.WarmWhite);
                    if (reactionObj.Bool("delete"))
                    {
                        DestroyImmediate(obj);
                        reactions.DeleteArrayElement(i);
                        return;
                    }

                    reactionObj.ApplyModifiedProperties();
                }
            }
            parent.ApplyModifiedProperties();
            if (FoldOut.CornerButton(Tint.Blue) && reactionList != null && reactionList.Length > 0)
            {
                var menu = new GenericMenu();
                for (var i = 0; i < reactionList.Length; i++)
                    menu.AddItem(new GUIContent(reactionList[i]), false, CreateReaction, reactionList[i]);
                menu.ShowAsContext();
            }
        }

        private void CreateReaction(object obj)
        {
            var typeName = (string)obj;
            var fullTypeName = "TwoBitMachines.FlareEngine." + typeName;
            var type = EditorTools.RetrieveType(fullTypeName);
            if (type == null)
            {
                Debug.LogWarning("FlareEngine: Reaction type '" + typeName + "' not found.");
                return;
            }

            var comp = main.transform.gameObject.AddComponent(type);
            comp.hideFlags = HideFlags.HideInInspector;
            var newAbility = (ReactionBehaviour)comp;

            parent.Update();
            var reaction = parent.Get("reaction");
            reaction.arraySize++;
            reaction.LastElement().objectReferenceValue = (ReactionBehaviour)comp;
            parent.ApplyModifiedProperties();
        }
    }
}