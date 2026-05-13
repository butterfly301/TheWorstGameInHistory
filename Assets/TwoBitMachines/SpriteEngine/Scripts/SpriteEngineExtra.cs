using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TwoBitMachines.TwoBitSprite
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteEngineExtra : SpriteEngineBase
    {
        [SerializeField] public List<SpritePacketExtra> sprites = new();
        [SerializeField] public SpritePacketExtra sprite;
        [NonSerialized] private float counter;
        [NonSerialized] private int frameIndex;

        [NonSerialized] public bool randomActive;
        [NonSerialized] private string returnSpriteRandom;

        public void Awake()
        {
            if (render == null) render = GetComponent<SpriteRenderer>();

            SpriteManager.get.Register(this);
            tree.Initialize(this);

            if (sprites.Count > 0) SetNewAnimation(sprites[0].name);
        }

        private void OnDrawGizmosSelected()
        {
            if (render == null) render = GetComponent<SpriteRenderer>();
        }

        public override void SetFirstAnimation()
        {
            gameObject.SetActive(true);
            currentAnimation = "";
            if (sprites.Count > 0) SetNewAnimation(sprites[0].name);
        }

        public override void Play()
        {
            if (pause || !enabled) return;

            tree.FindNextAnimation();

            if (sprite == null || sprite.frame.Count == 0)
            {
                tree.ClearSignals();
                return;
            }

            if (sprite.changedDirection && tree.SignalTrue("changedDirection"))
            {
                var oldSprite = sprite;
                if (sprite.Transition(this, currentAnimation))
                {
                    oldSprite.ResetProperties();
                    returnAnimation = currentAnimation;
                    counter = frameIndex = 0;
                    render.sprite = sprite.frame[0].sprite;
                    sprite.ClearRandomSprites();
                    sprite.ClearFrameEvents(0);
                    sprite.TriggerFrameEvents(0, 0);
                    sprite.SetProperties(0, true);
                }
            }

            var rate = sprite.frame[frameIndex].rate;
            var percent = counter / rate;
            sprite.TriggerFrameEvents(frameIndex, percent);

            if (Clock.Timer(ref counter, rate))
            {
                frameIndex = frameIndex + 1 >= sprite.frame.Count ? 0 : frameIndex + 1;

                if (frameIndex == 0) //                                  has looped
                {
                    frameIndex = sprite.loopStartIndex;
                    if (sprite.loopOnce)
                    {
                        frameIndex = sprite.frame.Count - 1; //  set to last 
                        sprite.onLoopOnce.Invoke();
                    }

                    if (randomActive)
                    {
                        randomActive = false;
                        SetSprite(returnSpriteRandom);
                    }

                    if (inTransition) SetReturnSprite();
                    sprite.CheckForRandomSprite(this);
                }

                sprite.ClearFrameEvents(frameIndex);
                sprite.TriggerFrameEvents(frameIndex, 0);
                render.sprite = sprite.frame[frameIndex].sprite;
                sprite.SetProperties(frameIndex);
            }

            sprite.InterpolateProperties(frameIndex, counter);
            tree.ClearSignals();
        }

        public override void SetNewAnimation(string newAnimation)
        {
            if (currentAnimation == newAnimation || newAnimation == "") return;
            SetSprite(newAnimation);
        }

        public void SetSprite(string newAnimation)
        {
            for (var i = 0; i < sprites.Count; i++)
                if (sprites[i].name == newAnimation)
                {
                    if (sprites[i].frame.Count == 0) return;
                    if (sprite != null) sprite.ResetProperties();

                    inTransition = false;
                    sprite = sprites[i];
                    if (sprite.useTransition)
                    {
                        returnAnimation = newAnimation;
                        sprite.Transition(this, currentAnimation);
                    }

                    currentAnimation = newAnimation;
                    counter = frameIndex = 0;
                    render.sprite = sprite.frame[0].sprite;
                    sprite.ClearRandomSprites();
                    sprite.ClearFrameEvents(0);
                    sprite.TriggerFrameEvents(0, 0);
                    sprite.SetProperties(0, true);
                    randomActive = false;
                    return;
                }
        }

        public void SetReturnSprite()
        {
            for (var i = 0; i < sprites.Count; i++)
                if (sprites[i].name == returnAnimation)
                {
                    if (sprites[i].frame.Count == 0) return;
                    if (sprite != null) sprite.ResetProperties();

                    inTransition = false;
                    sprite = sprites[i];
                    currentAnimation = returnAnimation;
                    counter = frameIndex = 0;
                    render.sprite = sprite.frame[0].sprite;
                    sprite.ClearRandomSprites();
                    sprite.ClearFrameEvents(0);
                    sprite.TriggerFrameEvents(0, 0);
                    sprite.SetProperties(0, true);
                    return;
                }
        }

        public void SetRandomSprite(string animationName)
        {
            frameIndex = 0;
            returnSpriteRandom = currentAnimation;
            SetSprite(animationName);
            currentAnimation = returnSpriteRandom;
            randomActive = true;
        }

        public SpritePacketExtra GetSprite(string animationName)
        {
            for (var i = 0; i < sprites.Count; i++)
                if (sprites[i].name == animationName)
                    return sprites[i];
            return null;
        }

        public override bool FlipAnimation(Dictionary<string, bool> signal, string signalName, string direction)
        {
            if (signal.TryGetValue(signalName, out var value) && value)
            {
                var renderer = render;

                if (direction == animationDirection[0])
                {
                    if (!renderer.flipX) renderer.flipX = true;
                }
                else if (direction == animationDirection[1])
                {
                    if (renderer.flipX) renderer.flipX = false;
                }
                else if (direction == animationDirection[2])
                {
                    if (renderer.flipY) renderer.flipY = false;
                }
                else if (direction == animationDirection[3])
                {
                    if (!renderer.flipY) renderer.flipY = true;
                }
            }

            return value;
        }
    }

    [Serializable]
    public class SpritePacketExtra
    {
        [SerializeReference] public List<ExtraProperty> property = new();
        [SerializeField] public List<FrameExtra> frame = new();
        [SerializeField] public UnityEvent onLoopOnce;
        [SerializeField] public int loopStartIndex;
        [SerializeField] public bool loopOnce;
        [SerializeField] public string name;

        [SerializeField] public List<RandomSprites> randomSprites = new();

        [SerializeField] public List<AnimationTransition> transition = new();
        [SerializeField] public bool hasTransition;
        [SerializeField] public bool hasChangedDirection;
        [NonSerialized] public int randomIndex;
        public bool useTransition => hasTransition && transition.Count > 0;
        public bool changedDirection => hasTransition && hasChangedDirection;

        public bool Transition(SpriteEngineExtra spriteEngine, string currentSprite)
        {
            for (var i = 0; i < transition.Count; i++)
                if (transition[i].from == currentSprite && spriteEngine.tree.SignalTrue(transition[i].condition))
                {
                    var newSprite = spriteEngine.GetSprite(transition[i].to);
                    if (newSprite != null)
                    {
                        spriteEngine.inTransition = true;
                        spriteEngine.sprite = newSprite;
                        return true;
                    }

                    return false;
                }

            return false;
        }

        public bool CheckForRandomSprite(SpriteEngineExtra spriteEngine)
        {
            if (randomSprites.Count == 0) return false;

            for (var i = 0; i < randomSprites.Count; i++)
            {
                if (i != randomIndex) continue;
                randomSprites[i].counter++;
                if (randomSprites[i].switchSprite)
                {
                    randomSprites[i].counter = 0;
                    randomIndex = Mathf.Clamp(randomIndex + 1, 0, randomSprites.Count - 1);
                    spriteEngine.SetRandomSprite(randomSprites[i].name);
                    return true;
                }
            }

            return false;
        }

        public void SetProperties(int frameIndex, bool firstFrame = false)
        {
            for (var i = 0; i < property.Count; i++) property[i].SetState(frameIndex, firstFrame);
        }

        public void ResetProperties()
        {
            for (var i = 0; i < property.Count; i++) property[i].ResetToOriginalState();
        }

        public void InterpolateProperties(int frameIndex, float counter)
        {
            for (var i = 0; i < property.Count; i++)
                if (property[i].interpolate)
                    property[i].Interpolate(frameIndex, frame[frameIndex].rate, counter);
        }

        public void ClearRandomSprites()
        {
            randomIndex = 0;
            for (var i = 0; i < randomSprites.Count; i++) randomSprites[i].counter = 0;
        }

        public void ClearFrameEvents(int frameIndex)
        {
            frame[frameIndex].ClearFrameEvents();
        }

        public void TriggerFrameEvents(int frameIndex, float percent)
        {
            frame[frameIndex].TriggerFrameEvents(percent);
        }

        #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] private bool active;
        [SerializeField] [HideInInspector] public int frameIndex;
        [SerializeField] [HideInInspector] private int signalIndex;
        [SerializeField] [HideInInspector] private float globalRate = 10;
        [SerializeField] [HideInInspector] public bool transitionFoldOut;
        [SerializeField] [HideInInspector] public bool addTransition;
        [SerializeField] [HideInInspector] public bool deleteTransition;
#pragma warning restore 0414
#endif

        #endregion
    }

    [Serializable]
    public class FrameExtra
    {
        [SerializeField] public float rate;
        [SerializeField] public Sprite sprite;
        [SerializeField] public List<FrameEvents> events = new();

        public void ClearFrameEvents()
        {
            for (var i = 0; i < events.Count; i++) events[i].activated = false;
        }

        public void TriggerFrameEvents(float percent)
        {
            for (var i = 0; i < events.Count; i++)
                if (!events[i].activated && percent >= events[i].atPercent)
                {
                    events[i].activated = true;
                    events[i].frameEvent.Invoke();
                }
        }

        #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] private bool add;
        [SerializeField] [HideInInspector] private bool delete;
        [SerializeField] [HideInInspector] private bool active;
        [SerializeField] [HideInInspector] private bool eventFoldOut;
        [SerializeField] [HideInInspector] private int signalIndex = 1;
#pragma warning restore 0414
#endif

        #endregion
    }

    [Serializable]
    public class FrameEvents
    {
        [SerializeField] public UnityEvent frameEvent;
        [SerializeField] public float atPercent;
        [NonSerialized] public bool activated;

        #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] private bool add;
        [SerializeField] [HideInInspector] private bool delete;
        [SerializeField] [HideInInspector] private bool eventFoldOut;
#pragma warning restore 0414
#endif

        #endregion
    }

    [Serializable]
    public class RandomSprites
    {
        [SerializeField] public string name;
        [SerializeField] public int time = 10;
        [NonSerialized] public int counter;
        public bool switchSprite => counter >= time;
    }
}