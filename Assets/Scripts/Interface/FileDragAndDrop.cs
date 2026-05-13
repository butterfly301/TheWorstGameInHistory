using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using AOT;
using UnityEngine;
using Debug = UnityEngine.Debug;

// 必须引用这个用于 IL2CPP

namespace HotUpdate.Utility
{
    public class FileDragAndDrop : MonoBehaviour
    {
        // 外部订阅此事件即可获取文件列表
        public event Action<List<string>> OnFilesDropped;

        // =========================================================
        // 方案：打包模式 (Windows Win32 API + IL2CPP修复)
        // =========================================================
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        // 委托定义
        private delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        // 【IL2CPP 修正】必须是 static 变量，因为静态回调无法访问实例成员
        private static IntPtr _oldWndProcPtr;
        private static IntPtr _newWndProcPtr;
        private static IntPtr _unityWindowHandle;
        private static WndProcDelegate _newWndProc;
        
        // 【静态桥接】用于从静态 WndProc 回调通知到当前的实例对象
        private static Action<List<string>> _onDropAction;

        [DllImport("shell32.dll")]
        private static extern void DragAcceptFiles(IntPtr hwnd, bool fAccept);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern uint DragQueryFile(IntPtr hDrop, uint iFile, StringBuilder lpszFile, uint cch);

        [DllImport("shell32.dll")]
        private static extern void DragFinish(IntPtr hDrop);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern IntPtr SetWindowLong32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
        
        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        private static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            return IntPtr.Size == 8 ? SetWindowLongPtr64(hWnd, nIndex, dwNewLong) : SetWindowLong32(hWnd, nIndex, dwNewLong);
        }

        private const int GWLP_WNDPROC = -4;
        private const uint WM_DROPFILES = 0x0233;

        private void Start()
        {
            // 将当前实例的处理方法绑定到静态委托
            _onDropAction = HandleDroppedFilesInstance;

            // 获取窗口句柄 (优先用 GetActiveWindow)
            _unityWindowHandle = GetActiveWindow();
            if (_unityWindowHandle == IntPtr.Zero)
                 _unityWindowHandle = Process.GetCurrentProcess().MainWindowHandle;

            if (_unityWindowHandle != IntPtr.Zero)
            {
                DragAcceptFiles(_unityWindowHandle, true);
                
                // 实例化委托
                _newWndProc = new WndProcDelegate(WndProc);
                _newWndProcPtr = Marshal.GetFunctionPointerForDelegate(_newWndProc);
                
                // 替换窗口过程 (Hook)
                _oldWndProcPtr = SetWindowLongPtr(_unityWindowHandle, GWLP_WNDPROC, _newWndProcPtr);
                
                UnityEngine.Debug.Log("FileDragAndDrop Hook Success (IL2CPP Ready).");
            }
        }

        private void OnDisable()
        {
            if (_unityWindowHandle != IntPtr.Zero && _oldWndProcPtr != IntPtr.Zero)
            {
                // 还原窗口过程
                SetWindowLongPtr(_unityWindowHandle, GWLP_WNDPROC, _oldWndProcPtr);
                DragAcceptFiles(_unityWindowHandle, false);
                _oldWndProcPtr = IntPtr.Zero;
                _unityWindowHandle = IntPtr.Zero;
            }
            // 清理引用，防止内存泄漏
            _onDropAction = null;
        }

        // 实例方法：中转
        private void HandleDroppedFilesInstance(List<string> files)
        {
            OnFilesDropped?.Invoke(files);
        }

        // 【IL2CPP 修正】必须加此特性，且必须是 static 方法
        [MonoPInvokeCallback(typeof(WndProcDelegate))]
        private static IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == WM_DROPFILES)
            {
                try 
                {
                    IntPtr hDrop = wParam;
                    uint count = DragQueryFile(hDrop, 0xFFFFFFFF, null, 0);
                    List<string> files = new List<string>();
                    var sb = new StringBuilder(1024);
                    for (uint i = 0; i < count; i++)
                    {
                        DragQueryFile(hDrop, i, sb, 1024);
                        files.Add(sb.ToString());
                    }
                    DragFinish(hDrop);

                    // 通过静态变量回调到实例
                    if (_onDropAction != null && files.Count > 0)
                    {
                        _onDropAction(files);
                    }
                }
                catch(Exception e)
                {
                    UnityEngine.Debug.LogError("Win32 Drop Error: " + e.Message);
                }
                
                return IntPtr.Zero; // 返回0表示我们处理了该消息
            }

            return CallWindowProc(_oldWndProcPtr, hWnd, msg, wParam, lParam);
        }
#endif
#if UNITY_EDITOR
        // =========================================================
        // 调试用：手动触发 (Inspector 右键菜单)
        // =========================================================
        [ContextMenu("模拟拖入破冰者软件安装包")]
        public void SimulateDropIceBreaker()
        {
            Debug.Log("Simulating Drop from Context Menu...");
            // 构造一个假路径，触发逻辑
            OnFilesDropped?.Invoke(new List<string> { "C:\\FakePath\\矩阵破冰者软件安装包.png" });
        }

        [ContextMenu("模拟拖入最后的勇者2安装包")]
        public void SimulateDropTLH2()
        {
            Debug.Log("Simulating Drop from Context Menu...");
            // 构造一个假路径，触发逻辑
            OnFilesDropped?.Invoke(new List<string> { "C:\\FakePath\\最后的勇者2游戏包体.png" });
        }
#endif
    }
}