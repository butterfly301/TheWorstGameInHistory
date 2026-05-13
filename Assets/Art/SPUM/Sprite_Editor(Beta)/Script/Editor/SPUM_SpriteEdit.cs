using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SPUM_SpriteEditManager))]
public class SPUM_SpriteEdit : Editor
{
    // Start is called before the first frame update
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var SPB = (SPUM_SpriteEditManager)target;
        if (GUILayout.Button("Empty All Sprite", GUILayout.Height(50))) SPB.EmptyAllSprite();
    }
}