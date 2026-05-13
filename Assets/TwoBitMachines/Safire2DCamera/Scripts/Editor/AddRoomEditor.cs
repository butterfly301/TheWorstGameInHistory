using TwoBitMachines.Editors;
using UnityEditor;

namespace TwoBitMachines.Safire2DCamera.Editors
{
    [CustomEditor(typeof(AddRoom))]
    public class AddRoomEditor : UnityEditor.Editor
    {
        private SerializedObject parent;
        private AddRoom room;

        private void OnEnable()
        {
            room = target as AddRoom;
            parent = serializedObject;
            Layout.Initialize();
        }

        private void OnSceneGUI()
        {
            room.room.bounds.position = room.transform.position;
            Rooms.DrawRooms(room.room, null, -1, true);
        }

        public override void OnInspectorGUI()
        {
            Layout.Update();
            Layout.VerticalSpacing(10);

            parent.Update();
            {
                var room = parent.Get("room");
                FoldOut.Box(1, FoldOut.boxColor);
                {
                    parent.Field("Safire Camera", "safireCamera");
                }
                Layout.VerticalSpacing(5);
                Rooms.CustomInspectorRoom(room, null, 0);
            }
            parent.ApplyModifiedProperties();
        }

        [DrawGizmo(GizmoType.NonSelected | GizmoType.NotInSelectionHierarchy)]
        private static void DrawWhenObjectIsNotSelected(AddRoom room, GizmoType gizmoType)
        {
            SceneTools.blockHandles = true;
            room.room.bounds.position = room.transform.position;
            Rooms.DrawRooms(room.room, null, -1, true);
            SceneTools.blockHandles = false;
        }
    }
}