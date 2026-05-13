using System;
using System.IO;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TwoBitMachines
{
    public class Storage
    {
        public static bool encrypt = false;
        public static string path => Path.Combine(Application.persistentDataPath, "TwoBitMachines");

#if UNITY_EDITOR
        [MenuItem("Window/Flare Save Folder")]
        private static void ShowSaveFolder()
        {
            EditorUtility.RevealInFinder(path);
        }
#endif

        public static bool Delete(string folder, string key)
        {
            var currentPath = Path.Combine(Path.Combine(path, folder), key);
            if (!File.Exists(currentPath))
            {
                Debug.LogWarning("Path does not exist: " + currentPath);
                return false;
            }

            File.Delete(currentPath);
            return true;
        }

        public static bool DeleteAll(string folder)
        {
            var currentPath = Path.Combine(path, folder);
            if (Directory.Exists(currentPath)) Directory.Delete(currentPath, true);
            Directory.CreateDirectory(currentPath);
            return true;
        }

        public static void Save(object data, string folder, string key)
        {
            if (data == null || string.IsNullOrWhiteSpace(folder) || string.IsNullOrWhiteSpace(key))
            {
                Debug.LogError("Invalid input parameters.");
                return;
            }

            var jsonData = JsonUtility.ToJson(data);
            var directory = Path.Combine(path, folder);
            var filePath = Path.Combine(directory, key);

            try
            {
                Directory.CreateDirectory(directory);
                using (var writer = new StreamWriter(filePath, false))
                {
                    writer.Write(EncryptDecrypt(jsonData));
                    writer.Flush();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving file: {e.Message}");
            }
        }

        public static T Load<T>(T defaultValue, string folder, string key) where T : class
        {
            if (string.IsNullOrWhiteSpace(folder) || string.IsNullOrWhiteSpace(key))
            {
                Debug.LogError("Invalid input parameters.");
                return default;
            }

            string data = null;
            var currentPath = Path.Combine(Path.Combine(path, folder), key);

            if (!File.Exists(currentPath)) return defaultValue;

            try
            {
                using (var reader = new StreamReader(currentPath, false))
                {
                    data = reader.ReadToEnd();
                    data = EncryptDecrypt(data);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error reading file '{currentPath}': {e.Message}");
                return defaultValue;
            }

            if (data == null) return defaultValue;

            try
            {
                return JsonUtility.FromJson<T>(data);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error parsing JSON data from file '{currentPath}': {e.Message}");
                return defaultValue;
            }
        }

        public static string EncryptDecrypt(string data)
        {
            if (!encrypt)
                return data;

            var input = new StringBuilder(data);
            var output = new StringBuilder(data.Length);
            char character;

            for (var i = 0; i < data.Length; i++)
            {
                character = input[i];
                character = (char)(character ^ 333);
                output.Append(character);
            }

            return output.ToString();
        }

        //   https: //learn.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
        public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive = true)
        {
            sourceDir = Path.Combine(path, sourceDir);
            destinationDir = Path.Combine(path, destinationDir);

            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists) throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            var dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (var file in dir.GetFiles())
            {
                var targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
                foreach (var subDir in dirs)
                {
                    var newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir);
                }
        }
    }
}


// public static T Load<T> (T defaultValue, string folder, string key) where T : class
// {
//         string data = null;
//         string currentPath = Path.Combine(Path.Combine(path, folder), key);

//         if (!File.Exists(currentPath))
//         {
//                 return defaultValue;
//         }

//         using (StreamReader reader = new StreamReader(currentPath, false))
//         {
//                 try
//                 {
//                         data = reader.ReadToEnd();
//                         data = EncryptDecrypt(data);
//                 }
//                 catch
//                 {
//                         data = null;
//                 }
//         }
//         return data == null ? defaultValue : JsonUtility.FromJson<T>(data);
// }


// public static void Save (object data, string folder, string key)
// {
//         string jsonData = JsonUtility.ToJson(data);
//         string directory = Path.Combine(path, folder);
//         string filePath = Path.Combine(directory, key);

//         try
//         {
//                 Directory.CreateDirectory(directory);
//         }
//         catch
//         {

//         }

//         using (StreamWriter writer = new StreamWriter(filePath, false))
//         {
//                 try
//                 {
//                         writer.Write(EncryptDecrypt(jsonData));
//                         writer.Flush();
//                 }
//                 catch
//                 {

//                 }
//         }
// }