using System;
using UnityEngine;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

namespace TwoBitMachines.Safire2DCamera
{
    [Serializable]
    public class ResolutionScaling
    {
        [SerializeField] public bool enable;
        [SerializeField] public ResolutionScalingMonobehaviour resolution;

        public void Execute(Camera camera)
        {
            if (enable && resolution != null && Application.isPlaying)
            {
                resolution.cam = camera;
                resolution.Execute();
            }
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] public bool foldOut;
        [SerializeField] [HideInInspector] public bool close;
        [SerializeField] [HideInInspector] public bool edit;
        [SerializeField] [HideInInspector] public bool add;

        public static void CustomInspector(SerializedProperty parent, Color barColor, Color labelColor,
            Safire2DCamera main)
        {
            if (!parent.Bool("edit"))
                return;

            main.resolutionScaling.Create(main);

            if (Follow.Open(parent, "Resolution Scaling", barColor, labelColor))
            {
                var property = parent.Get("resolution");
                if (property.objectReferenceValue == null) return;
                var newParent = new SerializedObject(property.objectReferenceValue);

                newParent.Update();

                var type = newParent.Enum("type");
                var ortho = main.cameraRef.orthographic;
                FoldOut.Box(3, Tint.Box);
                {
                    newParent.Field("Match", "type", type != 0);
                    newParent.FieldDouble("Type", "type", "color", type == 0);
                    newParent.Field("Resolution", "resolution");
                    newParent.Field("Target PPU", "targetPPU", ortho);
                    newParent.Field("Target FOV", "targetFOV", !ortho);
                }
                Layout.VerticalSpacing(5);


                newParent.ApplyModifiedProperties();
            }

            ;
        }

        public void Create(Safire2DCamera main)
        {
            if (Application.isPlaying) return;
            if (enable && resolution == null)
            {
                resolution = main.gameObject.AddComponent<ResolutionScalingMonobehaviour>();
                resolution.resolutionRef = main.resolutionScaling;
                resolution.cam = main.cameraRef;
            }

            if (!enable && resolution != null) MonoBehaviour.DestroyImmediate(resolution);
            if (enable && resolution != null && main.cameraRef != null) resolution.cam = main.cameraRef;
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}