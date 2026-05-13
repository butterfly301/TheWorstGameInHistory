using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Editors
{
    public class FlareSettingsEditorWindow : EditorWindow
    {
        private readonly List<Object> pathObjects = new();


        private readonly List<string> paths = new();

        private void OnEnable()
        {
            paths.Clear();
            pathObjects.Clear();

            for (var i = 0; i < UserFolderPaths.paths.Length; i++)
            {
                var path = PlayerPrefs.GetString(UserFolderPaths.paths[i], "");
                paths.Add(path);
                pathObjects.Add(string.IsNullOrEmpty(path) ? null : AssetDatabase.LoadAssetAtPath<Object>(path));
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("");
            for (var i = 0; i < UserFolderPaths.paths.Length; i++)
            {
                var previousFolder = pathObjects[i];
                pathObjects[i] = EditorGUILayout.ObjectField(UserFolderPaths.pathLabel[i], pathObjects[i],
                    typeof(DefaultAsset), false);

                if (previousFolder != pathObjects[i])
                {
                    if (pathObjects[i] != null)
                    {
                        var newPath = AssetDatabase.GetAssetPath(pathObjects[i]);
                        PlayerPrefs.SetString(UserFolderPaths.paths[i], newPath);
                        PlayerPrefs.SetString(UserFolderPaths.paths[i] + "Name", pathObjects[i].name);
                    }
                    else
                    {
                        PlayerPrefs.SetString(UserFolderPaths.paths[i], "");
                        PlayerPrefs.SetString(UserFolderPaths.paths[i] + "Name", "");
                    }

                    AIEditor.dataList.Clear();
                    AIEditor.actionList.Clear();
                }
            }

            GUILayout.Label("");
            EditorGUILayout.LabelField("To refresh folder references, recompile code or enter and exit playmode.",
                EditorStyles.wordWrappedLabel);
        }

        [MenuItem("Window/Flare Settings")]
        public static void ShowWindow()
        {
            GetWindow<FlareSettingsEditorWindow>("Flare Settings");
        }
    }
}