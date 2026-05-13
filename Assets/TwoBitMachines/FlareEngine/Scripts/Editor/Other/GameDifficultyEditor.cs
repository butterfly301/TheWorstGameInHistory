using TwoBitMachines.Editors;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Editors
{
    [CustomEditor(typeof(GameDifficulty), true)]
    public class GameDifficultyEditor : UnityEditor.Editor
    {
        public static SaveOptions save = new();
        private GameDifficulty main;
        private SerializedObject parent;
        private string saveFolder;

        private void OnEnable()
        {
            main = target as GameDifficulty;
            parent = serializedObject;
            Layout.Initialize();

            Storage.encrypt = false; // probably not right?
            SaveOptions.Load(ref save);
            saveFolder = save.RetrieveSaveFolder();
            main.Restore(saveFolder);
        }

        public override void OnInspectorGUI()
        {
            //bool saveChanges = false;
            Layout.Update();
            Layout.VerticalSpacing(10);
            parent.Update();
            {
                EditorGUI.BeginChangeCheck();
                var difficulty = parent.Get("difficulty");
                FoldOut.Box(1, Tint.Delete);
                {
                    difficulty.Field("Difficulty", "difficulty");
                }
                Layout.VerticalSpacing(5);

                var array = difficulty.Get("level");


                difficulty.ClampInt("difficulty", 0, Mathf.Max(0, array.arraySize - 1));

                for (var i = 0; i < array.arraySize; i++)
                {
                    var level = array.Element(i);
                    var open = level.Bool("foldOut");
                    if (FoldOut.Bar(level, Tint.Blue)
                        .Grip(difficulty, array, i, color: Tint.WarmWhite)
                        .Label("Difficulty: " + i, Color.white)
                        .RightButton("delete", "Delete", "Delete Difficulty", execute: open)
                        .RightButton(toolTip: "Add Behaviour", execute: open)
                        .FoldOut())
                    {
                        if (level.ReadBool("delete"))
                        {
                            array.DeleteArrayElement(i);
                            break;
                        }

                        var behaviour = level.Get("behaviour");
                        if (level.ReadBool("add")) behaviour.arraySize++;
                        if (behaviour.arraySize == 0) Layout.VerticalSpacing(3);
                        for (var j = 0; j < behaviour.arraySize; j++)
                        {
                            var behaviourE = behaviour.Element(j);
                            FoldOut.Box(2, Tint.Box);
                            {
                                if (behaviourE.FieldAndButton("Behaviour", "type", "Delete"))
                                {
                                    behaviour.DeleteArrayElementAtIndex(j);
                                    break;
                                }

                                behaviourE.Field("Value", "value");
                            }
                            Layout.VerticalSpacing(5);
                        }
                    }
                }

                if (FoldOut.CornerButton(Tint.Blue)) array.arraySize++;
            }
            parent.ApplyModifiedProperties();
            Layout.VerticalSpacing(10);

            if (EditorGUI.EndChangeCheck()) main.Save(saveFolder);
        }
    }
}