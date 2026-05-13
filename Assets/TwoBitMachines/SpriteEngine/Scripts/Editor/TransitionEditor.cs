using System.Collections.Generic;
using TwoBitMachines.Editors;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.TwoBitSprite.Editors
{
    public static class TransitionEditor
    {
        public static void Transition(Editor editor, SerializedObject parent, SerializedProperty animation,
            string field, List<string> spriteNamesArray, List<string> signals)
        {
            if (!Block.Header(parent).Style(Tint.SoftDark).Fold("Transition", "transitionFoldOut", true, Tint.White)
                    .Button().Build()) return;

            if (Header.SignalActive("Add"))
            {
                var menu = new GenericMenu();
                for (var i = 0; i < spriteNamesArray.Count; i++)
                {
                    var animationName = spriteNamesArray[i];
                    menu.AddItem(new GUIContent(animationName), false, () =>
                    {
                        parent.Update();
                        var animations = parent.Get(field);
                        for (var i = 0; i < animations.arraySize; i++)
                            if (animations.Element(i).String("name") == animationName)
                            {
                                animations.Element(i).SetTrue("hasTransition");
                                break;
                            }

                        parent.ApplyModifiedProperties();
                    });
                }

                menu.ShowAsContext();
            }

            for (var i = 0; i < animation.arraySize; i++)
            {
                var element = animation.Element(i);
                if (!element.Bool("hasTransition")) continue;

                Block.Header(element).Style(Tint.Green)
                    .Fold(element.String("name"), "transitionFoldOut", true, Tint.White)
                    .Button("Delete")
                    .Button()
                    .Build();

                if (Header.SignalActive("Delete"))
                {
                    element.SetFalse("hasTransition");
                    continue;
                }

                var array = element.Get("transition");
                if (array.arraySize == 0 || Header.SignalActive("Add"))
                {
                    array.CreateNewElement();
                    element.SetTrue("transitionFoldOut");
                }

                if (!element.Bool("transitionFoldOut")) continue;

                element.SetFalse("hasChangedDirection");
                for (var j = 0; j < array.arraySize; j++)
                {
                    var transition = array.Element(j);
                    Block.Box(2, Tint.Box, j == 0);
                    {
                        Block.CornerGrip(editor, array, j);
                        if (transition.DropDownListAndButton_(signals, "Condition", "condition", "Delete"))
                        {
                            array.DeleteArrayElement(j);
                            break;
                        }

                        transition.DropDownDoubleList_(spriteNamesArray, "From, Transition", "from", "to");
                        if (transition.String("condition") == "changedDirection")
                            element.SetTrue("hasChangedDirection");
                    }
                }
            }
        }
    }
}