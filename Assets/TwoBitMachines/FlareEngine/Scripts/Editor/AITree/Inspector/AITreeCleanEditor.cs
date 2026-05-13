using TwoBitMachines.FlareEngine.AI;
using TwoBitMachines.FlareEngine.AI.BlackboardData;
using UnityEditor;

namespace TwoBitMachines.FlareEngine.Editors
{
    [CustomEditor(typeof(AIClean))]
    public class AITreeCleanEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var main = target as AIClean;

            if (main == null) return;

            if (main.gameObject.GetComponent<AIBase>() != null)
            {
                DestroyImmediate(main);
                return;
            }

            var allChildren = main.transform.GetComponents<Node>();
            for (var i = 0; i < allChildren.Length; i++) DestroyImmediate(allChildren[i]);

            var allChildrenB = main.transform.GetComponents<Blackboard>();
            for (var i = 0; i < allChildrenB.Length; i++) DestroyImmediate(allChildrenB[i]);
            DestroyImmediate(main);
        }
    }
}