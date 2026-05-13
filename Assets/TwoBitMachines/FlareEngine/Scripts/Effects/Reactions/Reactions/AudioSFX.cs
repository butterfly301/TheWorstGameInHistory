#region

using UnityEngine;
#if UNITY_EDITOR
using System;
using System.Reflection;
using TwoBitMachines.Editors;
using UnityEditor;
#endif

#endregion

namespace TwoBitMachines.FlareEngine
{
    [AddComponentMenu("")]
    public class AudioSFX : ReactionBehaviour
    {
        [SerializeField] public Audio sfx = new();

        public override void Activate(ImpactPacket packet)
        {
            AudioManager.get.PlaySFX(sfx);
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        public override bool OnInspector(SerializedObject parent, Color barColor, Color labelColor)
        {
            if (Open(parent, "Audio SFX", barColor, labelColor))
            {
                Bar.Remember();
                var element = parent.Get("sfx");
                var width = Layout.labelWidth + Layout.contentWidth - 40;

                FoldOut.Bar(element, Tint.Box)
                    .LF("clip", (int)(width * 0.7f), -4, 2)
                    .LF("volume", (int)(width * 0.3f), -4, 2);
                element.Clamp("volume");

                if (Bar.ButtonRight("Play", Tint.White))
                {
                    StopAllClips();
                    PlayClip(element.Get("clip").objectReferenceValue as AudioClip);
                }

                if (Bar.ButtonRight("Red", Tint.White)) StopAllClips();
                Bar.ResetToOld();
            }

            return true;
        }

        //https: //forum.unity.com/threads/way-to-play-audio-in-editor-using-an-editor-script.132042/
        public static void PlayClip(AudioClip clip, int startSample = 0, bool loop = false)
        {
            if (clip == null)
                return;
            var unityEditorAssembly = typeof(AudioImporter).Assembly;

            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "PlayPreviewClip",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new[] { typeof(AudioClip), typeof(int), typeof(bool) },
                null
            );

            method.Invoke(null, new object[] { clip, startSample, loop });
        }

        public static void StopAllClips()
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;

            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "StopAllPreviewClips",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new Type[] { },
                null
            );
            method.Invoke(null, new object[] { });
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}