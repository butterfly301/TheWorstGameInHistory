using System;
using System.IO;
using Newtonsoft.Json;
using QFramework;
using UnityEngine;

namespace HotUpdate.Data.Utility
{
    public interface IStorage : IUtility
    {
        void Save<T>(string key, T data);
        T Load<T>(string key) where T : class;
    }

public class JsonStorage : IStorage
    {
        public void Save<T>(string key, T data)
        {
            var path = Path.Combine(Application.persistentDataPath, $"{key}.json");
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(path, json);
            Debug.Log($"游戏已保存至: {path}");
        }

public T Load<T>(string key) where T : class
        {
            var path = Path.Combine(Application.persistentDataPath, $"{key}.json");

if (!File.Exists(path)) return null;

try
            {
                var json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"读取存档失败: {e.Message}");
                return null;
            }
        }
    }
}