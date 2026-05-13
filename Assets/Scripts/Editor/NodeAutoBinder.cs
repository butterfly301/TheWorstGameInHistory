#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using HotUpdate.Interface;

/// <summary>
/// 自动绑定工具：当物体改名时，自动绑定到父节点中继承IAutoBind的组件的同名字段
/// 支持多种命名风格：SpeakerNameText -> speakerNameText
/// </summary>
[InitializeOnLoad]
public class NodeAutoBinder
{
    static NodeAutoBinder()
    {
        ObjectFactory.componentWasAdded -= OnComponentAdded;
        ObjectFactory.componentWasAdded += OnComponentAdded;
    }

    private static void OnComponentAdded(Component component)
    {
        // 组件变化时触发
    }

    [MenuItem("GameObject/Auto Bind to Node", false, 0)]
    private static void AutoBindToNode(MenuCommand command)
    {
        GameObject selectedObj = command.context as GameObject;
        if (selectedObj != null)
        {
            TryAutoBind(selectedObj);
        }
    }

    /// <summary>
    /// 检查组件是否实现了IAutoBind接口
    /// </summary>
    private static bool IsIAutoBindComponent(Component component)
    {
        if (component == null) return false;

        try
        {
            Type componentType = component.GetType();
            Type IAutoBindType = Type.GetType("HotUpdate.Interface.IAutoBind, HotUpdate");
            if (IAutoBindType == null) return false;

            return IAutoBindType.IsAssignableFrom(componentType);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 尝试自动绑定物体到父节点的字段
    /// </summary>
    public static void TryAutoBind(GameObject obj)
    {
        if (obj == null) return;

        string objectName = obj.name;
        Transform current = obj.transform.parent;

        while (current != null)
        {
            Component[] components = current.GetComponents<Component>();

            foreach (var component in components)
            {
                if (IsIAutoBindComponent(component))
                {
                    if (TryBindToComponent(component, objectName, obj))
                    {
                        Debug.Log($"自动绑定成功: {objectName} -> {component.GetType().Name}");
                        EditorUtility.SetDirty(component);
                        AssetDatabase.SaveAssets();
                        return;
                    }
                }
            }

            current = current.parent;
        }
    }

    /// <summary>
    /// 尝试将物体绑定到组件的同名字段
    /// </summary>
    private static bool TryBindToComponent(Component targetComponent, string objectName, GameObject objToBind)
    {
        if (targetComponent == null || string.IsNullOrEmpty(objectName)) return false;

        Type targetType = targetComponent.GetType();

        // 尝试多种命名风格匹配
        FieldInfo fieldInfo = null;
        string[] possibleFieldNames = GetPossibleFieldNames(objectName);

        foreach (string possibleName in possibleFieldNames)
        {
            fieldInfo = GetFieldInfo(targetType, possibleName);
            if (fieldInfo != null) break;
        }

        if (fieldInfo == null) return false;

        object valueToBind = FindValueForField(fieldInfo, objToBind);
        if (valueToBind == null) return false;

        return TrySetFieldValue(targetComponent, fieldInfo, valueToBind);
    }

    /// <summary>
    /// 获取可能的字段名称（支持多种命名风格）
    /// </summary>
    private static string[] GetPossibleFieldNames(string objectName)
    {
        System.Collections.Generic.List<string> names = new System.Collections.Generic.List<string>();

        // 1. 原始名称
        names.Add(objectName);

        // 2. 首字母小写 (PascalCase -> camelCase)
        if (!string.IsNullOrEmpty(objectName))
        {
            names.Add(char.ToLower(objectName[0]) + objectName.Substring(1));
        }

        // 3. 全小写
        names.Add(objectName.ToLower());

        // 4. 全大写
        names.Add(objectName.ToUpper());

        return names.ToArray();
    }

    /// <summary>
    /// 根据字段类型查找匹配的值（组件或GameObject）
    /// </summary>
    private static object FindValueForField(FieldInfo fieldInfo, GameObject objToBind)
    {
        Type fieldType = fieldInfo.FieldType;

        // 如果字段类型是GameObject
        if (fieldType == typeof(GameObject))
        {
            return objToBind;
        }

        // 如果字段类型是Component或其子类
        if (typeof(Component).IsAssignableFrom(fieldType))
        {
            // 首先尝试精确匹配
            Component exactComponent = objToBind.GetComponent(fieldType);
            if (exactComponent != null) return exactComponent;

            // 如果没有找到精确匹配，尝试查找可兼容的组件
            if (fieldType.IsInterface || fieldType == typeof(Component))
            {
                Component[] components = objToBind.GetComponents<Component>();
                foreach (var comp in components)
                {
                    if (fieldType.IsAssignableFrom(comp.GetType()))
                    {
                        return comp;
                    }
                }
            }

            // 如果字段是Transform类型，直接返回transform
            if (fieldType == typeof(Transform))
            {
                return objToBind.transform;
            }
        }

        return null;
    }

    /// <summary>
    /// 尝试设置字段值
    /// </summary>
    private static bool TrySetFieldValue(Component targetComponent, FieldInfo fieldInfo, object value)
    {
        if (fieldInfo == null || value == null) return false;

        if (!fieldInfo.FieldType.IsInstanceOfType(value)) return false;

        try
        {
            Undo.RecordObject(targetComponent, $"Auto Bind {fieldInfo.Name}");
            fieldInfo.SetValue(targetComponent, value);
            EditorUtility.SetDirty(targetComponent);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 尝试设置字段值（重载方法）
    /// </summary>
    private static bool TrySetFieldValue(Component targetComponent, string fieldName, object value)
    {
        Type targetType = targetComponent.GetType();
        FieldInfo fieldInfo = GetFieldInfo(targetType, fieldName);
        return TrySetFieldValue(targetComponent, fieldInfo, value);
    }

    /// <summary>
    /// 获取字段信息（包括私有和基类的字段）
    /// </summary>
    private static FieldInfo GetFieldInfo(Type type, string fieldName)
    {
        Type currentType = type;
        while (currentType != null)
        {
            FieldInfo[] fields = currentType.GetFields(BindingFlags.Instance |
                                                       BindingFlags.Public |
                                                       BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                bool shouldSerialize = field.IsPublic ||
                                      field.GetCustomAttribute<SerializeReference>() != null ||
                                      field.GetCustomAttribute<SerializeField>() != null;

                if (shouldSerialize && field.Name == fieldName)
                {
                    return field;
                }
            }

            currentType = currentType.BaseType;
        }

        return null;
    }
}
#endif
