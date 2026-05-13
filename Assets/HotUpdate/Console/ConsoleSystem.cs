using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace HotUpdate.Console
{
    public class ConsoleSystem : AbstractSystem
    {
        private readonly List<LogData> mLogs = new();
        public bool IsVisible { get; set; } = false;

        protected override void OnInit()
        {
            Application.logMessageReceived += HandleLog;

            // Create a GameObject to handle Input and Rendering (OnGUI)
            var consoleGo = new GameObject("ConsoleView");
            Object.DontDestroyOnLoad(consoleGo);
            consoleGo.AddComponent<ConsoleView>();
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            mLogs.Add(new LogData
            {
                Message = logString,
                StackTrace = stackTrace,
                Type = type
            });

            if (mLogs.Count > 200) mLogs.RemoveAt(0);
        }

        public IEnumerable<LogData> GetLogs()
        {
            return mLogs;
        }

        public void Clear()
        {
            mLogs.Clear();
        }

        public struct LogData
        {
            public string Message;
            public string StackTrace;
            public LogType Type;
        }
    }
}