using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
    // [CreateAssetMenu(menuName = "FlareEngine/TagListSO")]
    public class TagListSO : ScriptableObject
    {
        [SerializeField] public List<string> tags = new();
        [SerializeField] public bool editorOpen;
    }
}