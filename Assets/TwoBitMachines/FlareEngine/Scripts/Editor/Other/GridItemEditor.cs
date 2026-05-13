using System;
using TwoBitMachines.Editors;
using UnityEditor;

namespace TwoBitMachines.FlareEngine.Editors
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GridItem), true)]
    public class GridItemEditor : UnityEditor.Editor
    {
        private GridItem main;
        private SerializedObject parent;

        private void OnEnable()
        {
            parent = serializedObject;
            Layout.Initialize();

            main = target as GridItem;
            if (main.ID == "" || main.ID == null) main.ID = Guid.NewGuid().ToString();
        }

        public override void OnInspectorGUI()
        {
            Layout.Update();
            Layout.VerticalSpacing(10);
            parent.Update();
            {
                FoldOut.Box(3, Tint.PurpleDark, 3);
                {
                    parent.FieldToggle("Grid Element", "gridElement");
                    parent.FieldAndEnable("Must Pay", "cost", "mustPay");
                    Labels.FieldText("Cost", 18);
                    parent.FieldToggleAndEnable("Save", "save");
                }

                if (FoldOut.FoldOutButton(parent.Get("eventsFoldOut")))
                {
                    Fields.EventFoldOutEffect(parent.Get("onSelect"), parent.Get("selectWE"),
                        parent.Get("selectFoldOut"), "On Select", FoldOut.boxColorLight);
                    Fields.EventFoldOutEffect(parent.Get("onFocus"), parent.Get("onFocusWE"),
                        parent.Get("onFocusFoldOut"), "On Focus", FoldOut.boxColorLight);
                    Fields.EventFoldOutEffect(parent.Get("onOutOfFocus"), parent.Get("onOutOfFocusWE"),
                        parent.Get("onOutOfFocusFoldOut"), "On Out Of Focus", FoldOut.boxColorLight);
                    Fields.EventFoldOutEffect(parent.Get("purchaseFailed"), parent.Get("purchaseFailedWE"),
                        parent.Get("purchaseFailedFoldOut"), "Purchase Failed", FoldOut.boxColorLight);
                    Fields.EventFoldOutEffect(parent.Get("purchaseSuccess"), parent.Get("purchaseSuccessWE"),
                        parent.Get("purchaseSuccessFoldOut"), "Purchase Success", FoldOut.boxColorLight);
                    Fields.EventFoldOutEffect(parent.Get("cantPurchase"), parent.Get("cantPurchaseWE"),
                        parent.Get("cantPurchaseFoldOut"), "Already Purchased", FoldOut.boxColorLight);
                    Fields.EventFoldOutEffect(parent.Get("onHasBeenPurchased"), parent.Get("onHasBeenPurchasedWE"),
                        parent.Get("onHasBeenPurchasedFoldOut"), "Has Been Purchased", FoldOut.boxColorLight);
                }

                EditorGUILayout.LabelField("ID: " + main.ID);
            }
            parent.ApplyModifiedProperties();
            Layout.VerticalSpacing(10);
        }
    }
}