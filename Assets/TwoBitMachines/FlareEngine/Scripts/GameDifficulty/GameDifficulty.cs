using System;
using System.Collections.Generic;
using TwoBitMachines.FlareEngine.AI;
using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
    public class GameDifficulty : MonoBehaviour
    {
        [SerializeField] public GameDifficultySave difficulty = new();

        public void Awake()
        {
            Restore();
        }

        public void Start()
        {
            difficulty.Execute();
        }

        public void OnDestroy()
        {
            Save();
        }

        public void ChangeDifficultyAndSave(int newDifficulty)
        {
            difficulty.difficulty = newDifficulty;
            Save();
        }

        public void Save()
        {
            Storage.Save(difficulty, WorldManager.saveFolder, "GameDifficulty");
        }

        public void Restore()
        {
            difficulty = Storage.Load(difficulty, WorldManager.saveFolder, "GameDifficulty");
        }

        public void Restore(string saveFolder)
        {
            difficulty = Storage.Load(difficulty, saveFolder, "GameDifficulty");
        }

        public void Save(string saveFolder)
        {
            Storage.Save(difficulty, saveFolder, "GameDifficulty");
        }

        public int DifficultyLevel()
        {
            return difficulty.difficulty;
        }
    }

    [Serializable]
    public class GameDifficultySave
    {
        [SerializeField] public int difficulty;
        [SerializeField] public List<DifficultyLevel> level = new();

        public void Execute()
        {
            for (var i = 0; i < level.Count; i++)
                if (i == difficulty)
                {
                    level[i].Execute();
                    return;
                }
        }


        #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] private int signalIndex;
        [SerializeField] [HideInInspector] private bool active;
        [SerializeField] [HideInInspector] private bool foldOut;
#pragma warning restore 0414
#endif

        #endregion
    }

    [Serializable]
    public class DifficultyLevel
    {
        [SerializeField] public List<DifficultyBehaviour> behaviour = new();

        public void Execute()
        {
            for (var i = 0; i < behaviour.Count; i++) behaviour[i].Execute();
        }

        #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] private bool add;
        [SerializeField] [HideInInspector] private bool delete;
        [SerializeField] [HideInInspector] private bool foldOut;
#pragma warning restore 0414
#endif

        #endregion
    }

    [Serializable]
    public class DifficultyBehaviour
    {
        [SerializeField] public DifficultyBehaviourType type;
        [SerializeField] public float value;

        public void Execute() // execute on start
        {
            if (type == DifficultyBehaviourType.MultiplyEnemyDamage) AIDamage.difficulty = value;
        }
    }


    public enum DifficultyBehaviourType
    {
        MultiplyEnemyDamage
    }
}