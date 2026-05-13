using TwoBitMachines.Editors;
using TwoBitMachines.FlareEngine.Timeline;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Editors
{
    public class ActionWindowEditor
    {
        public float contentOffsetXRef;
        private ChronosEditor editor;
        private ChronosVariables info;
        private Chronos main;
        public Vector2 mousePositionRef;
        private SerializedObject parent;

        public void Initialize(ChronosEditor chronosEditor)
        {
            editor = chronosEditor;
            info = chronosEditor.info;
            main = chronosEditor.main;
            parent = chronosEditor.parent;
        }

        public void Execute(Rect window)
        {
            GUI.BeginGroup(window); // everything starts at 0,0 and is clipped 

            window.x = window.y = 1;
            window.DrawRect(Tint.BoxLight);
            var contentOffsetX = ScrollBar.ContentOffset(30f, info.scrollX, window.width, info.contentLengthX);
            var contentOffsetY = ScrollBar.ContentOffset(30f, info.scrollY, window.height, info.contentLengthY);

            info.contentLengthX = 0;
            var trackArray = parent.Get("track");

            Draw.GLStart(main.lineMaterial);

            float step = 23;
            var positionY = -contentOffsetY - step;
            var isPlaying = Application.isPlaying;

            for (var i = 0; i < trackArray.arraySize; i++)
            {
                positionY += step;
                var trackKey = 1000000 * (i + 1);
                var track = trackArray.Element(i);
                var actionArray = track.Get("action");

                var row = new Rect(0, positionY + 1, window.width, 21f);
                if (row.ContainsMouseDown(false)) info.trackKey = trackKey;
                if (info.trackKey == trackKey) EditorGUI.DrawRect(row, Tint.WhiteOpacity100);
                for (var j = 0; j < actionArray.arraySize; j++)
                {
                    var actionKey = trackKey + 10 * (j + 1);
                    var action = actionArray.Element(j);
                    var startTime = action.Get("startTime");
                    var endTime = action.Get("endTime");
                    var type = (ActionType)action.Enum("type");


                    var setType = type == ActionType.Set;
                    var position = startTime.floatValue * info.timeConvert;

                    // convert time duration to pixel size
                    var duration = Compute.Round(endTime.floatValue - startTime.floatValue, 0.01f);
                    var pixelLength =
                        duration * info.timeConvert; //                               normal pixel size, no zoom
                    var fullPixelLength =
                        setType ? 5f : pixelLength + info.zoom * (pixelLength / info.timeRate); //  include zoom

                    var positionP = new Vector2(position, 0);
                    positionP +=
                        Vector2.right *
                        (position / info.timeRate * info.zoom); //   zoom offset, timeRate base value, needs a constant
                    positionP -= Vector2.right * contentOffsetX; //                                       content offset
                    positionP += Vector2.up * (positionY + 1); //                                        row

                    var max = positionP.x + contentOffsetX + fullPixelLength + 50;
                    info.contentLengthX = max >= info.contentLengthX ? max : info.contentLengthX;

                    var bar = new Rect(positionP, new Vector2(fullPixelLength, 21f));
                    var handle = new Rect(bar) { x = bar.x - 1, y = bar.y, width = 2, height = bar.height };
                    var left = new Rect(bar) { x = bar.x - 1, y = 0, width = 6, height = bar.y + bar.height };
                    var right = new Rect(bar)
                        { x = bar.x + bar.width - 3, y = 0, width = 6, height = bar.y + bar.height };

                    //   DrawLine(new Vector2(position.floatValue , 0) , new Vector2(position.floatValue , 0) + Vector2.up * 23);

                    if (info.actionKey == actionKey && bar.ContainsMouseRightDown())
                    {
                        mousePositionRef = Event.current.mousePosition;
                        contentOffsetXRef = contentOffsetX;
                        FullActionMenu(trackKey, actionKey);
                    }

                    if (info.actionKey == actionKey && !setType &&
                        (left.ContainsMouseDown(false) || right.ContainsMouseDown(false)))
                    {
                        if (left.ContainsMouseDown(false))
                            parent.SetTrue("dragLeft");
                        else if (right.ContainsMouseDown(false)) parent.SetTrue("dragRight");
                        parent.SetTrue("dragContent");
                        info.initOffset = true;
                        info.moveOffsetX = position;
                    }

                    if (bar.ContainsMouseDown(false))
                    {
                        parent.SetTrue("dragContent");
                        info.initOffset = true;
                        info.moveOffsetX = position;

                        info.actionPressed = true;
                        info.sameAction = info.actionKey == actionKey;
                        info.actionArray = actionArray;
                        info.action = action;
                        info.actionKey = actionKey;
                    }

                    if (Event.current.type == EventType.Repaint)
                    {
                        if (isPlaying && main.time >= action.Float("startTime") && main.time <= action.Float("endTime"))
                            EditorGUI.DrawRect(bar, Tint.Delete);
                        else
                            EditorGUI.DrawRect(bar,
                                info.actionKey == actionKey ? Tint.Blue : setType ? Tint.Green : Tint.Orange);
                        EditorGUI.DrawRect(handle, Tint.BoxTwo);
                        if (info.actionKey == actionKey && !setType)
                        {
                            left.width = 2f;
                            right.width = 2f;
                            right.x += 2f;
                            EditorGUI.DrawRect(left, Tint.Delete * Tint.WhiteOpacity180);
                            EditorGUI.DrawRect(right, Tint.Delete * Tint.WhiteOpacity180);
                        }
                    }

                    if (Event.current.type == EventType.MouseUp)
                    {
                        parent.SetFalse("dragContent");
                        parent.SetFalse("dragLeft");
                        parent.SetFalse("dragRight");
                    }

                    if (info.actionKey == actionKey && parent.Bool("dragContent") &&
                        Event.current.type == EventType.MouseDrag && Event.current.delta.x != 0)
                    {
                        if (info.initOffset)
                        {
                            info.initOffset = false;
                            info.moveOffsetX -= (Event.current.mousePosition.x + contentOffsetX) * info.timeRate /
                                                info.time;
                        }

                        if (parent.Bool("dragLeft"))
                        {
                            var newStartTime = (Event.current.mousePosition.x + contentOffsetX) * info.timeRate /
                                               info.time;
                            newStartTime = Mathf.Max(Compute.Round(newStartTime + info.moveOffsetX, 0.05f), 0);
                            startTime.floatValue = newStartTime / info.timeConvert;
                        }
                        else if (parent.Bool("dragRight"))
                        {
                            var newEndTime = (Event.current.mousePosition.x + contentOffsetX) * info.timeRate /
                                             info.time;
                            newEndTime = Mathf.Max(Compute.Round(newEndTime, 0.05f), 0);
                            endTime.floatValue = newEndTime / info.timeConvert;
                        }
                        else
                        {
                            var newStartTime = (Event.current.mousePosition.x + contentOffsetX) * info.timeRate /
                                               info.time;
                            newStartTime = Mathf.Max(Compute.Round(newStartTime + info.moveOffsetX, 0.05f), 0);
                            startTime.floatValue = newStartTime / info.timeConvert;
                            endTime.floatValue = newStartTime / info.timeConvert + duration;
                        }

                        AdjustActions(actionArray, info);
                        editor.Repaint();
                    }
                }


                if (row.ContainsMouseRightDown())
                {
                    mousePositionRef = Event.current.mousePosition;
                    contentOffsetXRef = contentOffsetX;
                    AddActionMenu(trackKey);
                }
            }

            Draw.GLEnd();


            if (Event.current.type != EventType.Repaint || main.lineMaterial == null)
            {
                GUI.EndGroup();
                return;
            }

            DrawWindowDivisions(window, contentOffsetY);
            GUI.EndGroup();
        }

        private void DrawWindowDivisions(Rect window, float contentOffsetY)
        {
            Draw.GLStart(main.lineMaterial);
            {
                GL.Color(Tint.BoxTwo);
                var positionY = -contentOffsetY;
                var step = 23f;
                var size = 6;

                for (float i = 0; i < size; i++)
                {
                    positionY += step;
                    if (positionY <= 0 || positionY + 0 >= window.height) continue;
                    Draw.GLLine(new Vector2(0, positionY), new Vector2(window.width, positionY));
                }
            }
            Draw.GLEnd();
        }

        public static void AdjustActions(SerializedProperty array, ChronosVariables info)
        {
            if (array == null || array.arraySize <= 1) return;
            for (var i = 0; i < array.arraySize - 1; i++)
            {
                var a = array.Element(i);
                var b = array.Element(i + 1);

                var endTime = a.Get("endTime").floatValue;
                var startTime = b.Get("startTime").floatValue;

                if (endTime > startTime)
                {
                    var duration = b.Get("endTime").floatValue - startTime;
                    b.Get("startTime").floatValue = endTime;
                    b.Get("endTime").floatValue = endTime + duration;
                }
            }
        }

        private void AddActionMenu(int key)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add Action"), false, CreateAction, key);
            menu.ShowAsContext();
        }

        private void FullActionMenu(int trackKey, int actionKey)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add Action"), false, CreateAction, trackKey);
            menu.AddItem(new GUIContent("Duplicate Action"), false, DuplicateAction, actionKey);
            menu.AddItem(new GUIContent("Delete Action"), false, DeleteAction, actionKey);
            menu.ShowAsContext();
        }

        public void CreateAction(object obj)
        {
            parent.Update();
            var key = (int)obj;
            var tracks = parent.Get("track");

            for (var i = 0; i < tracks.arraySize; i++)
            {
                var trackKey = 1000000 * (i + 1);
                if (key != trackKey) continue;

                float startTime = 0;
                var moveIndex = false;
                var destinationIndex = 0;
                var timeLinePosition = (mousePositionRef.x + contentOffsetXRef) * info.timeRate / info.time;
                var actionArray = tracks.Element(i).Get("action");
                var originArraySize = actionArray.arraySize;
                actionArray.arraySize++;
                var newAction = actionArray.LastElement();

                // clear
                EditorTools.ClearProperty(newAction);
                EditorTools.ClearProperty(newAction.Get("fieldChange"));
                newAction.Get("type").intValue = 0;

                if (originArraySize == 0)
                {
                    startTime = timeLinePosition;
                }
                else if (originArraySize == 1)
                {
                    var beforeStart = actionArray.Element(0).Get("startTime").floatValue * info.timeConvert;
                    startTime = actionArray.Element(0).Get("endTime").floatValue * info.timeConvert;
                    if (timeLinePosition > startTime)
                    {
                        startTime = timeLinePosition;
                    }
                    else if (timeLinePosition < beforeStart)
                    {
                        startTime = timeLinePosition;
                        moveIndex = true;
                        destinationIndex = 0;
                    }
                }
                else if (originArraySize > 1)
                {
                    for (var j = 0; j < actionArray.arraySize - 2; j++)
                    {
                        var start = actionArray.Element(j).Get("startTime").floatValue * info.timeConvert;
                        var rangeA = actionArray.Element(j).Get("endTime").floatValue * info.timeConvert;
                        var rangeB = actionArray.Element(j + 1).Get("startTime").floatValue * info.timeConvert;

                        if (j == 0 && timeLinePosition < start)
                        {
                            startTime = timeLinePosition;
                            moveIndex = true;
                            destinationIndex = 0;
                            break;
                        }

                        if (timeLinePosition >= rangeA && timeLinePosition <= rangeB)
                        {
                            startTime = timeLinePosition;
                            moveIndex = true;
                            destinationIndex = j + 1;
                            break;
                        }

                        if (j == actionArray.arraySize - 3 && timeLinePosition >= rangeB)
                        {
                            startTime = timeLinePosition;
                            break;
                        }
                    }
                }

                newAction.Get("startTime").floatValue = startTime / info.timeConvert;
                newAction.Get("endTime").floatValue = startTime / info.timeConvert + 2f;

                info.action = newAction;
                info.actionArray = actionArray;
                info.actionKey = trackKey + 10 * actionArray.arraySize;

                if (moveIndex)
                {
                    actionArray.MoveArrayElement(actionArray.arraySize - 1, destinationIndex);
                    info.actionKey = trackKey + 10 * (destinationIndex + 1);
                    info.action = actionArray.Element(destinationIndex);
                }

                AdjustActions(actionArray, info);
                parent.ApplyModifiedProperties();
                return;
            }

            parent.ApplyModifiedProperties();
        }

        public void DuplicateAction(object obj)
        {
            parent.Update();
            var actionKey = (int)obj;
            var tracks = parent.Get("track");

            for (var i = 0; i < tracks.arraySize; i++)
            {
                var actionArray = tracks.Element(i).Get("action");
                for (var j = 0; j < actionArray.arraySize; j++)
                {
                    var key = 1000000 * (i + 1) + 10 * (j + 1);
                    if (key == actionKey)
                    {
                        var startTime = actionArray.Element(j).Get("startTime").floatValue * info.timeConvert;
                        var endTime = actionArray.Element(j).Get("endTime").floatValue * info.timeConvert;
                        var duration = Mathf.Abs(endTime - startTime);

                        // move to last so it can be copied
                        actionArray.MoveArrayElement(j, actionArray.arraySize - 1);

                        // copy
                        actionArray.arraySize++;

                        // get new copy
                        var newAction = actionArray.LastElement();
                        newAction.Get("startTime").floatValue = endTime / info.timeConvert;
                        newAction.Get("endTime").floatValue = endTime / info.timeConvert + duration;

                        // move back
                        actionArray.MoveArrayElement(actionArray.arraySize - 2, j);
                        actionArray.MoveArrayElement(actionArray.arraySize - 1, j + 1);

                        info.actionKey = 1000000 * (i + 1) + 10 * (j + 1 + 1);
                        info.actionArray = actionArray;
                        info.action = actionArray.Element(j + 1);

                        AdjustActions(actionArray, info);
                        parent.ApplyModifiedProperties();
                        return;
                    }
                }
            }

            parent.ApplyModifiedProperties();
        }

        public void DeleteAction(object obj)
        {
            parent.Update();
            var actionKey = (int)obj;
            var tracks = parent.Get("track");

            for (var i = 0; i < tracks.arraySize; i++)
            {
                var actionArray = tracks.Element(i).Get("action");
                for (var j = 0; j < actionArray.arraySize; j++)
                {
                    var key = 1000000 * (i + 1) + 10 * (j + 1);
                    if (key == actionKey)
                    {
                        actionArray.DeleteArrayElementAtIndex(j);

                        info.action = null;
                        info.actionArray = null;
                        var index = Mathf.Clamp(j - 1, 0, actionArray.arraySize - 1);
                        if (actionArray.arraySize > 0 && index < actionArray.arraySize)
                        {
                            info.action = actionArray.Element(index);
                            info.actionKey = 1000000 * (i + 1) + 10 * (index + 1);
                            info.actionArray = actionArray;
                        }

                        parent.ApplyModifiedProperties();
                        return;
                    }
                }
            }

            parent.ApplyModifiedProperties();
        }
    }
}