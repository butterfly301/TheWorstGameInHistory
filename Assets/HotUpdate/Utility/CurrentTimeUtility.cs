// csharp

using System;

namespace HotUpdate.Utility
{
    public static class CurrentTimeUtility
    {
        /// <summary>
        ///     获取本地当前时间，格式为 yyyy/MM/dd HH:mm:ss（例如 2025/12/26 13:21:05）。
        /// </summary>
        public static string GetCurrentTimeString()
        {
            return DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }

        /// <summary>
        ///     新增：获取本地当前日期字符串，格式为 yyyy/MM/dd（例如 2025/12/26）。
        /// </summary>
        public static string GetCurrentDateString()
        {
            return DateTime.Now.ToString("yyyy/MM/dd");
        }

        /// <summary>
        ///     获取指定时间的格式化字符串，格式为 yyyy/MM/dd HH:mm:ss。
        /// </summary>
        public static string GetTimeString(DateTime time)
        {
            return time.ToString("yyyy/MM/dd HH:mm:ss");
        }

        /// <summary>
        ///     新增：获取指定时间的日期字符串，格式为 yyyy/MM/dd（例如 2025/12/26）。
        /// </summary>
        public static string GetDateString(DateTime time)
        {
            return time.ToString("yyyy/MM/dd");
        }
    }
}