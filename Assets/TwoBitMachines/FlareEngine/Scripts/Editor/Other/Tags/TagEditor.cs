using TwoBitMachines.Editors;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Editors
{
    [CustomEditor(typeof(FlareTag))]
    [CanEditMultipleObjects]
    public class TagEditor : Editor
    {
        public static bool initialize = false;
        private FlareTag main;
        private SerializedObject so;

        private void OnEnable()
        {
            main = target as FlareTag;
            if (serializedObject != null) so = serializedObject;
            Layout.Initialize();

            if (main.tagListSO == null)
            {
                var guids = AssetDatabase.FindAssets("t:TagListSO");
                if (guids.Length > 0)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    main.tagListSO = AssetDatabase.LoadAssetAtPath<TagListSO>(path);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            if (so == null)
                return;
            Layout.Update();
            Layout.VerticalSpacing(10);
            so.Update();


            var tags = so.Get("tags");
            FoldOut.BoxSingle(1, Tint.Blue);
            {
                if (Labels.LabelAndButton("Add Tag", "Add") && main.tagListSO != null)
                {
                    var menu = new GenericMenu();
                    for (var j = 0; j < main.tagListSO.tags.Count; j++)
                    {
                        var tagName = main.tagListSO.tags[j];
                        menu.AddItem(new GUIContent(tagName), false, () =>
                        {
                            tags.serializedObject.Update();
                            tags.arraySize++;
                            tags.LastElement().stringValue = tagName;
                            tags.serializedObject.ApplyModifiedProperties();
                        });
                    }

                    menu.ShowAsContext();
                }
            }
            Layout.VerticalSpacing(2);

            if (tags.arraySize > 0)
            {
                FoldOut.Box(tags.arraySize, Tint.SlateGrey, offsetY: -2);
                for (var i = 0; i < tags.arraySize; i++)
                    if (Labels.LabelAndButton(tags.Element(i).stringValue, "Delete"))
                        tags.DeleteArrayElement(i);

                Layout.VerticalSpacing(3);
            }

            Layout.VerticalSpacing(5);
            so.ApplyModifiedProperties();

            if (main.tagListSO != null) UserTags(new SerializedObject(main.tagListSO));

            Layout.VerticalSpacing(10);
        }

        public void UserTags(SerializedObject so)
        {
            so.Update();
            if (Block.Header(so).Style(Tint.Box).Fold("Create User Tag", "editorOpen").Build())
            {
                var array = so.Get("tags");
                for (var i = 0; i < array.arraySize; i++)
                    Block.Header(array.Element(i)).Style(Tint.Box, height: 20)
                        .Grip(this, array, i)
                        .Field(array.Element(i))
                        .ArrayButtons()
                        .BuildGet()
                        .ReadArrayButtons(array, i);
                ;
            }

            so.ApplyModifiedProperties();
        }
    }
}