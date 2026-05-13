using TwoBitMachines.Editors;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Editors
{
    [CustomEditor(typeof(QuestSO), true)]
    public class QuestSOEditor : UnityEditor.Editor
    {
        public static string inputName = " Name";
        private QuestSO main;
        private SerializedObject parent;

        private void OnEnable()
        {
            main = target as QuestSO;
            parent = serializedObject;
            Layout.Initialize();
        }

        public override void OnInspectorGUI()
        {
            Layout.Update();
            Layout.VerticalSpacing(10);
            parent.Update();

            var questSO = main;
            var open = parent.Bool("foldOut");

            FoldOut.Bar(parent, Tint.Orange)
                .Label(questSO.title, Color.white)
                .RightButton(toolTip: "Add Reward", execute: open)
                .RightButton("deleteData", "Delete", "Delete Saved Data", execute: open);

            if (parent.ReadBool("deleteData")) WorldManagerEditor.DeleteSavedData(questSO.title);
            if (parent.ReadBool("delete") && questSO != null)
            {
                var assetPath = AssetDatabase.GetAssetPath(questSO);
                AssetDatabase.DeleteAsset(assetPath);
                DestroyImmediate(questSO, true);
                return;
            }

            FoldOut.Box(3, FoldOut.boxColor, offsetY: -2);
            {
                if (parent.FieldAndButton("Title", "title", "Sort", toolTip: "Update Name"))
                {
                    var assetPath = AssetDatabase.GetAssetPath(questSO.GetInstanceID());
                    AssetDatabase.RenameAsset(assetPath, parent.String("title"));
                    AssetDatabase.SaveAssets();
                    EditorUtility.SetDirty(questSO);
                }

                parent.Field("Icon", "icon");
                parent.Field("Goal", "goal");
            }
            Layout.VerticalSpacing(3);

            if (FoldOut.FoldOutBoxButton(parent.Get("descriptionFoldOut"), "Description", FoldOut.boxColor))
            {
                var description = parent.Get("description");
                var rect = Layout.CreateRect(Layout.longInfoWidth, 150, -11, -1);
                description.stringValue = GUI.TextArea(rect, description.stringValue);
            }

            if (FoldOut.FoldOutBoxButton(parent.Get("extraInfoFoldOut"), "Extra Info", FoldOut.boxColor))
            {
                var description = parent.Get("extraInfo");
                var rect = Layout.CreateRect(Layout.longInfoWidth, 150, -11, -1);
                description.stringValue = GUI.TextArea(rect, description.stringValue);
            }

            var rewards = parent.Get("rewards");

            if (parent.ReadBool("add"))
            {
                rewards.arraySize++;
                rewards.LastElement().Get("name").stringValue = "Reward Name";
            }

            for (var r = 0; r < rewards.arraySize; r++)
            {
                var reward = rewards.Element(r);

                FoldOut.Box(3, Tint.BoxTwo);
                {
                    if (reward.FieldAndButton("Reward", "reward", "Delete"))
                    {
                        rewards.DeleteArrayElement(r);
                        break;
                    }

                    reward.Field("Name", "name");
                    reward.Field("Reward Icon", "icon");
                }
                Layout.VerticalSpacing(5);

                FoldOut.Box(2, Tint.BoxTwo, offsetY: -2);
                {
                    reward.Field("For World Float", "worldFloat");
                    reward.FieldDouble("For Inventory", "inventorySO", "itemSO");
                }
                Layout.VerticalSpacing(3);
            }

            parent.ApplyModifiedProperties();
        }
    }
}