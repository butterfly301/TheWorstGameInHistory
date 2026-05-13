using TwoBitMachines.Editors;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Editors
{
    [CustomEditor(typeof(ItemSO))]
    public class ItemSOEditor : UnityEditor.Editor
    {
        public static string inputName = " Name";
        private ItemSO main;
        private SerializedObject parent;

        private void OnEnable()
        {
            main = target as ItemSO;
            parent = serializedObject;
            Layout.Initialize();
        }

        public override void OnInspectorGUI()
        {
            Layout.Update();
            Layout.VerticalSpacing(10);
            parent.Update();
            var itemSO = main;
            var open = parent.Bool("foldOut");

            FoldOut.Bar(parent, Tint.Orange)
                .Label(itemSO.itemName, Color.white)
                .RightButton("deleteData", "Delete", "Delete Saved Data", execute: open);

            if (parent.ReadBool("deleteData")) WorldManagerEditor.DeleteSavedData(itemSO.itemName);
            if (parent.ReadBool("delete") && itemSO != null)
            {
                var assetPath = AssetDatabase.GetAssetPath(itemSO);
                AssetDatabase.DeleteAsset(assetPath);
                DestroyImmediate(itemSO, true);
                return;
            }

            var droppable = parent.Enum("droppable");

            FoldOut.Box(6, FoldOut.boxColor, offsetY: -2);
            {
                if (parent.FieldAndButton("Name", "itemName", "Sort", toolTip: "Update Name"))
                {
                    var assetPath = AssetDatabase.GetAssetPath(itemSO.GetInstanceID());
                    AssetDatabase.RenameAsset(assetPath, parent.String("itemName"));
                    AssetDatabase.SaveAssets();
                    EditorUtility.SetDirty(itemSO);
                }

                parent.Field("Key Name", "keyName");
                parent.Field("Icon", "icon");
                parent.Field("For Inventory", "forInventory");
                parent.Field("Droppable", "droppable", droppable == 0);
                parent.FieldDouble("Droppable", "droppable", "prefab", droppable == 1);
                parent.FieldAndEnable("Consumable", "stackLimit", "consumable");
                Labels.FieldText("Stack Limit", 17);
            }
            Layout.VerticalSpacing(3);

            FoldOut.Box(3, FoldOut.boxColor);
            {
                parent.Field("Generic Float", "genericFloat");
                parent.Field("Generic String", "genericString");
                parent.FieldDouble("Cost", "cost", "vendorItem");
            }
            Layout.VerticalSpacing(5);

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

            parent.ApplyModifiedProperties();
        }
    }
}