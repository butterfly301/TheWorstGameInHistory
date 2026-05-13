using TwoBitMachines.Editors;
using UnityEditor;

namespace TwoBitMachines.FlareEngine.Editors
{
    [CustomEditor(typeof(TerrainEvents))]
    public class TerrainEventsEditor : Editor
    {
        public static bool initialize = false;
        private TerrainEvents main;
        private SerializedObject so;

        private void OnEnable()
        {
            main = target as TerrainEvents;
            so = serializedObject;
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
            Layout.Update();
            Layout.VerticalSpacing(10);
            so.Update();

            if (main.tagListSO != null)
            {
                Terrain(so.Get("terrainGround"));
                Terrain(so.Get("terrainJumped"));
                Terrain(so.Get("terrainCeiling"));
                Terrain(so.Get("terrainWalls"));
            }

            Layout.VerticalSpacing(5);
            so.ApplyModifiedProperties();
            Layout.VerticalSpacing(10);
        }

        public void Terrain(SerializedProperty property)
        {
            if (Block.Header(property).Style(Tint.SlateGrey)
                .Fold(property.String("name"), bold: true, color: Tint.White).Button().Build())
            {
                var events = property.Get("events");
                if (Header.SignalActive("Add") || events.arraySize == 0) events.CreateNewElement();
                for (var i = 0; i < events.arraySize; i++)
                {
                    var element = events.Element(i);
                    if (Block.Header(element).DropArrow().Style(Tint.Box).DropList("name", main.tagListSO.tags)
                        .Button("Delete").Build())
                    {
                        if (Header.SignalActive("Delete"))
                        {
                            events.DeleteArrayElement(i);
                            return;
                        }

                        if (element.Bool("foldOut")) Fields.EventField(element.Get("onEvent"), noGap: true);
                    }
                }
            }
        }
    }
}