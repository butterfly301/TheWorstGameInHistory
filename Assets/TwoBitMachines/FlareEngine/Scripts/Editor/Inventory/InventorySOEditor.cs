using TwoBitMachines.Editors;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Editors
{
    [CustomEditor(typeof(InventorySO), true)]
    public class InventorySOEditor : UnityEditor.Editor
    {
        public static string inputName = " Name";
        private InventorySO main;
        private SerializedObject parent;

        private void OnEnable()
        {
            main = target as InventorySO;
            parent = serializedObject;
            Layout.Initialize();
        }

        public override void OnInspectorGUI()
        {
            Layout.Update();
            Layout.VerticalSpacing(10);
            parent.Update();

            var array = parent.Get("referenceInventory");
            var arrayDefault = parent.Get("defaultItems");

            if (Fields.InputAndButtonBox("Create New Item", "Add", Tint.Blue, ref inputName))
            {
                CreateScriptableObject(array, inputName);
                inputName = "Item Name";
            }

            if (FoldOut.Bar(parent, Tint.Blue).Label("Default Items", Color.black, false).FoldOut("defaultFoldOut"))
                arrayDefault.ArrayBox("Default Item", FoldOut.boxColor, -2);

            for (var i = 0; i < array.arraySize; i++)
            {
                var element = array.Element(i);

                if (element.objectReferenceValue == null)
                {
                    array.DeleteArrayElement(i);
                    break;
                }

                var newObj = new SerializedObject(element.objectReferenceValue);
                newObj.Update();
                var itemSO = (ItemSO)element.objectReferenceValue;
                var open = newObj.Bool("foldOut");
                var deleteAsk = newObj.Bool("deleteAsk");

                if (
                    FoldOut.Bar(newObj, Tint.Orange, 0)
                    .Grip(parent, array, i, color: Tint.WarmWhite)
                    .Label(itemSO.itemName, Color.white)
                    .RightButton("remove", "X", "Remove From Inventory", execute: open)
                    .RightButton("deleteData", "Delete", "Delete Saved Data", execute: open)
                    .RightButton("deleteAsk", "Delete", on: Tint.Delete, off: Tint.Delete, toolTip: "Delete Item",
                        execute: open && !deleteAsk)
                    .RightButton("deleteAsk", "Close", "Return", execute: open && deleteAsk)
                    .RightButton("delete", "Yes", "Delete", execute: open && deleteAsk)
                    .FoldOut())
                {
                    if (newObj.ReadBool("deleteData")) WorldManagerEditor.DeleteSavedData(itemSO.itemName);
                    if (newObj.ReadBool("delete") && itemSO != null)
                    {
                        var assetPath = AssetDatabase.GetAssetPath(itemSO);
                        AssetDatabase.DeleteAsset(assetPath);
                        DestroyImmediate(itemSO, true);
                        return;
                    }

                    if (newObj.ReadBool("remove") && itemSO != null)
                    {
                        array.DeleteArrayElement(i);
                        break;
                    }

                    var droppable = newObj.Enum("droppable");

                    FoldOut.Box(6, FoldOut.boxColor, offsetY: -2);
                    {
                        if (newObj.FieldAndButton("Name", "itemName", "Sort", toolTip: "Update Name"))
                        {
                            var assetPath = AssetDatabase.GetAssetPath(itemSO.GetInstanceID());
                            AssetDatabase.RenameAsset(assetPath, newObj.String("itemName"));
                            AssetDatabase.SaveAssets();
                            EditorUtility.SetDirty(itemSO);
                        }

                        newObj.Field("Key Name", "keyName");
                        newObj.Field("Icon", "icon");
                        newObj.Field("For Inventory", "forInventory");
                        newObj.Field("Droppable", "droppable", droppable == 0);
                        newObj.FieldDouble("Droppable", "droppable", "prefab", droppable == 1);
                        newObj.FieldAndEnable("Consumable", "stackLimit", "consumable");
                        Labels.FieldText("Stack Limit", 17);
                    }
                    Layout.VerticalSpacing(3);

                    FoldOut.Box(3, FoldOut.boxColor);
                    {
                        newObj.Field("Generic Float", "genericFloat");
                        newObj.Field("Generic String", "genericString");
                        newObj.FieldDouble("Cost", "cost", "vendorItem");
                    }
                    Layout.VerticalSpacing(5);

                    if (FoldOut.FoldOutBoxButton(newObj.Get("descriptionFoldOut"), "Description", FoldOut.boxColor))
                    {
                        var description = newObj.Get("description");
                        var rect = Layout.CreateRect(Layout.longInfoWidth, 150, -11, -1);
                        description.stringValue = GUI.TextArea(rect, description.stringValue);
                    }

                    if (FoldOut.FoldOutBoxButton(newObj.Get("extraInfoFoldOut"), "Extra Info", FoldOut.boxColor))
                    {
                        var description = newObj.Get("extraInfo");
                        var rect = Layout.CreateRect(Layout.longInfoWidth, 150, -11, -1);
                        description.stringValue = GUI.TextArea(rect, description.stringValue);
                    }
                }

                newObj.ApplyModifiedProperties();
            }

            DragAndDropArea(array);

            parent.ApplyModifiedProperties();
            Layout.VerticalSpacing(10);
        }

        private void DragAndDropArea(SerializedProperty array)
        {
            var color = Tint.Blue;
            var dropArea =
                Layout.CreateRect(Layout.longInfoWidth, 27,
                    -11); //, texture : Icon.Get ("BackgroundLight"), color : Color.clear);
            Labels.Centered(dropArea, "-- Drag Existing Item Here --", Tint.BoxTwo, 13);
            Fields.DropAreaGUI<ItemSO>(dropArea, array);
        }

        public void CreateScriptableObject(SerializedProperty array, string name)
        {
            var path = "Assets/TwoBitMachines/FlareEngine/AssetsFolder/Inventory/ItemsSO/" + name + ".asset";
            var newSO = AssetDatabase.LoadAssetAtPath(path, typeof(ItemSO)) as ItemSO;
            if (newSO != null)
            {
                Debug.LogWarning("Scriptable Object with name " + name + " already exists.");
                return;
            }

            var asset = CreateInstance<ItemSO>();
            asset.name = name;
            asset.itemName = name;
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();

            array.arraySize++;
            array.LastElement().objectReferenceValue = asset;
        }
    }
}