using UnityEngine;

namespace InGameEditor
{
    public enum MessageType
    {
        OnPointerDown,
        OnPointerUp,
        OnDrag
    }
    public struct MyModal
    {
        public MessageType Type;
        public string Name;
        public Vector3 Pos;
    }
}