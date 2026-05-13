#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using HotUpdate.Data.Model;
using UnityEditor;
using UnityEngine;

public class SaveEditorWindow : EditorWindow
{
    private const string FileName = "TheWorstGameInHistory_Save.json";
    private SoftwareName _addSelection = SoftwareName.IceBreaker;
    private GameData _data;
    private string _path;
    private Vector2 _scroll;

    private void OnEnable()
    {
        _path = Path.Combine(Application.persistentDataPath, FileName);
        Load();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("存档路径", _path);

        // 新增：打开存档所在文件夹和定位存档文件的按钮，方便测试
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("打开存档文件夹", GUILayout.Width(140)))
        {
            var folder = Path.GetDirectoryName(_path);
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            try
            {
                Process.Start("explorer.exe", "\"" + folder + "\"");
            }
            catch
            {
                // 兜底使用 Unity 提供的方法
                EditorUtility.RevealInFinder(folder);
            }
        }

        if (GUILayout.Button("定位存档文件", GUILayout.Width(120)))
        {
            if (File.Exists(_path))
                EditorUtility.RevealInFinder(_path);
            else
                EditorUtility.DisplayDialog("提示", "存档文件不存在", "确定");
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        if (_data == null) _data = new GameData();

        // 周目编辑
        EditorGUILayout.LabelField("周目 (playThrough)");
        EditorGUILayout.BeginHorizontal();
        _data.playThrough = EditorGUILayout.IntField(_data.playThrough);
        if (GUILayout.Button("+", GUILayout.Width(30))) _data.playThrough++;
        if (GUILayout.Button("-", GUILayout.Width(30))) _data.playThrough = Mathf.Max(0, _data.playThrough - 1);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // 软件列表编辑
        EditorGUILayout.LabelField("软件列表 (software)");
        if (_data.software == null) _data.software = new List<SoftwareName>();

        var removeIndex = -1;
        for (var i = 0; i < _data.software.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(i + 1 + ".", GUILayout.Width(24));
            _data.software[i] = (SoftwareName)EditorGUILayout.EnumPopup(_data.software[i]);
            if (GUILayout.Button("删除", GUILayout.Width(60)))
                // 记录删除索引，稍后在循环外统一删除，避免在迭代时修改集合
                removeIndex = i;
            EditorGUILayout.EndHorizontal();
        }

        if (removeIndex >= 0)
        {
            if (removeIndex < _data.software.Count)
            {
                _data.software.RemoveAt(removeIndex);
            }
            else
            {
                // 防御性容错：索引越界时尝试移除最后一项
                if (_data.software.Count > 0) _data.software.RemoveAt(_data.software.Count - 1);
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("添加软件");
        EditorGUILayout.BeginHorizontal();
        _addSelection = (SoftwareName)EditorGUILayout.EnumPopup(_addSelection);
        if (GUILayout.Button("添加", GUILayout.Width(60)))
        {
            if (_data.software == null) _data.software = new List<SoftwareName>();
            if (!_data.software.Contains(_addSelection))
                _data.software.Add(_addSelection);
            else
                EditorUtility.DisplayDialog("提示", "已包含该软件", "确定");
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("保存")) Save();
        if (GUILayout.Button("重载"))
            if (EditorUtility.DisplayDialog("重载确认", "重载将放弃未保存的修改，是否继续？", "重载", "取消"))
                Load();

        if (GUILayout.Button("重置为默认"))
            if (EditorUtility.DisplayDialog("重置确认", "将重置为默认新存档，是否继续？", "重置", "取消"))
                _data = new GameData();

        EditorGUILayout.EndHorizontal();
    }

    [MenuItem("史上最垃圾的游戏/存档编辑器")]
    public static void OpenWindow()
    {
        var w = GetWindow<SaveEditorWindow>("存档编辑器");
        w.minSize = new Vector2(320, 240);
        w.Show();
    }

    private void Load()
    {
        if (File.Exists(_path))
        {
            var txt = File.ReadAllText(_path);
            // 尝试直接解析为 GameData
            try
            {
                _data = JsonUtility.FromJson<GameData>(txt);
            }
            catch
            {
                _data = null;
            }

            // 如果直接解析失败，可能存的是 BindableProperty<GameData> 或者是小写字段名
            if (_data == null)
                try
                {
                    var wrapper = JsonUtility.FromJson<WrapperValue>(txt);
                    if (wrapper != null && wrapper.Value != null) _data = wrapper.Value;
                }
                catch
                {
                }

            if (_data == null)
                try
                {
                    var wrapperLower = JsonUtility.FromJson<Wrapper_value>(txt);
                    if (wrapperLower != null && wrapperLower.value != null) _data = wrapperLower.value;
                }
                catch
                {
                }

            if (_data == null)
                // 兜底创建一个默认实例
                _data = new GameData();
        }
        else
        {
            _data = new GameData();
        }
    }

    private void Save()
    {
        if (_data == null) _data = new GameData();
        var json = JsonUtility.ToJson(_data, true);
        File.WriteAllText(_path, json);
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("存档编辑器", "已保存到:\n" + _path, "确定");
    }

    // 用于解析可能的 BindableProperty 序列化格式 (含 Value)
    [Serializable]
    private class WrapperValue
    {
        public GameData Value;
    }

    // 有些实现可能序列化为小写字段名 (value)
    [Serializable]
    private class Wrapper_value
    {
        public GameData value;
    }
}
#endif