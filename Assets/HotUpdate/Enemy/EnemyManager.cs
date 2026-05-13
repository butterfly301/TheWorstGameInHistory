using System.Collections.Generic;
using HotUpdate.Manager;
using HotUpdate.Utility;
using QFramework;
using UnityEngine;

namespace HotUpdate.Enemy
{
    public class EnemyManager : MonoSingleton<EnemyManager>
    {
        private Dictionary<string, List<GameObject>> _activeEnemies;
        private Dictionary<string, GameObject> _enemyPrefabCache;
        private bool _isInitialized;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Cleanup();
        }

        /// <summary>
        ///     同步初始化，会阻塞直到所有资源加载完成
        /// </summary>
        public void Init(string[] enemyNames)
        {
            if (_isInitialized) return;

            Debug.Log("[EnemyManager] Starting synchronous initialization...");

            _enemyPrefabCache = new Dictionary<string, GameObject>();
            _activeEnemies = new Dictionary<string, List<GameObject>>();

            // 1. 同步加载敌人数据并存储
            var jsonAsset =
                AddressablesManager.Instance.LoadAssetSynchronously<TextAsset>(
                    AddressableKeys.Data.Enemy.EnemyData_Json);
            if (jsonAsset == null)
            {
                Debug.LogError("[EnemyManager] Failed to load EnemyData.json synchronously. Initialization aborted.");
                return;
            }

            // 2. 同步加载所有需要的敌人预制体
            if (enemyNames != null)
                foreach (var enemyName in enemyNames)
                    if (!_enemyPrefabCache.ContainsKey(enemyName))
                    {
                        var prefab =
                            AddressablesManager.Instance.LoadAssetSynchronously<GameObject>(
                                AddressableKeys.Prefabs.Enemy.GetEnemy(enemyName));
                        if (prefab != null) _enemyPrefabCache[enemyName] = prefab;
                    }

            _isInitialized = true;
            Debug.Log("[EnemyManager] Synchronous initialization complete.");
        }

        public void SpawnEnemy(string enemyName, Vector3 position, Quaternion rotation)
        {
            if (!_isInitialized)
            {
                Debug.LogError(
                    $"[EnemyManager] Is not initialized! Cannot spawn '{enemyName}'. Make sure Init() is called first.");
                return;
            }

            if (_enemyPrefabCache.TryGetValue(enemyName, out var prefab))
            {
                var enemyInstance = Instantiate(prefab, position, rotation);
                if (!_activeEnemies.ContainsKey(enemyName)) _activeEnemies[enemyName] = new List<GameObject>();
                _activeEnemies[enemyName].Add(enemyInstance);
            }
            else
            {
                Debug.LogWarning(
                    $"[EnemyManager] Prefab not found for '{enemyName}'. Was it included in the Init() call?");
            }
        }

        /// <summary>
        ///     **恢复并完善了 Cleanup 逻辑**
        ///     清理当前场景的敌人实例和已加载的预制体资源
        /// </summary>
        public void Cleanup()
        {
            if (!_isInitialized) return;

            // 销毁所有活跃的敌人实例
            if (_activeEnemies != null)
            {
                foreach (var enemyList in _activeEnemies.Values)
                    // 从后往前遍历删除，避免修改列表时出错
                    for (var i = enemyList.Count - 1; i >= 0; i--)
                        if (enemyList[i] != null)
                            Destroy(enemyList[i]);

                _activeEnemies.Clear();
            }

            // 释放已加载的敌人预制体 Addressables 资源
            if (_enemyPrefabCache != null)
            {
                foreach (var enemyName in _enemyPrefabCache.Keys)
                {
                    string address = AddressableKeys.Prefabs.Enemy.GetEnemy(enemyName);
                    if (!string.IsNullOrEmpty(address)) AddressablesManager.Instance.Release<GameObject>(address);
                }

                _enemyPrefabCache.Clear();
            }

            _isInitialized = false;
        }
    }
}