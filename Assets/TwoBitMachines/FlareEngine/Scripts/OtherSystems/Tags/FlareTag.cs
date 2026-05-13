using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
    public class FlareTag : MonoBehaviour
    {
        [SerializeField] public TagListSO tagListSO;
        [SerializeField] public List<string> tags = new();

        public bool Contains(string id)
        {
            return tags.Contains(id);
        }

        public void AddTag(string tag)
        {
            if (!tags.Contains(tag)) tags.Add(tag);
        }

        public static bool ObjectHasTag(Transform transform, string tag)
        {
            var flareTag = transform == null ? null : transform.gameObject.GetComponent<FlareTag>();
            return flareTag == null ? false : flareTag.Contains(tag);
        }
    }
}