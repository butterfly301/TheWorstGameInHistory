#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIMirror), true)]
[CanEditMultipleObjects]
public class UIMirrorEditor : Editor
{
    private SerializedProperty m_MirrorType;
    private GUIContent m_SetNativeSizeContent;
    private GUIContent m_MirrorTypeContent;

    private void OnEnable()
    {
        m_SetNativeSizeContent = new GUIContent("Set Native Size", "Sets the size to match the mirrored content.");
        m_MirrorTypeContent = new GUIContent("Mirror Type");
        m_MirrorType = serializedObject.FindProperty("m_MirrorType");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_MirrorType, m_MirrorTypeContent);

        if (GUILayout.Button(m_SetNativeSizeContent, EditorStyles.miniButton))
        {
            foreach (Object targetObject in targets)
            {
                if (targetObject is UIMirror mirror)
                {
                    Undo.RecordObject(mirror.rectTransform, "Set Native Size");
                    mirror.SetNativeSize();
                    EditorUtility.SetDirty(mirror);
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
