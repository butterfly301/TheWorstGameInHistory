using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SPUM_AssetHelper : AssetPostprocessor
{
    public static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        foreach (var asset in importedAssets)
            //Debug.Log(asset);
            if (asset.Contains("Assets/SPUM/ReadMe")) // || ContainsPattern(asset))
                ShowHelpWindow(asset);
    }

    private static void ShowHelpWindow(string assetName)
    {
        HelpWindow.ShowWindow(assetName);
    }
    // static bool ContainsPattern(string fileName)
    // {
    //     string pattern = @"SPUM\d+\.unitypackage$";
    //     return Regex.IsMatch(fileName, pattern);
    // }
}

public class HelpWindow : EditorWindow
{
    public string scenePath = "Assets/SPUM/Scenes/SPUM_Scene.unity";
    private string assetName;
    private Rect contentRect;

    private readonly List<string> excludedDirectories = new()
    {
        "Assets/SPUM/Basic_Resources/Package/", // 삭제하지 않을 디렉토리
        "Assets/SPUM/Resources/",
        "Assets/SPUM/Backup/"
    };

    // 제외할 파일 및 디렉토리 목록
    private readonly List<string> excludedFiles = new()
    {
        "SPUM_PackageImporter.cs" // 삭제하지 않을 파일
    };

    private Texture2D helpImage;
    private readonly List<(string text, string url, int startIndex, int endIndex)> links = new();
    private string packagePath = "Assets/SPUM/Basic_Resources/Package/SPUM170.unitypackage";
    private string readmeContent;
    private string rootPath = "Assets/SPUM";
    private Vector2 scrollPosition;

    private void OnGUI()
    {
        // GUILayout.Label("[S]oonSoon [P]ixel [U]nit [M]aker" + assetName, EditorStyles.boldLabel);
        // GUILayout.Label("Here are some tips on how to use it:", EditorStyles.wordWrappedLabel);
        var style = new GUIStyle(EditorStyles.label);
        style.fontSize = 14;
        style.richText = true;
        // style.wordWrap = true;
        if (helpImage != null)
            GUILayout.Label(helpImage);
        else
            GUILayout.Label("Image not found.");
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        var parsedMarkdown = ParseMarkdown(readmeContent);

        var content = new GUIContent(parsedMarkdown);
        contentRect = GUILayoutUtility.GetRect(content, style, GUILayout.ExpandWidth(true));
        EditorGUI.LabelField(contentRect, content, style);


        var mousePos = Event.current.mousePosition;
        mousePos.y += scrollPosition.y;

        var currentEvent = Event.current;
        if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
            foreach (var link in links)
                if (IsPositionInLink(mousePos, link, contentRect))
                {
                    Application.OpenURL(link.url);
                    currentEvent.Use();
                    break;
                }


        //  var linkTexture = new Texture2D(1, 1);
        //  linkTexture.SetPixel(0, 0, Color.blue);
        //  linkTexture.Apply();

        foreach (var link in links)
            if (IsPositionInLink(mousePos, link, contentRect))
                EditorGUIUtility.AddCursorRect(contentRect, MouseCursor.Link);
            else
                EditorGUIUtility.AddCursorRect(contentRect, MouseCursor.Arrow);

        //GUI.DrawTexture(linkRect, linkTexture);
        GUILayout.EndScrollView();
        if (GUILayout.Button("OK")) Close();
        packagePath = EditorGUILayout.TextField("Package Path", packagePath);
        rootPath = EditorGUILayout.TextField("Root Path", rootPath);
        var styles = new GUIStyle(GUI.skin.button);
        styles.normal.textColor = Color.blue;
        styles.fontSize = 20;
        if (GUILayout.Button("Clean Install LocalPath Package ", styles, GUILayout.Height(60)))
            if (EditorUtility.DisplayDialog("Warrning", "All Spum paths except for the one below will be deleted. \n" +
                                                        excludedDirectories[0] + "\n"
                                                        + excludedDirectories[1] + "\n"
                                                        + excludedDirectories[2] + "\n"
                    , "OK", "Cancel"))
                if (EditorUtility.DisplayDialog("Warrning", "Are you sure you want to proceed?", "Yes", "No"))
                {
                    DeleteAllExceptExcluded();
                    ImportNewPackage();
                    DeleteEmptyCSFiles();
                    LoadSceneByPath();
                }
        // if (GUILayout.Button("Clean Install AssetStorePath package ", styles, GUILayout.Height(60)))
        // {
        //     if(EditorUtility.DisplayDialog("Warrning",  "All Spum paths except for the one below will be deleted. \n" + 
        //     excludedDirectories[0] + "\n"
        //     + excludedDirectories[1] + "\n"
        //     + excludedDirectories[2] + "\n"
        //     , "OK","Cancel"))
        //     {
        //         if(EditorUtility.DisplayDialog("Warrning",   "Are you sure you want to proceed?", "Yes","No"))
        //         {
        //             DeleteAllExceptExcluded();
        //             ImportStorePackage();
        //             DeleteEmptyCSFiles();
        //             LoadSceneByPath();
        //         }
        //     }

        // }
    }

    public static void ShowWindow(string assetName)
    {
        var window = GetWindow<HelpWindow>("SPUM PIXEL UNIT MAKER");

        window.assetName = assetName;
        window.helpImage =
            AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/SPUM//Basic_Resources/Package/Image/HelpImage.png");
        if (window.helpImage != null)
        {
            window.minSize = new Vector2(window.helpImage.width, 600); // 이미지 너비에 맞게 고정
            window.maxSize = new Vector2(window.helpImage.width, 600);
        }
        else
        {
            window.minSize = new Vector2(400, 300); // 기본 크기
            window.maxSize = window.minSize;
        }

        var path = assetName; // README.md 파일 경로 지정
        if (File.Exists(path))
            window.readmeContent = File.ReadAllText(path);
        // window.readmeContent = window.ParseMarkdown(window.readmeContent);
        else
            window.readmeContent = "README file not found.";
        window.Show();
    }

    private string ParseMarkdown(string markdown)
    {
        links.Clear();
        // 헤더 파싱
        markdown = Regex.Replace(markdown, @"^# (.*?)$", "<size=24><b>$1</b></size>", RegexOptions.Multiline);
        markdown = Regex.Replace(markdown, @"^## (.*?)$", "<size=20><b>$1</b></size>", RegexOptions.Multiline);
        markdown = Regex.Replace(markdown, @"^### (.*?)$", "<size=18><b>$1</b></size>", RegexOptions.Multiline);

        // 볼드체와 이탤릭체
        markdown = Regex.Replace(markdown, @"\*\*(.*?)\*\*", "<b>$1</b>");
        markdown = Regex.Replace(markdown, @"\*(.*?)\*", "<i>$1</i>");

        // 목록
        markdown = Regex.Replace(markdown, @"^- (.*?)$", "• $1", RegexOptions.Multiline);

        // 링크
        markdown = Regex.Replace(markdown, @"\[(.*?)\]\((.*?)\)", m =>
        {
            var linkText = m.Groups[1].Value;
            var url = m.Groups[2].Value;
            var startIndex = m.Index;
            var endIndex = startIndex + m.Length;
            links.Add((linkText, url, startIndex, endIndex));
            return $"<color=blue>{linkText}</color>";
        });

        // 코드 블록
        markdown = Regex.Replace(markdown, @"```(.*?)```", "<color=grey>$1</color>", RegexOptions.Singleline);

        return markdown;
    }

    public void LoadSceneByPath()
    {
        var sceneName = Path.GetFileNameWithoutExtension(scenePath);
        SceneManager.LoadScene(sceneName);
    }

    private bool IsPositionInLink(Vector2 position, (string text, string url, int startIndex, int endIndex) link,
        Rect totalRect)
    {
        var linkRect = GetLinkRect(link, totalRect);
        return linkRect.Contains(position);
    }

    private Rect GetLinkRect((string text, string url, int startIndex, int endIndex) link, Rect totalRect)
    {
        var style = new GUIStyle(EditorStyles.label);
        style.fontSize = 14;
        //style.richText = true;
        var startPosition = style.GetCursorPixelPosition(totalRect, new GUIContent(readmeContent), link.startIndex);
        var endPosition = style.GetCursorPixelPosition(totalRect, new GUIContent(readmeContent), link.endIndex);

        return new Rect(totalRect.x + startPosition.x, totalRect.y + startPosition.y - style.lineHeight,
            endPosition.x - startPosition.x, style.lineHeight);
    }

    private void DeleteAllExceptExcluded()
    {
        if (Directory.Exists(rootPath))
        {
            DeleteFilesAndFoldersRecursively(rootPath);
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogError($"Directory not found: {rootPath}");
        }
    }

    private void DeleteFilesAndFoldersRecursively(string path)
    {
        // 삭제하지 않을 디렉토리는 건너뜀
        if (IsExcluded(path))
        {
            Debug.Log($"Skipped directory: {path} (excluded from deletion)");
            return;
        }

        // 디렉토리 내의 모든 파일 삭제
        var files = Directory.GetFiles(path);
        foreach (var file in files)
            if (!IsExcluded(file))
            {
                FileUtil.DeleteFileOrDirectory(file);
                Debug.Log($"Deleted file: {file}");
            }
            else
            {
                Debug.Log($"Skipped file: {file} (excluded from deletion)");
            }

        // 디렉토리 내의 모든 하위 디렉토리 삭제
        var directories = Directory.GetDirectories(path);
        foreach (var dir in directories)
        {
            DeleteFilesAndFoldersRecursively(dir);

            // 만약 하위 디렉토리 삭제 후 해당 디렉토리가 비어 있으면 삭제
            if (Directory.Exists(dir) && Directory.GetFiles(dir).Length == 0 &&
                Directory.GetDirectories(dir).Length == 0)
            {
                Directory.Delete(dir);
                Debug.Log($"Deleted directory: {dir}");
            }
        }
    }

    private bool IsExcluded(string path)
    {
        // 경로를 절대 경로로 변환하여 비교
        var fullPath = Path.GetFullPath(path).Replace("\\", "/");

        // 제외할 파일 또는 디렉토리인 경우 true 반환
        var isExcludedFile = excludedFiles.Exists(file => Path.GetFullPath(file).Replace("\\", "/") == fullPath);
        var isExcludedDirectory =
            excludedDirectories.Exists(dir => fullPath.StartsWith(Path.GetFullPath(dir).Replace("\\", "/")));

        return isExcludedFile || isExcludedDirectory;
    }

    [MenuItem("SPUM/Clean Install")]
    public static void ShowPackage()
    {
        var directoryPath = @"Assets/SPUM/";

        var pattern = @"ReadMe\s*[-\s]*([\d\.]+)\.txt$";

        var files = Directory.GetFiles(directoryPath, "ReadMe*.txt");
        foreach (var item in files) Debug.Log(item);
        var matchedFiles = files
            .Select(file => new
            {
                FileName = file,
                Match = Regex.Match(Path.GetFileName(file), pattern)
            })
            .Where(x => x.Match.Success)
            .Select(x => new
            {
                x.FileName,
                Version = new Version(x.Match.Groups[1].Value.Trim())
            })
            .ToList();

        foreach (var file in matchedFiles) Debug.Log($"Matched File: {file.FileName}, Version: {file.Version}");

        var maxVersionFile = matchedFiles
            .OrderByDescending(x => x.Version)
            .FirstOrDefault();

        if (maxVersionFile != null)
        {
            Debug.Log($"Highest Version File: {maxVersionFile.FileName}");
            ShowWindow(maxVersionFile.FileName);
        }
        else
        {
            Debug.Log("No matching ReadMe files found.");
        }
    }

    private void ImportNewPackage()
    {
        var directoryPath = @"Assets/SPUM/Basic_Resources/Package/";
        var pattern = @"SPUM(\d+)\.unitypackage$";
        var files = Directory.GetFiles(directoryPath);

        var maxNumber = files
            .Select(file => Regex.Match(file, pattern))
            .Where(match => match.Success)
            .Select(match => int.Parse(match.Groups[1].Value))
            .DefaultIfEmpty(0)
            .Max();

        var highestNumberFile = files
            .FirstOrDefault(file => Regex.IsMatch(Path.GetFileName(file), $"SPUM{maxNumber}\\.unitypackage$"));
        if (string.IsNullOrEmpty(highestNumberFile))
        {
            Debug.Log("No path exists.");
            return;
        }

        AssetDatabase.ImportPackage(highestNumberFile, false);
        Debug.Log($"Imported: {highestNumberFile}");
    }

    private void ImportStorePackage()
    {
        var
            assetStorePath =
                ""; // = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), @"Unity\Asset Store-5.x\soonsoon\Textures Materials2D Characters\2D Pixel Unit Maker - SPUM.unitypackage");
        //string assetStorePath;


        if (Application.platform == RuntimePlatform.WindowsEditor)
            // Windows 
            assetStorePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"Unity\Asset Store-5.x\soonsoon\Textures Materials2D Characters\2D Pixel Unit Maker - SPUM.unitypackage");
        else if (Application.platform == RuntimePlatform.OSXEditor)
            // macOS 
            assetStorePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                @"Library\Asset Store-5.x\soonsoon\Textures Materials2D Characters\2D Pixel Unit Maker - SPUM.unitypackage");
        if (string.IsNullOrEmpty(assetStorePath))
        {
            Debug.Log("No path exists.");
            return;
        }

        AssetDatabase.ImportPackage(assetStorePath, false);
    }

    private void DeleteEmptyCSFiles()
    {
        if (Directory.Exists(rootPath))
        {
            DeleteEmptyCSFilesRecursively(rootPath);
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogError($"Directory not found: {rootPath}");
        }
    }

    private void DeleteEmptyCSFilesRecursively(string currentPath)
    {
        foreach (var directory in Directory.GetDirectories(currentPath)) DeleteEmptyCSFilesRecursively(directory);

        foreach (var file in Directory.GetFiles(currentPath, "*.cs"))
            if (new FileInfo(file).Length == 0)
            {
                File.Delete(file);
                Debug.Log($"Deleted empty file: {file}");
            }
    }
}