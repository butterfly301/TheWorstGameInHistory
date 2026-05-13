using System;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
    [AddComponentMenu("")]
    public partial class Character : MonoBehaviour //* Has partial class Equipment
    {
        [NonSerialized] public static List<Character> aiCharacters = new();
        [NonSerialized] public static List<Character> lateAICharacters = new();
        [NonSerialized] public static List<Character> aiMovingPlatforms = new();
        [NonSerialized] public static List<WorldCollision> characters = new();
        [NonSerialized] public static List<WorldCollision> passengers = new();
        [NonSerialized] public static Dictionary<Transform, MovingPlatform> movingPlatforms = new();
        [SerializeField] public CharacterType type; // a character is simply something that interacts with the world
        [SerializeField] public WorldCollision world = new();
        [SerializeField] public AnimationSignals signals = new();
        [SerializeField] public MovingPlatform movingPlatform = new();

        [SerializeField] public bool turnOffSignals;
        [SerializeField] public bool pushBackActive;
        [SerializeField] public bool executeInLateUpdate;

        #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] public bool mainFoldOut;
#pragma warning restore 0414
#endif

        #endregion

        [NonSerialized] public Vector2 externalVelocity;

        [NonSerialized] public Vector2 initialVelocity;

        public void Start()
        {
            var newTools = GetComponentsInChildren<Tool>(true);
            for (var i = 0; i < newTools.Length; i++) RegisterTool(newTools[i]);
            OnStart();
        }

        public virtual void OnEnable()
        {
            if (type == CharacterType.MovingPlatform)
            {
                if (!aiMovingPlatforms.Contains(this)) aiMovingPlatforms.Add(this);
                if (!movingPlatforms.ContainsKey(transform)) movingPlatforms.Add(transform, movingPlatform);
            }
            else if (type == CharacterType.Regular)
            {
                if (!executeInLateUpdate && !aiCharacters.Contains(this)) aiCharacters.Add(this);
                if (executeInLateUpdate && !lateAICharacters.Contains(this)) lateAICharacters.Add(this);
                if (!characters.Contains(world)) characters.Add(world);
                if (world.useMovingPlatform && !passengers.Contains(world)) passengers.Add(world);
            }
            else
            {
                if (!executeInLateUpdate && !aiCharacters.Contains(this)) aiCharacters.Add(this);
                if (executeInLateUpdate && !lateAICharacters.Contains(this)) lateAICharacters.Add(this);
            }

            OnEnabled(true);
        }

        public virtual void OnDisable()
        {
            OnEnabled(false);
        }

        public virtual void OnDestroy()
        {
            if (type == CharacterType.MovingPlatform)
            {
                if (aiMovingPlatforms.Contains(this)) aiMovingPlatforms.Remove(this);
                if (movingPlatforms.ContainsKey(transform)) movingPlatforms.Remove(transform);
            }
            else if (type == CharacterType.Regular)
            {
                if (!executeInLateUpdate && aiCharacters.Contains(this)) aiCharacters.Remove(this);
                if (executeInLateUpdate && lateAICharacters.Contains(this)) lateAICharacters.Remove(this);
                if (characters.Contains(world)) characters.Remove(world);
                if (world.useMovingPlatform && passengers.Contains(world)) passengers.Remove(world);
            }
            else
            {
                if (!executeInLateUpdate && aiCharacters.Contains(this)) aiCharacters.Remove(this);
                if (executeInLateUpdate && lateAICharacters.Contains(this)) lateAICharacters.Remove(this);
            }
        }

        public static void ResetMovingPlatforms()
        {
            for (var i = aiMovingPlatforms.Count - 1; i >= 0; i--)
                if (aiMovingPlatforms[i] != null)
                    aiMovingPlatforms[i].movingPlatform.ResetAll();
        }

        public static void AICharacters()
        {
            for (var i = aiCharacters.Count - 1; i >= 0; i--)
                if (aiCharacters[i] == null)
                    aiCharacters.RemoveAt(i);
                else
                    aiCharacters[i].Execute();
        }

        public static void LateAICharacters()
        {
            for (var i = lateAICharacters.Count - 1; i >= 0; i--)
                if (lateAICharacters[i] == null)
                    lateAICharacters.RemoveAt(i);
                else
                    lateAICharacters[i].Execute();
        }

        public static void AIMovingPlatforms()
        {
            for (var i = aiMovingPlatforms.Count - 1; i >= 0; i--)
                if (aiMovingPlatforms[i] == null)
                    aiMovingPlatforms.RemoveAt(i);
                else
                    aiMovingPlatforms[i].Execute();
        }

        public static void ResetAllAI()
        {
            for (var i = aiCharacters.Count - 1; i >= 0; i--)
                if (aiCharacters[i] != null)
                    aiCharacters[i].ResetAI();

            for (var i = lateAICharacters.Count - 1; i >= 0; i--)
                if (lateAICharacters[i] != null)
                    lateAICharacters[i].ResetAI();

            for (var i = aiMovingPlatforms.Count - 1; i >= 0; i--)
                if (aiMovingPlatforms[i] != null)
                    aiMovingPlatforms[i].ResetAI();
        }

        public void RemoveAI()
        {
            if (type == CharacterType.MovingPlatform)
            {
                if (aiMovingPlatforms.Contains(this)) aiMovingPlatforms.Remove(this);
            }
            else
            {
                if (!executeInLateUpdate && aiCharacters.Contains(this)) aiCharacters.Remove(this);
                if (executeInLateUpdate && lateAICharacters.Contains(this)) lateAICharacters.Remove(this);
            }
        }

        public virtual void ResetAI()
        {
        }

        public virtual void OnStart()
        {
        }

        public virtual void Execute()
        {
        }

        public virtual void PostAIExecute()
        {
        }

        public virtual void OnEnabled(bool onEnable)
        {
        }

        public virtual Vector2 Velocity()
        {
            return Vector2.zero;
        }
    }

    public enum CharacterType
    {
        Regular,
        NoCollisionChecks,
        MovingPlatform
    }
}