using TwoBitMachines.Editors;
using UnityEditor;

namespace TwoBitMachines.FlareEngine.Editors
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(JournalSlot))]
    public class JournalSlotEditor : UnityEditor.Editor
    {
        private JournalSlot main;
        private SerializedObject parent;

        private void OnEnable()
        {
            main = target as JournalSlot;
            parent = serializedObject;
            Layout.Initialize();
        }

        public override void OnInspectorGUI()
        {
            Layout.Update();
            Layout.VerticalSpacing(10);

            parent.Update();

            FoldOut.Box(6, Tint.PurpleDark, 3);
            {
                parent.Field("Icon", "icon");
                parent.Field("Complete", "complete");
                parent.Field("Incomplete", "incomplete");
                parent.Field("Title", "title");
                parent.Field("Extra Info", "extraInfo");
                parent.Field("Description", "description");
            }

            if (FoldOut.FoldOutButton(parent.Get("eventsFoldOut")))
                Fields.EventFoldOut(parent.Get("slotClicked"), parent.Get("clickFoldOut"), "On Clicked",
                    color: Tint.PurpleDark);

            parent.ApplyModifiedProperties();
            Layout.VerticalSpacing(10);
        }
    }
}