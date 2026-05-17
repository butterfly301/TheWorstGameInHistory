using HotUpdate.Core;
using QFramework;
using UnityEngine;

namespace HotUpdate.Console
{
    public class ConsoleView : MonoBehaviour, IController
    {
        [Header("Settings")] [SerializeField] private float mWindowHeightRatio = 0.5f;

[SerializeField] private int mFontSize = 40;
        [SerializeField] private int mToolbarHeight = 50;
        [SerializeField] private int mButtonWidth = 150;
        [SerializeField] private int mButtonHeight = 40;
        [SerializeField] private Font mFont;

// GUI Styles
        private GUIStyle mBackgroundStyle;
        private GUIStyle mButtonStyle;

private ConsoleSystem mConsoleSystem;
        private GUIStyle mErrorStyle;
        private GUIStyle mLogStyle;
        private Vector2 mScrollPosition;
        private string mSelectedStackTrace = "";
        private bool mShowStackTrace;
        private GUIStyle mWarningStyle;

private void Start()
        {
            mConsoleSystem = this.GetSystem<ConsoleSystem>();
        }

private void Update()
        {
            // Toggle console with BackQuote (`) key
            if (Input.GetKeyDown(KeyCode.BackQuote)) mConsoleSystem.IsVisible = !mConsoleSystem.IsVisible;
        }

private void OnGUI()
        {
            if (!mConsoleSystem.IsVisible) return;

InitStyles();

// Draw Background
            float width = Screen.width;
            var height = Screen.height * mWindowHeightRatio;
            GUI.Box(new Rect(0, 0, width, height), "", mBackgroundStyle);

// Toolbar
            GUILayout.BeginArea(new Rect(0, 0, width, mToolbarHeight));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear", mButtonStyle))
            {
                mConsoleSystem.Clear();
                mSelectedStackTrace = "";
            }

if (GUILayout.Button("Close", mButtonStyle)) mConsoleSystem.IsVisible = false;
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

// Logs Area
            GUILayout.BeginArea(new Rect(0, mToolbarHeight, width, height - mToolbarHeight));
            mScrollPosition = GUILayout.BeginScrollView(mScrollPosition);

foreach (var log in mConsoleSystem.GetLogs())
            {
                var style = mLogStyle;
                if (log.Type == LogType.Warning) style = mWarningStyle;
                else if (log.Type == LogType.Error || log.Type == LogType.Exception) style = mErrorStyle;

if (GUILayout.Button(log.Message, style))
                {
                    mSelectedStackTrace = log.StackTrace;
                    mShowStackTrace = !string.IsNullOrEmpty(mSelectedStackTrace);
                }

if (mShowStackTrace && mSelectedStackTrace == log.StackTrace)
                    GUILayout.Label(log.StackTrace, mLogStyle);
            }

GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

public IArchitecture GetArchitecture()
        {
            return TheWorstGameInHistory.Interface;
        }

private void InitStyles()
        {
            if (mBackgroundStyle == null)
            {
                mBackgroundStyle = new GUIStyle(GUI.skin.box);
                mBackgroundStyle.normal.background = MakeTex(2, 2, new Color(0f, 0f, 0f, 0.8f));
            }

if (mLogStyle == null)
            {
                mLogStyle = new GUIStyle(GUI.skin.label);
                mLogStyle.normal.textColor = Color.white;
                mLogStyle.wordWrap = true;
            }

mLogStyle.fontSize = mFontSize;
            mLogStyle.font = mFont;

if (mWarningStyle == null)
            {
                mWarningStyle = new GUIStyle(GUI.skin.label);
                mWarningStyle.normal.textColor = Color.yellow;
                mWarningStyle.wordWrap = true;
            }

mWarningStyle.fontSize = mFontSize;
            mWarningStyle.font = mFont;

if (mErrorStyle == null)
            {
                mErrorStyle = new GUIStyle(GUI.skin.label);
                mErrorStyle.normal.textColor = Color.red;
                mErrorStyle.wordWrap = true;
            }

mErrorStyle.fontSize = mFontSize;
            mErrorStyle.font = mFont;

if (mButtonStyle == null) mButtonStyle = new GUIStyle(GUI.skin.button);
            mButtonStyle.fixedWidth = mButtonWidth;
            mButtonStyle.fixedHeight = mButtonHeight;
            mButtonStyle.fontSize = mFontSize;
            mButtonStyle.font = mFont;
        }

private Texture2D MakeTex(int width, int height, Color col)
        {
            var pix = new Color[width * height];
            for (var i = 0; i < pix.Length; ++i) pix[i] = col;
            var result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}