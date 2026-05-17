using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using QFramework;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

namespace HotUpdate.Download.System
{
    public class DownloadProgress
    {
        public int CompletedCount;
        public string CurrentUrl;
        public string Error; // optional
        public string SavedPath; // optional
        public int TotalCount;
    }

public class DownloadResult
    {
        public string Error; // when failed
        public string SavedPath; // when success
        public bool Success;
        public string Url;
    }

public class DownloadSummary
    {
        public int FailCount;
        public List<DownloadResult> Results = new();
        public string SaveDirectory;
        public int SuccessCount;
    }

public class DownloadSystem : AbstractSystem
    {
        protected override void OnInit()
        {
        }

public string GetSaveDirectory(string subFolderName)
        {
            if (string.IsNullOrWhiteSpace(subFolderName)) subFolderName = "Downloads";
            return Path.Combine(Application.persistentDataPath, subFolderName);
        }

public async Task<DownloadSummary> DownloadAndSaveAsync(IEnumerable<string> urls, string subFolderName,
            bool overwriteExisting = true, IProgress<DownloadProgress> progress = null,
            CancellationToken cancellationToken = default)
        {
            var urlList = urls?.Where(u => !string.IsNullOrWhiteSpace(u)).ToList() ?? new List<string>();
            var summary = new DownloadSummary { SaveDirectory = GetSaveDirectory(subFolderName) };

Directory.CreateDirectory(summary.SaveDirectory);

var total = urlList.Count;
            var completed = 0;

foreach (var url in urlList)
            {
                if (cancellationToken.IsCancellationRequested) break;

var result = new DownloadResult { Url = url };
                try
                {
                    using (var req = UnityWebRequest.Get(url))
                    {
                        var op = req.SendWebRequest();
                        while (!op.isDone)
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                req.Abort();
                                break;
                            }

await Task.Yield();
                        }

#if UNITY_2020_2_OR_NEWER
                        var success = req.result == UnityWebRequest.Result.Success;
#else
                        bool success = !(req.isNetworkError || req.isHttpError);
#endif
                        if (!success) throw new Exception(req.error);

var data = req.downloadHandler.data;
                        if (data == null || data.Length == 0) throw new Exception("Empty data");

// 仅从 URL 末段取文件名，不做 Content-Type 扩展名推断
                        var fileName = BuildFileNameFromUrl(url);
                        fileName = SanitizeFileName(fileName);

var savePath = Path.Combine(summary.SaveDirectory, fileName);
                        savePath = EnsureWritablePath(savePath, overwriteExisting);

File.WriteAllBytes(savePath, data);
                        result.Success = true;
                        result.SavedPath = savePath;

completed++;
                        progress?.Report(new DownloadProgress
                        {
                            CompletedCount = completed,
                            TotalCount = total,
                            CurrentUrl = url,
                            SavedPath = savePath
                        });
                    }
                }
                catch (Exception e)
                {
                    result.Success = false;
                    result.Error = e.Message;
                    completed++;
                    progress?.Report(new DownloadProgress
                    {
                        CompletedCount = completed,
                        TotalCount = total,
                        CurrentUrl = url,
                        Error = e.Message
                    });
                }

summary.Results.Add(result);
            }

summary.SuccessCount = summary.Results.Count(r => r.Success);
            summary.FailCount = summary.Results.Count(r => !r.Success);
            return summary;
        }

public void OpenFolder(string subFolderName)
        {
            var dir = GetSaveDirectory(subFolderName);
            try
            {
                Directory.CreateDirectory(dir);
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                Process.Start(new ProcessStartInfo
                {
                    FileName = dir,
                    UseShellExecute = true
                });
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
                Process.Start("open", dir);
#else
                UnityEngine.Debug.Log($"存档目录: {dir}");
#endif
            }
            catch (Exception e)
            {
                Debug.LogError($"打开目录失败: '{dir}'\n{e}");
            }
        }

private static string EnsureWritablePath(string path, bool overwrite)
        {
            if (overwrite || !File.Exists(path)) return path;

var dir = Path.GetDirectoryName(path) ?? string.Empty;
            var name = Path.GetFileNameWithoutExtension(path);
            var ext = Path.GetExtension(path);

var idx = 1;
            string candidate;
            do
            {
                candidate = Path.Combine(dir, $"{name} ({idx}){ext}");
                idx++;
            } while (File.Exists(candidate));

return candidate;
        }

// 仅从 URL 末段提取文件名；如果为空则返回 "file"
        private static string BuildFileNameFromUrl(string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url)) return "file";

var decoded = Uri.UnescapeDataString(url);
                var q = decoded.IndexOf('?');
                if (q >= 0) decoded = decoded.Substring(0, q);

var hash = decoded.IndexOf('#');
                if (hash >= 0) decoded = decoded.Substring(0, hash);

var slash = decoded.LastIndexOf('/');
                var last = slash >= 0 && slash + 1 < decoded.Length ? decoded.Substring(slash + 1) : decoded;
                if (string.IsNullOrWhiteSpace(last)) return "file";

return last;
            }
            catch
            {
                return "file";
            }
        }

private static string SanitizeFileName(string fileName)
        {
            var invalid = Path.GetInvalidFileNameChars();
            foreach (var ch in invalid) fileName = fileName.Replace(ch, '_');
            return fileName;
        }
    }
}