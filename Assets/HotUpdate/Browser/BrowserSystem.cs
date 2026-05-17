using System.IO;
using QFramework;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace HotUpdate.Browser
{
    public class BrowserSystem : AbstractSystem, IBrowserSystem
    {
        /// <summary>
        ///     统一处理URL请求。当前版本只处理外部文件打开。
        /// </summary>
        public void HandleURL(string url)
        {
            if (string.IsNullOrEmpty(url)) return;

// 现在的逻辑是，所有传入的URL都被视为一个需要打开的本地HTML文件名。
            OpenExternalFile(url);
        }

protected override void OnInit()
        {
        }

private void OpenExternalFile(string baseFileName)
        {
            var langCode = LocalizationSettings.SelectedLocale.Identifier.Code;
            var filePath = Path.Combine(Application.streamingAssetsPath, "htmls", langCode, baseFileName);

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS
                filePath = "file://" + filePath;
#endif

Application.OpenURL(filePath);
            Debug.Log($"[BrowserSystem] Opening external file: {filePath}");
        }
    }
}