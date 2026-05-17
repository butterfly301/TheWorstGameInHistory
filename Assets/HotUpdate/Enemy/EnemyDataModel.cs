using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotUpdate.Manager;
using HotUpdate.Utility;
using QFramework;
using UnityEngine;

namespace HotUpdate.Enemy
{
    // 这两个数据结构保持不变
    [Serializable]
    public class EnemyDictionaryData
    {
        public List<EnemyData> enemies = new();
    }

[Serializable]
    public class EnemyData
    {
        public string EnemyName;
    }

public class EnemyDataModel : AbstractModel, IEnemyDataModel
    {
        // 1. 改变内部存储结构，直接将敌人名字映射到数据，更高效
        private readonly Dictionary<string, EnemyData> _enemyDatabase = new();
        public bool IsDataLoaded { get; private set; }

public async Task LoadDataAsync()
        {
            if (IsDataLoaded) return;

// 2. 使用标签加载所有敌人数据JSON文件
            var enemyJsonAssets =
                await AddressablesManager.Instance.LoadAssetsByLabelTaskAsync<TextAsset>("EnemyDataJSONs");

if (enemyJsonAssets != null)
            {
                // 3. 遍历加载的所有文件，并将数据合并到数据库中
                foreach (var jsonAsset in enemyJsonAssets)
                {
                    var dictionaryData = JsonUtility.FromJson<EnemyDictionaryData>(jsonAsset.text);
                    if (dictionaryData != null)
                        foreach (var enemyData in dictionaryData.enemies)
                            if (!_enemyDatabase.ContainsKey(enemyData.EnemyName))
                                _enemyDatabase.Add(enemyData.EnemyName, enemyData);
                            else
                                Debug.LogWarning(
                                    $"[EnemyDataModel] Duplicate enemy name found: {enemyData.EnemyName} in file {jsonAsset.name}. Ignoring duplicate.");
                }

IsDataLoaded = true;
                Debug.Log(
                    $"[EnemyDataModel] Loaded {_enemyDatabase.Count} total enemy data entries from {enemyJsonAssets.Count} files.");
            }
            else
            {
                Debug.LogError("[EnemyDataModel] Failed to load assets with label 'EnemyDataJSONs'!");
            }
        }

// 4. 重构查询方法，现在更高效
        public string GetEnemyAddress(string enemyName)
        {
            if (!IsDataLoaded)
            {
                Debug.LogError("[EnemyDataModel] Enemy data is not initialized!");
                return null;
            }

if (_enemyDatabase.TryGetValue(enemyName, out var data))
                return AddressableKeys.GetPrefabs_Enemy(data.EnemyName);

Debug.LogWarning($"[EnemyDataModel] Enemy address not found for: {enemyName}");
            return null;
        }

protected override void OnInit()
        {
        }
    }
}
