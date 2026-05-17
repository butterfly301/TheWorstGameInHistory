// updated in 2025/8/5

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public static class MyTimer
{
    private static readonly List<TimingEvent> L;

static MyTimer()
    {
        L = new List<TimingEvent>();
    }

public static void AddEvent(Action action, float time, bool isLoop = false)
    {
        var s = new TimingEvent
        {
            StartTime = Time.time,
            Action = action,
            Time = time,
            IsLoop = isLoop
        };
        L.Add(s);
    }

public static void Update()
    {
        var n = L.Count;
        for (var i = 0; i < n; ++i)
            if (Time.time > L[i].StartTime + L[i].Time)
            {
                try
                {
                    L[i].Action();
                }
                catch (Exception)
                {
                    L.RemoveAt(i);
                    --i;
                    --n;
                    continue;
                }

if (!L[i].IsLoop)
                {
                    L.RemoveAt(i);
                    --i;
                    --n;
                }
                else
                {
                    TimingEvent t = L[i];
                    t.StartTime = Time.time;
                    L[i] = t;
                }
            }
    }

private struct TimingEvent
    {
        public float StartTime;
        public Action Action;
        public float Time;
        public bool IsLoop;
    }
}

public static class MyConverter
{
    public static ArraySegment<byte> String2Byte(string str)
    {
        byte[] byteArray = Encoding.Default.GetBytes(str);
        return new ArraySegment<byte>(byteArray);
    }

public static string Byte2String(ArraySegment<byte> bytes)
    {
        var list = (IList<byte>)bytes;
        var bt = bytes.ToArray();
        return Encoding.UTF8.GetString(bt);
    }
}

public static class MyTool
{
    public static float RandomRange(float baseNum, float offset)
    {
        return baseNum + (1 - Random.value * 2) * offset;
    }

public static string PrintArray<T>(T[] array)
    {
        StringBuilder s = new StringBuilder();
        for (int i = 0; i < array.Length; ++i)
        {
            s.Append(array[i]);
            s.Append("\t");
        }

return s.ToString();
    }

public static string PrintTwoDimensionalArray<T>(T[,] array, int w, int h)
    {
        StringBuilder s = new StringBuilder();
        for (int i = 0; i < w; ++i)
        {
            for (int j = 0; j < h; ++j)
            {
                s.Append(array[i, j]);
                s.Append("\t");
            }

s.Append("\n");
        }

return s.ToString();
    }

public static string PrintTwoDimensionalArray<T>(T[][] array, int w, int h)
    {
        StringBuilder s = new StringBuilder();
        for (int i = 0; i < w; ++i)
        {
            for (int j = 0; j < h; ++j)
            {
                s.Append(array[i][j]);
                s.Append("\t");
            }

s.Append("\n");
        }

return s.ToString();
    }

public static Vector2 GetRectNormal(Vector2 targetPos, Vector2 targetDir, Rect rect)
    {
        float ydx = targetDir.y / targetDir.x;
        float xdy = targetDir.x / targetDir.y;

if (targetPos.y > rect.yMax && targetDir.y < 0)
        {
            float toTop = rect.yMax - targetPos.y;
            float x = targetPos.x + xdy * toTop;
            if (rect.xMin < x && rect.xMax > x)
                return Vector2.up;
        }

if (targetPos.y < rect.yMin && targetDir.y > 0)
        {
            float toBottom = rect.yMin - targetPos.y;
            float x = targetPos.x + xdy * toBottom;
            if (rect.xMin < x && rect.xMax > x)
                return Vector2.down;
        }

if (targetPos.x > rect.xMax && targetDir.x < 0)
        {
            float toRight = rect.xMax - targetPos.x;
            float y = targetPos.y + ydx * toRight;
            if (rect.yMin < y && rect.yMax > y)
                return Vector2.right;
        }

if (targetPos.x < rect.xMin && targetDir.x > 0)
        {
            float toLeft = rect.xMin - targetPos.x;
            float y = targetPos.y + ydx * toLeft;
            if (rect.yMin < y && rect.yMax > y)
                return Vector2.left;
        }

return Vector2.zero;
    }
}

public class MyObjectPool<T> where T : MonoBehaviour
{
    private Stack<GameObject> _stack;
    private Dictionary<GameObject, T> _dictionary;
    private readonly GameObject _prefab;
    private readonly GameObject[] _prefabs;
    private int _maxNum;

public Action<GameObject, T> OnGet;
    public Action<GameObject, T> OnRelease;

private void Initialize(int maxNum)
    {
        _stack = new Stack<GameObject>(maxNum);
        _dictionary = new Dictionary<GameObject, T>(maxNum);
        _maxNum = maxNum;
    }

public MyObjectPool(GameObject prefab, int maxNum)
    {
        if (!prefab.TryGetComponent(out T _))
            throw new ArgumentException($"prefab has no {typeof(T)}");
        Initialize(maxNum);
        _prefab = prefab;
    }

public MyObjectPool(GameObject[] prefabs, int maxNum)
    {
        for (int i = 0; i < prefabs.Length; ++i)
        {
            if (!prefabs[i].TryGetComponent(out T _))
                throw new ArgumentException($"prefab has no {typeof(T)}");
        }

Initialize(maxNum);
        _prefabs = prefabs;
    }

// ReSharper disable Unity.PerformanceAnalysis
    public GameObject Get()
    {
        GameObject g;
        if (_stack.Count > 0)
        {
            g = _stack.Pop();
        }
        else
        {
            g = Object.Instantiate(!_prefab ? _prefabs[(int)(Random.value * _prefabs.Length - 0.01f)] : _prefab);
            _dictionary.Add(g, g.GetComponent<T>());
        }

g.SetActive(true);
        OnGet?.Invoke(g.gameObject, _dictionary[g]);
        return g;
    }

public bool TryGetScript(GameObject g, out T script)
    {
        bool b = _dictionary.ContainsKey(g);
        script = b ? _dictionary[g] : null;

return b;
    }

public void Release(GameObject g)
    {
        OnRelease?.Invoke(g, _dictionary[g]);
        if (_stack.Count < _maxNum)
        {
            g.SetActive(false);
            _stack.Push(g);
        }
        else
        {
            _dictionary.Remove(g);
            Object.Destroy(g);
        }
    }
}

#region MyEvent

public class MyEventCore
{
    private bool _joinFlag = false;

public readonly List<Delegate> _actions = new();
    public int _index;
    private object[] _lastArgs;

public Action EndAction;

protected void Add(Delegate d) => _actions.Add(d);
    protected void Remove(Delegate d) => _actions.Remove(d);

protected void Invoke(params object[] args)
    {
        _index = 0;
        _joinFlag = false;
        _lastArgs = args;

Resume();
    }

public void Resume()
    {
        _joinFlag = false;
        for (; _index < _actions.Count; ++_index)
        {
            if (_joinFlag) // to jump out
            {
                _joinFlag = false;
                return;
            }

_actions[_index].DynamicInvoke(_lastArgs);
        }

if (_joinFlag) // to jump out
        {
            _joinFlag = false;
            return;
        }

EndAction?.Invoke();
    }

public void Join() => _joinFlag = true;
}

public class MyEvent : MyEventCore
{
    public static MyEvent operator +(MyEvent e, Action a)
    {
        e.Add(a);
        return e;
    }

public static MyEvent operator -(MyEvent e, Action a)
    {
        e.Remove(a);
        return e;
    }

public void Invoke() => base.Invoke();
}

public class MyEvent<T1> : MyEventCore
{
    public static MyEvent<T1> operator +(MyEvent<T1> e, Action<T1> a)
    {
        e.Add(a);
        return e;
    }

public static MyEvent<T1> operator -(MyEvent<T1> e, Action<T1> a)
    {
        e.Remove(a);
        return e;
    }

public void Invoke(T1 arg1) => base.Invoke(arg1);
}

public class MyEvent<T1, T2> : MyEventCore
{
    public static MyEvent<T1, T2> operator +(MyEvent<T1, T2> e, Action<T1, T2> a)
    {
        e.Add(a);
        return e;
    }

public static MyEvent<T1, T2> operator -(MyEvent<T1, T2> e, Action<T1, T2> a)
    {
        e.Remove(a);
        return e;
    }

public void Invoke(T1 arg1, T2 arg2) => base.Invoke(arg1, arg2);
}

public class MyEvent<T1, T2, T3> : MyEventCore
{
    public static MyEvent<T1, T2, T3> operator +(MyEvent<T1, T2, T3> e, Action<T1, T2, T3> a)
    {
        e.Add(a);
        return e;
    }

public static MyEvent<T1, T2, T3> operator -(MyEvent<T1, T2, T3> e, Action<T1, T2, T3> a)
    {
        e.Remove(a);
        return e;
    }

public void Invoke(T1 arg1, T2 arg2, T3 arg3) => base.Invoke(arg1, arg2, arg3);
}

public class MyEvent<T1, T2, T3, T4> : MyEventCore
{
    public static MyEvent<T1, T2, T3, T4> operator +(MyEvent<T1, T2, T3, T4> e, Action<T1, T2, T3, T4> a)
    {
        e.Add(a);
        return e;
    }

public static MyEvent<T1, T2, T3, T4> operator -(MyEvent<T1, T2, T3, T4> e, Action<T1, T2, T3, T4> a)
    {
        e.Remove(a);
        return e;
    }

public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4) => base.Invoke(arg1, arg2, arg3, arg4);
}

#endregion