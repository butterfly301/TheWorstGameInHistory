using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HotUpdate.Manager
{
    public class ObjectPoolManager : MonoSingleton<ObjectPoolManager>
    {
        private readonly Dictionary<GameObject, PoolTag> activeObjects = new();
        private readonly Dictionary<PoolTag, Queue<GameObject>> poolDictionary = new();
        private readonly Dictionary<PoolTag, GameObject> prefabDictionary = new();

        protected override void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            base.OnDestroy();
        }

        public override void OnSingletonInit()
        {
            base.OnSingletonInit();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Clear all pools and dictionaries when a new scene is loaded
            // to prevent references to destroyed objects.
            poolDictionary.Clear();
            prefabDictionary.Clear();
            activeObjects.Clear();
        }

        public void CreatePool(PoolTag poolTag, GameObject prefab, int initialSize)
        {
            if (poolDictionary.ContainsKey(poolTag)) return;

            poolDictionary[poolTag] = new Queue<GameObject>();
            prefabDictionary[poolTag] = prefab;

            for (var i = 0; i < initialSize; i++)
            {
                var obj = Instantiate(prefab);
                obj.SetActive(false);
                poolDictionary[poolTag].Enqueue(obj);
            }
        }

        public GameObject SpawnFromPool(PoolTag poolTag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(poolTag))
            {
                Debug.LogWarning("Pool with tag " + poolTag + " doesn't exist.");
                return null;
            }

            GameObject objectToSpawn = null;

            // Dequeue until a valid (non-destroyed) object is found
            while (poolDictionary[poolTag].Count > 0)
            {
                objectToSpawn = poolDictionary[poolTag].Dequeue();
                if (objectToSpawn != null) break; // Found a valid object
                // If null, the object was destroyed; loop continues to the next one.
            }

            // If the pool was empty or only contained destroyed objects, create a new one
            if (objectToSpawn == null) objectToSpawn = Instantiate(prefabDictionary[poolTag]);

            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
            objectToSpawn.SetActive(true);

            activeObjects[objectToSpawn] = poolTag;

            return objectToSpawn;
        }

        public void ReturnToPool(GameObject objectToReturn)
        {
            if (objectToReturn == null) return; // Safety check

            if (activeObjects.TryGetValue(objectToReturn, out var poolTag))
            {
                objectToReturn.SetActive(false);
                poolDictionary[poolTag].Enqueue(objectToReturn);
                activeObjects.Remove(objectToReturn);
            }
            else
            {
                // This can happen if an object is returned to the pool twice.
                // We'll just destroy it to be safe.
                Destroy(objectToReturn);
            }
        }

        public void ReturnToPool(PoolTag poolTag)
        {
            // 创建一个列表来存储需要回收的对象，以避免在遍历时修改 activeObjects 字典
            var objectsToReturn = new List<GameObject>();
            foreach (var pair in activeObjects)
                if (pair.Value == poolTag)
                    objectsToReturn.Add(pair.Key);

            // 遍历列表，将对象逐个返还给对象池
            foreach (var obj in objectsToReturn) ReturnToPool(obj);
        }


        public void ReturnAllToPool()
        {
            // Create a copy of the keys to avoid modification during iteration
            var objectsToReturn = new List<GameObject>(activeObjects.Keys);
            foreach (var obj in objectsToReturn) ReturnToPool(obj);
        }
    }
}