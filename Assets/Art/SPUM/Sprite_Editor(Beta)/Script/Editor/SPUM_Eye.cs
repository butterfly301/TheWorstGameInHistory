using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpriteEyeSync))]
public class SPUM_Eye : Editor
{
    // Start is called before the first frame update
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var SPB = (SpriteEyeSync)target;
        if (SPB._nowTexture != null)
        {
            if (GUILayout.Button("Sync Pivot", GUILayout.Height(50))) SPB.SyncPivot();
            if (GUILayout.Button("Remove Sprite", GUILayout.Height(50))) SPB.RemoveSprite();
        }
    }
}