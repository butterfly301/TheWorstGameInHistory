using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

// For the Stopwatch class

namespace TwoBitMachines
{
    public static class DebugTimer
    {
        public static float timeStamp;
        private static Stopwatch stopwatch = new();

        public static void Start()
        {
            timeStamp = Time.realtimeSinceStartup;
        }

        public static float Stop(string identifier, bool displayMessage = true)
        {
            var time = Time.realtimeSinceStartup - timeStamp;
            timeStamp = Time.realtimeSinceStartup;
            if (displayMessage)
                Debug.Log(identifier + "" + time);
            return time;
        }

        // public static void Start ()
        // {
        //         stopwatch.Reset();
        //         stopwatch.Start();

        // }

        // public static float Stop (string identifier, bool displayMessage = true)
        // {
        //         stopwatch.Stop();
        //         if (displayMessage)
        //                 UnityEngine.Debug.Log(identifier + stopwatch.ElapsedMilliseconds + " ms");

        //         return 0;
        // }
    }
}