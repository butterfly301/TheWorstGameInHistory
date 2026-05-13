using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Editors
{
    public class AINodeEditors
    {
        public static void Clock(Object obj)
        {
            var parent = new SerializedObject(obj);
            parent.Update();

            parent.ApplyModifiedProperties();
        }
    }
}