using System;
using System.Globalization;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace TwoBitMachines
{
    public class TextMeshProEffects : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMesh;
        [SerializeField] private bool startOnEnable = true;

        [SerializeField] private int typewriterFade = 5;
        [SerializeField] private float typewriterSpeed = 5.5f;
        [SerializeField] private float typewriterWobble = 2f;

        [SerializeField] private float wobble = 1f;
        [SerializeField] private float wobbleSpeed = 1f;

        [SerializeField] private float waveSpeed = 2f;
        [SerializeField] private float wavePhase = 0.01f;
        [SerializeField] private float waveStrength = 0.1f;

        [SerializeField] private float waveSpeedX = 2f;
        [SerializeField] private float wavePhaseX = 0.01f;
        [SerializeField] private float waveStrengthX = 0.1f;

        [SerializeField] private float jitterStrength = 0.1f;
        [SerializeField] private float jitterRate = 0.01f;

        [SerializeField] private float distortionStrength = 0.1f;
        [SerializeField] private float distortionRate = 0.1f;

        [SerializeField] public string worldEffect = "";
        [SerializeField] public float typingRate = 1f;
        [SerializeField] public UnityEventEffect onTyping = new();
        [SerializeField] private UnityEvent onComplete = new();
        [NonSerialized] private int characterRange;
        [NonSerialized] private float counter;
        [NonSerialized] private int currentCharacter;
        [NonSerialized] private ITypeWriterComplete dialogueRef;
        [NonSerialized] private float distortionCounter;
        [NonSerialized] private float jitterCounter;

        [NonSerialized] private Vector3[] oldVertices;
        [NonSerialized] private float pauseCounter;
        [NonSerialized] private bool pauseDetected;
        [NonSerialized] private float pauseLimit;
        [NonSerialized] private bool pauseTypeWriter;
        [NonSerialized] private float speedOrigin;
        [NonSerialized] private bool typeWriterOn;
        [NonSerialized] private float typingCounter;

        private void Awake()
        {
            speedOrigin = typewriterSpeed;
        }

        private void Update()
        {
            if (oldVertices == null) return; // not ready yet

            var update = false;
            TypeWriter(ref update);
            Effects(ref update);
            if (update) //                      only update if changes have been detected
                textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }

        private void OnEnable()
        {
            if (startOnEnable) BeginTextMeshEffects();
        }

        public void BeginTextMeshEffects()
        {
            if (dialogueRef == null) dialogueRef = gameObject.GetComponent<ITypeWriterComplete>();

            dialogueRef?.TypingCommence();
            textMesh.ForceMeshUpdate();
            oldVertices = textMesh.mesh.vertices;
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 0);
            counter = 0;
            pauseCounter = 0;
            characterRange = 0;
            currentCharacter = 0;
            typingCounter = 10000f;
            typeWriterOn = true;
            pauseDetected = false;
            pauseTypeWriter = false;
            typewriterSpeed = speedOrigin;
        }

        private void TypeWriter(ref bool update)
        {
            if (pauseTypeWriter && Clock.Timer(ref pauseCounter, pauseLimit))
            {
                pauseTypeWriter = false;
                pauseDetected = false;
                var textInfo = textMesh.textInfo;
                if (characterRange + 1 <
                    textInfo.characterCount)
                    characterRange += 1; // increase here or system can go back into pause mode!
            }

            if (!pauseTypeWriter && typeWriterOn && Clock.Timer(ref typingCounter, typingRate))
            {
                var impact = ImpactPacket.impact.Set(worldEffect, transform, null, transform.position, null,
                    Vector2.zero, 1, 0);
                onTyping.Invoke(impact);
            }

            var rate = (10f - typewriterSpeed) * 0.01f + 0.01f;
            if (typeWriterOn && Clock.Timer(ref counter, rate))
            {
                update = true;
                var textInfo = textMesh.textInfo;
                var characterCount = textInfo.characterCount;
                var fadeSteps = Mathf.Max(1f, 255f / typewriterFade);

                for (var i = currentCharacter; i < characterRange + 1; i++)
                {
                    var materialIndex =
                        textInfo.characterInfo[i]
                            .materialReferenceIndex; // get the index of the material used by the current character.
                    var index = textInfo.characterInfo[i]
                        .vertexIndex; //                    get the index of the first vertex used by this text element.
                    var colors =
                        textInfo.meshInfo[materialIndex]
                            .colors32; //         get the vertex colors of the mesh used by this text element (character or sprite).                 
                    var vertices = textMesh.textInfo.meshInfo[materialIndex].vertices;

                    if (i > 0 && colors[0].a == 0)
                    {
                        SetAlpha(colors, 0, (byte)255f);
                        SetVertices(vertices, 0, Vector3.zero);
                    }

                    var lerp = Mathf.Clamp(colors[index + 0].a + fadeSteps, 0, 255f);

                    if (!textInfo.characterInfo[i].isVisible)
                    {
                        if (i == currentCharacter && ++currentCharacter >= characterCount) Complete();
                        continue;
                    }

                    var angle = Mathf.Lerp(0, 180f, lerp / 255f);
                    SetAlpha(colors, index, (byte)lerp);
                    SetVertices(vertices, index, Vector3.up * Mathf.Sin(angle * Mathf.Deg2Rad) * typewriterWobble);

                    if (lerp >= 255f && ++currentCharacter >= characterCount) Complete();
                }

                if (!pauseTypeWriter && characterRange + 1 < characterCount) characterRange += 1;
            }
        }

        private void Effects(ref bool update)
        {
            var textInfo = textMesh.textInfo;
            if (textInfo == null || textInfo.lineInfo == null)
                return;

            for (var l = 0; l < textInfo.linkCount; l++)
            {
                update = true;
                var link = textInfo.linkInfo[l];
                var startIndex = link.linkTextfirstCharacterIndex;
                var range = startIndex + link.linkTextLength;
                var linkID = link.GetLinkID();

                if (linkID.Contains("jitter"))
                {
                    Jitter(startIndex, range, textInfo);
                }
                else if (linkID.Contains("wobble"))
                {
                    Wobble(startIndex, range, textInfo);
                }
                else if (linkID.Contains("distortion"))
                {
                    Distortion(startIndex, range, textInfo);
                }
                else if (linkID.Contains("waveX"))
                {
                    WaveX(startIndex, range, textInfo);
                }
                else if (linkID.Contains("wave"))
                {
                    Wave(startIndex, range, textInfo);
                }
                else if (!pauseDetected && characterRange + 1 == startIndex &&
                         linkID.Contains("pause")) //+ 1 so that pause occurs before the first letter of the word
                {
                    // string[] time = linkID.Split (new string[] { "pause" }, System.StringSplitOptions.None);
                    pauseTypeWriter = true;
                    pauseDetected = true;
                    pauseCounter = 0;
                    var time = Regex.Replace(linkID, "[^0-9]", "");
                    pauseLimit = float.Parse(time, CultureInfo.InvariantCulture);
                }
                else if (!pauseDetected && characterRange + 1 == startIndex && linkID.Contains("speed"))
                {
                    var time = linkID.Split(new[] { "speed" }, StringSplitOptions.None);
                    typewriterSpeed = float.Parse(time[1], CultureInfo.InvariantCulture);
                }
                // if pause and speed are adjacent in the string message, then speed must come first, or else it will never be set
            }
        }

        private void Wobble(int startIndex, int range, TMP_TextInfo textInfo) //(float time)
        {
            for (var i = startIndex; i < range; i++)
            {
                if (ValidCharacter(i, textInfo, out var c)) continue;
                var t = Time.time + i;
                var effect = new Vector2(Mathf.Sin(t * wobbleSpeed), Mathf.Cos(t * wobbleSpeed)) * wobble;
                SetVertices(textInfo.meshInfo[c.materialReferenceIndex].vertices, c.vertexIndex, effect);
            }
        }

        private void Wave(int startIndex, int range, TMP_TextInfo textInfo)
        {
            for (var i = startIndex; i < range; i++)
            {
                if (ValidCharacter(i, textInfo, out var c)) continue;
                var effect = new Vector2(0,
                    Mathf.Sin(Time.time * waveSpeed + oldVertices[c.vertexIndex].x * wavePhase) * waveStrength);
                SetVertices(textInfo.meshInfo[c.materialReferenceIndex].vertices, c.vertexIndex, effect);
            }
        }

        private void WaveX(int startIndex, int range, TMP_TextInfo textInfo)
        {
            for (var i = startIndex; i < range; i++)
            {
                if (ValidCharacter(i, textInfo, out var c)) continue;
                var effect =
                    new Vector2(
                        Mathf.Sin(Time.time * waveSpeedX + oldVertices[c.vertexIndex].x * wavePhaseX) * waveStrengthX,
                        0);
                SetVertices(textInfo.meshInfo[c.materialReferenceIndex].vertices, c.vertexIndex, effect);
            }
        }

        private void Jitter(int startIndex, int range, TMP_TextInfo textInfo)
        {
            if (!Clock.Timer(ref jitterCounter, jitterRate))
                return;

            for (var i = startIndex; i < range; i++)
            {
                if (ValidCharacter(i, textInfo, out var c)) continue;
                var offset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                SetVertices(textInfo.meshInfo[c.materialReferenceIndex].vertices, c.vertexIndex,
                    offset * jitterStrength);
            }
        }

        private void Distortion(int startIndex, int range, TMP_TextInfo textInfo)
        {
            if (!Clock.Timer(ref distortionCounter, distortionRate))
                return;

            for (var i = startIndex; i < range; i++)
            {
                if (ValidCharacter(i, textInfo, out var c)) continue;
                var vertices = textInfo.meshInfo[c.materialReferenceIndex].vertices;
                var index = c.vertexIndex;
                for (var j = 0; j < 4; j++)
                {
                    var offset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                    vertices[index + j] = oldVertices[index + j] + offset * distortionStrength;
                }
            }
        }

        private bool ValidCharacter(int i, TMP_TextInfo textInfo, out TMP_CharacterInfo charInfo)
        {
            charInfo = textInfo.characterInfo[i];
            return charInfo.character == ' ' || !charInfo.isVisible;
        }

        private void SetVertices(Vector3[] vertices, int index, Vector3 offset)
        {
            vertices[index + 0] = oldVertices[index + 0] + offset;
            vertices[index + 1] = oldVertices[index + 1] + offset;
            vertices[index + 2] = oldVertices[index + 2] + offset;
            vertices[index + 3] = oldVertices[index + 3] + offset;
        }

        private void SetAlpha(Color32[] colors, int index, byte alpha)
        {
            colors[index + 0].a = alpha;
            colors[index + 1].a = alpha;
            colors[index + 2].a = alpha;
            colors[index + 3].a = alpha;
        }

        private void Complete()
        {
            typeWriterOn = false;
            onComplete.Invoke();
            if (dialogueRef != null) dialogueRef.TypingComplete();
        }

        public void SetTypeWriterSpeed(float speed)
        {
            typewriterSpeed = speed;
        }

        #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] public ShowType showType;
        [SerializeField] private bool completeFoldOut;
        [SerializeField] private bool eventsFoldOut;
        [SerializeField] private bool typingFoldOut;
#pragma warning restore 0414
#endif

        #endregion
    }

    public interface ITypeWriterComplete
    {
        public void TypingComplete();
        public void TypingCommence();
    }

    [Flags]
    public enum ShowType
    {
        Wobble = 1 << 0,
        Wave = 1 << 1,
        WaveX = 1 << 2,
        Jitter = 1 << 3,
        Distortion = 1 << 4
    }
}