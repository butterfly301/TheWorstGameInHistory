#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.Editors
{
    public static class Mouse
    {
        public static bool up;
        public static bool anyUp;
        public static bool alt;
        public static bool ctrl;
        public static bool down;
        public static bool middleDown;
        public static bool holding;
        public static bool dragging;
        public static bool middleDragging;
        public static bool scrolling;
        public static bool rightDown;
        public static bool rightHold;
        public static bool middleHold;
        public static bool wasMiddle;
        public static bool wasUp;
        public static bool wasDragging;
        public static Vector3 position = Vector2.zero;
        public static Vector3 previousPosition = Vector2.zero;
        public static Vector3 delta;
        public static bool moved;
        public static Vector2 mousePosition => position;

        public static void Update()
        {
            moved = false;
            delta = Vector3.zero;
            var guiEvent = Event.current;
            var p = position;
            position = Event.current.mousePosition;
            if (SceneView.currentDrawingSceneView != null && SceneView.currentDrawingSceneView.camera != null)
            {
                previousPosition = p;
                position.y = SceneView.currentDrawingSceneView.camera.pixelHeight - position.y;
                position = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(position);
                if ((previousPosition - position).magnitude > 5)
                    previousPosition = position;
                delta = position - previousPosition;
                if (delta != Vector3.zero)
                    moved = true;
            }

            wasMiddle = middleDown;
            wasUp = anyUp;
            wasDragging = dragging;

            anyUp = guiEvent.type == EventType.MouseUp;
            up = guiEvent.type == EventType.MouseUp && guiEvent.button == 0;
            down = guiEvent.type == EventType.MouseDown && guiEvent.button == 0;
            middleDown = guiEvent.type == EventType.MouseDown && guiEvent.button == 2;
            rightDown = guiEvent.type == EventType.MouseDown && guiEvent.button == 1;
            scrolling = guiEvent.type == EventType.ScrollWheel && guiEvent.button == 2;
            ctrl = guiEvent.modifiers == EventModifiers.Control;
            alt = guiEvent.modifiers == EventModifiers.Alt;
            if (rightDown)
                rightHold = true;
            if (middleDown)
                middleHold = true;
            if (down)
                holding = true;
            if (up)
                dragging = false;
            if (anyUp)
                holding = middleHold = rightHold = dragging = middleDragging = false;

            if (holding && guiEvent.type == EventType.MouseDrag)
                dragging = true;
            if (middleHold && guiEvent.type == EventType.MouseDrag)
                middleDragging = true;
        }

        public static bool ContainsMouse(this Rect rect)
        {
            return rect.Contains(Event.current.mousePosition);
        }

        public static bool ContainsMouseDown(this Rect rect, bool UseEvent = true)
        {
            if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown &&
                Event.current.button == 0)
            {
                if (UseEvent)
                    Layout.UseEvent();
                return true;
            }

            // Debug.Log("False " + rect.Contains(Event.current.mousePosition) + "   " + (Event.current.type == EventType.MouseDown));
            return false;
        }

        public static bool ContainsMouseDrag(this Rect rect)
        {
            if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDrag &&
                Event.current.button == 0)
            {
                GUI.changed = true;
                Event.current.Use();
                return true;
            }

            return false;
        }

        public static bool ContainsMouseDrag(this Rect rect, bool useEvent = true)
        {
            if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDrag &&
                Event.current.button == 0)
            {
                if (useEvent)
                    Layout.UseEvent();
                return true;
            }

            return false;
        }

        public static bool ContainsMouseRightDown(this Rect rect, bool useEvent = true)
        {
            if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown &&
                Event.current.button == 1)
            {
                if (useEvent)
                    Layout.UseEvent();
                return true;
            }

            return false;
        }


        public static bool ContainsMouseMiddleDown(this Rect rect, bool useEvent = true)
        {
            if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown &&
                Event.current.button == 2)
            {
                if (useEvent)
                    Layout.UseEvent();
                return true;
            }

            return false;
        }

        public static bool ContainsScrollWheel(this Rect rect, bool useEvent = true)
        {
            if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.ScrollWheel)
            {
                GUI.changed = true;
                if (useEvent)
                    Event.current.Use();
                return true;
            }

            return false;
        }

        public static bool ContainsScrollWheelDrag(this Rect rect)
        {
            if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDrag &&
                Event.current.button == 2)
            {
                GUI.changed = true;
                Event.current.Use();
                return true;
            }

            return false;
        }

        public static bool ContainsMouseUp(this Rect rect, bool UseEvent = true)
        {
            if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseUp &&
                Event.current.button == 0)
            {
                if (UseEvent)
                    Layout.UseEvent();
                return true;
            }

            return false;
        }

        public static int MouseDeltaSign()
        {
            return (int)Mathf.Sign(Event.current.delta.y);
        }

        public static bool MouseDown(bool useEvent = false)
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                if (useEvent)
                    Layout.UseEvent();
                return true;
            }

            return false;
        }

        public static bool MouseUp(bool useEvent = false)
        {
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                if (useEvent)
                    Layout.UseEvent();
                return true;
            }

            return false;
        }

        public static bool MouseMiddleDown(bool useEvent = false)
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 2)
            {
                if (useEvent)
                    Layout.UseEvent();
                return true;
            }

            return false;
        }

        public static bool MouseDrag(bool useEvent = true)
        {
            if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
            {
                GUI.changed = true;
                if (useEvent)
                    Event.current.Use();
                return true;
            }

            return false;
        }

        public static bool MouseRightDown()
        {
            return Event.current.type == EventType.MouseDown && Event.current.button == 1;
        }

        public static bool EventScrollWheel(bool useEvent = false)
        {
            if (Event.current.type == EventType.ScrollWheel)
            {
                if (useEvent)
                    Layout.UseEvent();
                return true;
            }

            return false;
        }
    }
}
#endif