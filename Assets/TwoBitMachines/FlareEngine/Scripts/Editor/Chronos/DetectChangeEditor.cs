using TwoBitMachines.Editors;
using TwoBitMachines.FlareEngine.Timeline;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Editors
{
    public class DetectChangeEditor
    {
        public static void ViewState(FieldChange fieldChange, Track track)
        {
            if (fieldChange.component == null || fieldChange.value == null || fieldChange.valueString == "") return;

            if (fieldChange.dataType == FieldDataType.Field)
            {
                var fieldInfo = fieldChange.component.GetType().GetField(fieldChange.fieldName);
                if (fieldInfo != null)
                {
                    if (!track.restoreSet)
                        track.SetRestoreState(fieldChange.extraType, fieldInfo.GetValue(fieldChange.component));
                    fieldInfo.SetValue(fieldChange.component, fieldChange.value);
                }
            }
            else
            {
                var propertyInfo = fieldChange.component.GetType().GetProperty(fieldChange.fieldName);
                if (propertyInfo != null)
                {
                    if (!track.restoreSet)
                        track.SetRestoreState(fieldChange.extraType, propertyInfo.GetValue(fieldChange.component));
                    propertyInfo.SetValue(fieldChange.component, fieldChange.value);
                }
            }
        }

        public static void RestoreState(FieldChange fieldChange, Track track)
        {
            if (fieldChange.component == null || track.restoreValue == null || track.restoreString == "" ||
                !track.restoreSet) return;
            if (fieldChange.dataType == FieldDataType.Field)
            {
                var fieldInfo = fieldChange.component.GetType().GetField(fieldChange.fieldName);
                if (fieldInfo != null)
                {
                    fieldInfo.SetValue(fieldChange.component, track.restoreValue);
                    track.restoreSet = false;
                }
            }
            else
            {
                var propertyInfo = fieldChange.component.GetType().GetProperty(fieldChange.fieldName);
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(fieldChange.component, track.restoreValue);
                    track.restoreSet = false;
                }
            }
        }

        public static void RecordState(Chronos main, ChronosVariables info)
        {
            for (var i = 0; i < main.track.Count; i++)
            for (var j = 0; j < main.track[i].action.Count; j++)
            {
                var track = main.track[i];
                var action = track.action[j];

                if (Application.isPlaying)
                {
                    if (track.restoreSet) RestoreState(action.fieldChange, track);
                    continue;
                }

                if (info.restoreState)
                    if (info.trackKey == 1000000 * (i + 1))
                    {
                        info.restoreState = false;
                        RestoreState(action.fieldChange, track);
                    }

                var key = 1000000 * (i + 1) + 10 * (j + 1);
                if (key == main.currentActionKey)
                {
                    if (track.gameObject != null)
                    {
                        if (main.record && info.actionPressed)
                        {
                            main.record = false;
                            main.recordState.RevertToOrigin();
                        }

                        var color = main.record ? Tint.Delete : Tint.SoftDark;
                        var label = main.record ? "Recording" : "Record";
                        if (FoldOut.LargeButton(label, color, Tint.WarmWhite, Icon.Get("BackgroundLight")))
                        {
                            if (track.restoreSet) RestoreState(action.fieldChange, track);
                            if (main.record)
                            {
                                main.recordState.RecordChange(action.fieldChange);
                                main.recordState.RevertToOrigin();
                            }

                            main.record = !main.record;
                            if (main.record) main.recordState.RecordObjectOrigin(track.gameObject);
                        }

                        if (!main.record && info.actionPressed) ViewState(action.fieldChange, track);
                    }

                    break;
                }
            }
        }
    }
}