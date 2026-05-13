#region

using UnityEngine;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

#endregion

namespace TwoBitMachines.FlareEngine
{
    [AddComponentMenu("")]
    public class ReactionEvent : ReactionBehaviour
    {
        [SerializeField] public UnityEventEffect onReaction = new();

        public override void Activate(ImpactPacket impact)
        {
            onReaction.Invoke(impact);
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] public bool eventFoldout;
        public override bool OnInspector(SerializedObject parent, Color barColor, Color labelColor)
        {
            if (Open(parent, "On Reaction", barColor, labelColor))
                Fields.EventFoldOut(parent.Get("onReaction"), parent.Get("eventFoldout"), "On Reaction");
            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}