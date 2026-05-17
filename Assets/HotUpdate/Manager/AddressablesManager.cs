using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace HotUpdate.Manager
{
    public class AddressablesInfo
    {
        public uint count;
        public AsyncOperationHandle handle;

        public AddressablesInfo(AsyncOperationHandle handle)
        {
            this.handle = handle;
            count = 1;
        }
    }

    public class AddressablesManager
    {
        private readonly Dictionary<string, AddressablesInfo> resDic = new();

        private AddressablesManager()
        {
        }

        public static AddressablesManager Instance { get; } = new();

        #region Synchronous Loading Method

        /// <summary>
        ///     Load an Addressables asset synchronously. This will block the caller until completion.
        /// </summary>
        public T LoadAssetSynchronously<T>(string name) where T : class
        {
            var keyName = name + "_" + typeof(T).Name;

            if (resDic.ContainsKey(keyName))
            {
                resDic[keyName].count++;
                Debug.Log(keyName + " already loaded (sync), ref count: " + resDic[keyName].count);
                return resDic[keyName].handle.Convert<T>().Result;
            }

            var handle = Addressables.LoadAssetAsync<T>(name);
            var result = handle.WaitForCompletion(); // This blocks until the load finishes.

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var info = new AddressablesInfo(handle);
                resDic.Add(keyName, info);
                return result;
            }

            Debug.LogError($"Synchronous load failed: {keyName}");
            return null;
        }

        #endregion

        #region New Task-based (async/await) Methods

        public async Task<T> LoadAssetTaskAsync<T>(string name) where T : class
        {
            var keyName = name + "_" + typeof(T).Name;

            if (resDic.ContainsKey(keyName))
            {
                resDic[keyName].count++;
                var handle = resDic[keyName].handle.Convert<T>();
                return await handle.Task;
            }

            var newHandle = Addressables.LoadAssetAsync<T>(name);
            var result = await newHandle.Task;

            if (newHandle.Status == AsyncOperationStatus.Succeeded)
            {
                var info = new AddressablesInfo(newHandle);
                resDic.Add(keyName, info);
                return result;
            }

            Debug.LogError($"Load failed: {keyName}");
            return null;
        }

        public async Task<IList<T>> LoadAssetsByLabelTaskAsync<T>(string label) where T : class
        {
            var keyName = "label_" + label + "_" + typeof(T).Name;

            if (resDic.ContainsKey(keyName))
            {
                resDic[keyName].count++;
                var handle = resDic[keyName].handle.Convert<IList<T>>();
                return await handle.Task;
            }

            var newHandle = Addressables.LoadAssetsAsync<T>(label, null);
            var result = await newHandle.Task;

            if (newHandle.Status == AsyncOperationStatus.Succeeded)
            {
                var info = new AddressablesInfo(newHandle);
                resDic.Add(keyName, info);
                return result;
            }

            Debug.LogError($"Load by label failed: {keyName}");
            return null;
        }

        #endregion

        #region Existing Callback-based Methods

        public void LoadAssetAsync<T>(string name, Action<AsyncOperationHandle<T>> callBack)
        {
            var keyName = name + "_" + typeof(T).Name;
            AsyncOperationHandle<T> handle;

            if (resDic.ContainsKey(keyName))
            {
                handle = resDic[keyName].handle.Convert<T>();
                resDic[keyName].count += 1;

                if (handle.IsDone)
                    callBack(handle);
                else
                    handle.Completed += obj =>
                    {
                        if (obj.Status == AsyncOperationStatus.Succeeded)
                            callBack(obj);
                    };
                return;
            }

            handle = Addressables.LoadAssetAsync<T>(name);
            handle.Completed += obj =>
            {
                if (obj.Status == AsyncOperationStatus.Succeeded)
                {
                    var info = new AddressablesInfo(obj);
                    resDic.Add(keyName, info);
                    callBack(obj);
                }
                else
                {
                    Debug.LogWarning(keyName + " load failed");
                }
            };
        }

        public void Release<T>(string name)
        {
            var keyName = name + "_" + typeof(T).Name;
            if (resDic.ContainsKey(keyName))
            {
                resDic[keyName].count -= 1;
                if (resDic[keyName].count == 0)
                {
                    var handle = resDic[keyName].handle;
                    Addressables.Release(handle);
                    resDic.Remove(keyName);
                    Debug.Log(keyName + " released");
                }
            }
        }

        public void Clear()
        {
            foreach (var item in resDic.Values) Addressables.Release(item.handle);
            resDic.Clear();
            AssetBundle.UnloadAllAssetBundles(true);
            Resources.UnloadUnusedAssets();
            GC.Collect();
            Debug.Log("AddressablesManager cleared all cached resources");
        }

        #endregion
    }
}
