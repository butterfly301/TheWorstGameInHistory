using TwoBitMachines.Editors;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Editors
{
    public class ChronosVariables
    {
        public SerializedProperty action;
        public SerializedProperty actionArray;
        public bool actionWasPressed;
        public SerializedProperty currentActionKey;

        ///// Remember
        public SerializedProperty currentTrackKey;
        public bool initOffset;
        public SerializedProperty maxContentLength;
        public float moveOffsetX;
        public SerializedProperty oldAction;
        public bool restoreState;
        public bool sameAction;

        public SerializedProperty scrollPosition;

        //// no need to serialize
        public float sideBar = 50f;
        public float timeRate = 50f; // 50 pixels = 10 seconds, 5 pixels per second
        public float timeSeconds = 10f;
        public SerializedProperty timeZoom;
        public SerializedProperty track;
        public float time => timeRate + timeZoom.floatValue;
        public float timeConvert => timeRate / timeSeconds;

        public bool actionPressed
        {
            get
            {
                var pressed = actionWasPressed;
                actionWasPressed = false;
                return pressed;
            }
            set => actionWasPressed = value;
        }

        public int actionKey
        {
            get => currentActionKey.intValue;
            set => currentActionKey.intValue = value;
        }

        public int trackKey
        {
            get => currentTrackKey.intValue;
            set => currentTrackKey.intValue = value;
        }

        public float zoom
        {
            get => timeZoom.floatValue;
            set => timeZoom.floatValue = value;
        }

        public float scrollX
        {
            get => scrollPosition.vector2Value.x;
            set => scrollPosition.vector2Value = new Vector2(value, scrollPosition.vector2Value.y);
        }

        public float scrollY
        {
            get => scrollPosition.vector2Value.y;
            set => scrollPosition.vector2Value = new Vector2(scrollPosition.vector2Value.x, value);
        }

        public float contentLengthX
        {
            get => maxContentLength.vector2Value.x;
            set => maxContentLength.vector2Value = new Vector2(value, maxContentLength.vector2Value.y);
        }

        public float contentLengthY
        {
            get => maxContentLength.vector2Value.y;
            set => maxContentLength.vector2Value = new Vector2(maxContentLength.vector2Value.x, value);
        }


        public void Initialize(SerializedObject parent)
        {
            track = parent.Get("trackRef");
            action = parent.Get("action");
            timeZoom = parent.Get("timeZoom");
            actionArray = parent.Get("actionArray");
            currentTrackKey = parent.Get("currentTrackKey");
            currentActionKey = parent.Get("currentActionKey");
            scrollPosition = parent.Get("scrollPosition");
            maxContentLength = parent.Get("maxContentLength");
        }
    }
}