#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.Editors
{
    public static class Slider
    {
        public static float barWidth = 10;

        public static bool Set(Color barColor, Color backgroundColor, SerializedProperty value, float min = 0,
            float max = 10, bool noGap = false)
        {
            if (value == null)
                return false;
            var controlID = GUIUtility.GetControlID(1001, FocusType.Passive);
            var baseRect = Block.BasicRect(Layout.longInfoWidth, 25, -11, noGap, 1, Icon.Get("HeaderMiddle"),
                backgroundColor);
            baseRect = baseRect.Offset(10, 10, -20, -20);
            var sliderRect = new Rect(baseRect) { y = baseRect.y - baseRect.height, height = baseRect.height * 3 };
            var previousValue = value.floatValue;

            switch (Event.current.GetTypeForControl(controlID))
            {
                case EventType.Repaint:
                    var position = value.floatValue / max * (baseRect.width - barWidth);
                    var knob = new Rect(sliderRect) { x = baseRect.x + position, width = barWidth };
                    position = Mathf.Clamp(position, barWidth, baseRect.width);
                    var topBar = new Rect(baseRect) { width = position };
                    Skin.Draw(baseRect, Tint.WhiteOpacity100, Skin.square); // bottom Bar
                    Skin.Draw(topBar, barColor, Skin.square); //               top bar
                    Skin.Draw(knob, barColor, Skin.square); //                 knob
                    break;
                case EventType.MouseDown:
                    if (sliderRect.Contains(Event.current.mousePosition) && Event.current.button == 0)
                        GUIUtility.hotControl = controlID;
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                        GUIUtility.hotControl = 0;
                    break;
            }

            if (Event.current.isMouse && GUIUtility.hotControl == controlID)
            {
                var relativeX = Event.current.mousePosition.x - sliderRect.x;
                value.floatValue = Mathf.Clamp(relativeX / sliderRect.width * max, min, max);
                value.floatValue = Compute.Round(value.floatValue, 0.25f);
                GUI.changed = true;
                Event.current.Use();
            }

            return previousValue != value.floatValue;
        }
    }
}
#endif