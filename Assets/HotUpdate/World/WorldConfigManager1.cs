using System.Collections.Generic;
using HotUpdate.Character;
using HotUpdate.Interface;
using HotUpdate.Manager;
using HotUpdate.Utility;
using QFramework;
using UnityEngine;

namespace HotUpdate.World
{
    public class WorldConfigManager1 : MonoSingleton<WorldConfigManager1>,IAutoBind
    {
        [SerializeField] private Transform dialogueBubbles;
        private Transform[] dialogueBubbleTransforms;
        
        [SerializeField] private Transform enemySpawnPoints;
        private Transform[] enemySpawnPointTransforms;
        
        private GameObject[] npcObjs;
        [SerializeField]private Transform npcs;
        private Transform[] npcTransforms;

        private void Awake()
        {
            dialogueBubbleTransforms = GetAllChildren(dialogueBubbles);
            npcTransforms = GetAllChildren(npcs);
            enemySpawnPointTransforms = GetAllChildren(enemySpawnPoints);

            InitializeNpCs();
            InitializeTriggerDialogueBubbles();
            InitializeEnemySpawnPoints();
        }

        public Transform[] GetAllChildren(Transform parent)
        {
            var children = new List<Transform>();

            for (var i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                children.Add(child);
            }

            return children.ToArray();
        }

        private void InitializeNpCs()
        {
            npcObjs = new GameObject[npcTransforms.Length];
            for (var i = 0; i < npcTransforms.Length; i++)
            {
                var index = i;
                var npcName = npcTransforms[index].name;
                AddressablesManager.Instance.LoadAssetAsync<GameObject>(
                    AddressableKeys.GetPrefabs_Character_NPC(npcName),
                    handle =>
                    {
                        var go = Instantiate(handle.Result, npcTransforms[index].position, Quaternion.identity);
                        npcObjs[index] = go;
                        var cr = npcObjs[index].GetComponent<CharacterReference>();
                        cr.Init();
                    });
            }
        }

        private void InitializeTriggerDialogueBubbles()
        {
            for (var i = 0; i < dialogueBubbleTransforms.Length; i++)
            {
                var index = i;
                AddressablesManager.Instance.LoadAssetAsync<GameObject>(
                    AddressableKeys.GetPrefabs_TriggerDialogueBubble(index.ToString()),
                    handle =>
                    {
                        Instantiate(handle.Result, dialogueBubbleTransforms[index].position, Quaternion.identity);
                    });
            }
        }

        private void InitializeEnemySpawnPoints()
        {
        }

        public Transform GetEnemySpawnPoints(int index)
        {
            return enemySpawnPointTransforms[index];
        }
    }
}
