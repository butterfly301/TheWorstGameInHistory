using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace TwoBitMachines
{
    public static class Util
    {
        public const float a45 = 0.70710678118f;
        public static int empty = -1;
        public static Camera camera;
        private static readonly Vector2[] normals90 = { Vector2.up, Vector2.down, Vector2.right, Vector2.left };

        private static readonly Vector2[] normals45 =
        {
            Vector2.up, Vector2.down, Vector2.right, Vector2.left, new(a45, a45), new(-a45, a45), new(-a45, -a45),
            new(a45, -a45)
        };

        public static List<string> templist = new();
        public static int UILayer;
        public static string sceneName => SceneManager.GetActiveScene().name;

        public static Color GetColor(string hex)
        {
            if (hex == "")
                return Color.white;
            hex = hex.Replace("0x", ""); //in case the string is formatted 0xFFFFFF
            hex = hex.Replace("#", ""); //in case the string is formatted #FFFFFF
            byte a = 255; //assume fully visible unless specified in hex
            var r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            var g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            var b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            //Only use alpha if the string has enough characters
            if (hex.Length == 8) a = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);
            return new Color32(r, g, b, a);
        }

        public static void InitUtil()
        {
            camera = Camera.main;
            UILayer = LayerMask.NameToLayer("UI");
        }

        public static void ConfigureSpriteDimensions(Sprite sprite, ref Rect spriteRect, ref Texture2D spriteTexture,
            ref Vector2 size)
        {
            if (sprite == null)
                return;

            size = sprite.rect.size;
            spriteRect = sprite.rect;
            spriteTexture = sprite.texture;

            spriteRect.xMin /= spriteTexture.width;
            spriteRect.xMax /= spriteTexture.width;
            spriteRect.yMin /= spriteTexture.height;
            spriteRect.yMax /= spriteTexture.height;
            if (size.x <= 0)
                size.x = 1;
            if (size.y <= 0)
                size.y = 1;
        }

        public static void ConfigureSpriteDimensions(Sprite sprite, out Rect spriteRect, out Vector2 size)
        {
            size = Vector2.one;
            spriteRect = new Rect();
            if (sprite == null)
                return;

            size = sprite.rect.size;
            spriteRect = sprite.rect;
            var spriteTexture = sprite.texture;

            spriteRect.xMin /= spriteTexture.width;
            spriteRect.xMax /= spriteTexture.width;
            spriteRect.yMin /= spriteTexture.height;
            spriteRect.yMax /= spriteTexture.height;
            if (size.x <= 0)
                size.x = 1;
            if (size.y <= 0)
                size.y = 1;
        }

        public static Vector2 MousePosition(Vector2 defaultPosition = default)
        {
            if (camera != null)
                return camera.ScreenToWorldPoint(Input.mousePosition);
            return defaultPosition;
        }

        public static Vector2 MousePositionAny(Vector2 defaultPosition = default)
        {
            if (Camera.main != null)
                return Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return defaultPosition;
        }

        public static Vector2 MousePositionUI(Canvas canvas)
        {
            //https://answers.unity.com/questions/849117/46-ui-image-follow-mouse-position.html
            if (canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                return Input.mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
                Input.mousePosition, canvas.worldCamera, out var position);
            return canvas.transform.TransformPoint(position);
        }

        public static Vector2 WorldToScreenPoint(Vector2 point)
        {
            if (camera != null)
                return camera.WorldToScreenPoint(point);
            return point;
        }

        public static Vector3 FlipXSign(Vector3 value, float sign)
        {
            value.x = Mathf.Abs(value.x) * sign;
            return value;
        }

        public static string[] GetFilesPath(string path, string extension, bool replaceExtension = false)
        {
            var levelDirectoryPath = new DirectoryInfo(path);
            var fileInfo = levelDirectoryPath.GetFiles("*.*", SearchOption.AllDirectories);
            var asset = new List<string>();
            for (var i = 0; i < fileInfo.Length; i++)
                if (fileInfo[i].Extension == extension)
                {
                    if (replaceExtension)
                        asset.Add(fileInfo[i].Name.Replace(extension, ""));
                    else
                        asset.Add(fileInfo[i].Name);
                }

            var array = new string[asset.Count];
            for (var i = 0; i < array.Length; i++)
                array[i] = asset[i];
            return array;
        }

        public static void GetFileNames(string filePath, List<string> list, bool clear = true)
        {
            if (filePath == "") return;
            if (clear) list.Clear();
            var files = Directory.GetFiles(filePath);

            for (var i = 0; i < files.Length; i++)
            {
                var fileName = Path.GetFileName(files[i]).Split('.')[0];
                if (!list.Contains(fileName)) list.Add(fileName); // avoid duplicates
            }
        }

        public static void GetFileNames(string directoryName, string assetPath, List<string> list, bool clear = true)
        {
            var filePath = directoryName == "" ? assetPath : SearchDirectory(directoryName) + assetPath;

            // Check if the filePath is valid
            if (string.IsNullOrEmpty(filePath) ||
                !Directory.Exists(filePath)) return; // Exit if the path is invalid or does not exist

            // Get files from the directory
            string[] files;
            try
            {
                files = Directory.GetFiles(filePath);
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., log the error
                //Debug.LogError($"Error accessing directory: {ex.Message}");
                return; // Exit on error
            }
            // if (filePath == "")
            // {
            //         return;
            // }
            // string[] files = Directory.GetFiles(filePath); // get error here

            if (clear) list.Clear();
            for (var i = 0; i < files.Length; i++)
            {
                var fileName = Path.GetFileName(files[i]).Split('.')[0];
                if (!list.Contains(fileName)) list.Add(fileName); // avoid duplicates
            }
        }

        public static string[] GetFileNames(string directoryName, string assetPath)
        {
            var list = new List<string>();
            GetFileNames(directoryName, assetPath, list);
            return list.ToArray();
        }

        public static void GetFolderStructure(string directoryName, string assetPath, string folderName,
            List<string> list, bool clear = true, bool includeFolder = false)
        {
            if (assetPath == "") return;
            if (clear) list.Clear();
            var filePath = directoryName == "" ? assetPath : SearchDirectory(directoryName) + assetPath;
            var paths = DirectorySearch(filePath);

            for (var i = 0; i < paths.Count; i++)
            {
                if (paths[i].Contains(".meta")) continue;
                var folders = paths[i].Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                for (var index = 0; index < folders.Length; index++)
                    if (folders[index] == folderName)
                    {
                        var pathName = new StringBuilder();
                        if (includeFolder) pathName.Append(folderName + "/");
                        for (var f = index; f < folders.Length; f++)
                            if (f == folders.Length - 1)
                                pathName.Append(folders[f].Split('.')[0]); // last , remove .cs
                            else if (folders[f] != folderName) pathName.Append(folders[f] + "/");

                        list.Add(pathName.ToString());
                    }
            }
        }

        public static Texture2D GetIcon(this List<Texture2D> list, string name)
        {
            for (var i = 0; i < list.Count; i++)
                if (list[i] != null && list[i].name == name)
                    return list[i];
            return new Texture2D(0, 0);
        }

        private static List<string> DirectorySearch(string sDir)
        {
            var files = new List<string>();
            try
            {
                foreach (var f in Directory.GetFiles(sDir)) files.Add(f);
                foreach (var d in Directory.GetDirectories(sDir)) files.AddRange(DirectorySearch(d));
            }
            catch (Exception excpt)
            {
                //Debug.Log(excpt.Message);
            }

            return files;
        }

        public static string SearchDirectory(string parentDirectory)
        {
            var allPaths = Directory.GetDirectories("Assets", "*", SearchOption.AllDirectories);
            for (var i = 0; i < allPaths.Length; i++)
                if (allPaths[i].EndsWith(parentDirectory))
                    return allPaths[i].Replace("\\", "/");
            return "";
        }

        public static void LookAtPosition(Transform transform, Vector2 position, float offset = 0)
        {
            var relative = transform.InverseTransformPoint(position);
            var angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
            transform.Rotate(0, 0, -angle + offset);
        }

        public static void OverrideIfBigger(ref Vector2 oldValue, Vector2 newValue)
        {
            if (newValue.x * newValue.x > oldValue.x * oldValue.x) oldValue.x = newValue.x;
            if (newValue.y * newValue.y > oldValue.y * oldValue.y) oldValue.y = newValue.y;
        }

        public static void SetLayer(Camera camera, string layerName)
        {
            if (camera == null)
                return;
            var layer = LayerMask.NameToLayer(layerName);
            if (layer >= 0 && layer <= 31)
                camera.cullingMask |= 1 << layer;
        }

        public static void SetLayer(Camera camera, int layer)
        {
            if (camera == null)
                return;
            if (layer >= 0 && layer <= 31)
                camera.cullingMask |= 1 << layer;
        }

        //csharphelper.com/blog/2014/10/convert-between-pascal-case-camel-case-and-proper-case-in-c/
        public static string ToProperCase(string the_string)
        {
            // If there are 0 or 1 characters, just return the string.
            if (the_string == null)
                return the_string;
            if (the_string.Length < 2)
                return the_string.ToUpper();

            // Start with the first character.
            var result = the_string.Substring(0, 1).ToUpper();

            // Add the remaining characters.
            for (var i = 1; i < the_string.Length; i++)
            {
                if (char.IsUpper(the_string[i]))
                    result += " ";
                result += the_string[i];
            }

            return result;
        }

        public static void UndoLayer(Camera camera, string layerName)
        {
            if (camera == null)
                return;
            var layer = LayerMask.NameToLayer(layerName);
            if (layer >= 0 && layer <= 31)
                camera.cullingMask &= ~(1 << layer);
        }

        public static void UndoLayer(Camera camera, int layer)
        {
            if (camera == null)
                return;
            if (layer >= 0 && layer <= 31)
                camera.cullingMask &= ~(1 << layer);
        }

        public static Vector2 GetNearest90Normal(Vector2 normal)
        {
            var angle = Mathf.Infinity;
            var newNormal = normal;

            for (var i = 0; i < 4; i++)
            {
                var newAngle = Vector2.Angle(normal, normals90[i]);
                if (newAngle < angle)
                {
                    angle = newAngle;
                    newNormal = normals90[i];
                }
            }

            return newNormal;
        }

        public static Vector2 GetNearest45Normal(Vector2 normal)
        {
            var angle = Mathf.Infinity;
            var newNormal = normal;

            for (var i = 0; i < 8; i++)
            {
                var newAngle = Vector2.Angle(normal, normals45[i]);
                if (newAngle < angle)
                {
                    angle = newAngle;
                    newNormal = normals45[i];
                }
            }

            return newNormal;
        }

        public static Rect ExpandRect(this Rect rect, int expand)
        {
            return new Rect(rect)
            {
                x = rect.x - expand, y = rect.y - expand, width = rect.width + expand * 2,
                height = rect.height + expand * 2
            };
        }

        public static T IsPointerOverUI<T>() where T : class
        {
            var raycastResults = GetEventSystemRaycastResults();
            for (var index = 0; index < raycastResults.Count; index++)
            {
                var results = raycastResults[index];
                if (results.gameObject.layer == UILayer)
                {
                    var slot = results.gameObject.GetComponent<T>();
                    if (slot != null)
                        return slot;
                }
            }

            return null;
        }

        public static bool IsPointerOverAnyUI()
        {
            var raycastResults = GetEventSystemRaycastResults();
            for (var index = 0; index < raycastResults.Count; index++)
            {
                var results = raycastResults[index];
                if (results.gameObject.layer == UILayer) return true;
            }

            return false;
        }

        private static List<RaycastResult> GetEventSystemRaycastResults()
        {
            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            var raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            return raysastResults;
        }

        public static string GetSceneName(int i)
        {
            return Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
        }

        public static int SceneCount()
        {
            return SceneManager.sceneCountInBuildSettings;
        }

        public static float X(float speed = 1f)
        {
            return Input.GetKey(KeyCode.A) ? -speed : Input.GetKey(KeyCode.D) ? speed : 0;
        }
    }

    public enum RotateToType
    {
        RotateUp,
        RotateToNearest90
    }
}