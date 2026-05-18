#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

    /// <summary>
    /// 监听Hierarchy窗口中物体的重命名事件，触发自动绑定
    /// </summary>
    [InitializeOnLoad]
    public class HierarchyRenameHandler
    {
        private static string previousName = string.Empty;
        private static int previousInstanceId = 0;
        private static bool isRenaming = false;
        private static double lastRenameTime = 0;

        static HierarchyRenameHandler()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItemGUI;
        }

        private static void OnHierarchyItemGUI(int instanceID, Rect selectionRect)
        {
            // 获取对应的GameObject
            GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

            if (obj == null) return;

            // 检查是否在选中的物体中
            if (Selection.Contains(instanceID))
            {
                string currentName = obj.name;

                // 如果是首次选中这个物体，只记录初始名称，不触发重命名
                if (previousInstanceId != instanceID)
                {
                    previousName = currentName;
                    previousInstanceId = instanceID;
                    return;
                }

                // 检测重命名（名字发生变化且不是首次选中）
                if (previousName != currentName &&
                    !string.IsNullOrEmpty(previousName) &&
                    !isRenaming)
                {
                    // 避免频繁触发（至少间隔0.5秒）
                    double currentTime = EditorApplication.timeSinceStartup;
                    if (currentTime - lastRenameTime > 0.5)
                    {
                        isRenaming = true;
                        lastRenameTime = currentTime;

                        // 延迟执行，确保Unity已完成重命名操作
                        EditorApplication.delayCall += () =>
                        {
                            NodeAutoBinder.TryAutoBind(obj);
                            isRenaming = false;

                            // 更新previousName为当前名称
                            previousName = obj.name;
                        };
                        return; // 提前返回，避免下面再更新previousName
                    }
                }

                previousName = currentName;
            }
            else if (previousInstanceId == instanceID)
            {
                // 取消选中时重置
                previousName = string.Empty;
                previousInstanceId = 0;
                isRenaming = false;
            }
        }
    }
#endif
