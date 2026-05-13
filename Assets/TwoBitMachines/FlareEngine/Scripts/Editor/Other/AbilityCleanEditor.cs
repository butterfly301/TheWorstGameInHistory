using TwoBitMachines.FlareEngine.ThePlayer;
using UnityEditor;

namespace TwoBitMachines.FlareEngine.Editors
{
    [CustomEditor(typeof(AbilityClean))]
    public class AbilityCleanEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var main = target as AbilityClean;

            if (main == null) return;

            if (main.gameObject.GetComponent<Player>() != null)
            {
                DestroyImmediate(main); // If still exists, then destroy clean script
                return;
            }

            var allChildren = main.transform.GetComponents<Ability>();
            for (var i = 0; i < allChildren.Length; i++) DestroyImmediate(allChildren[i]);
            DestroyImmediate(main);
        }
    }
}