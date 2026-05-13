using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

// 添加必要的命名空间

public class LanguagePanel : MonoBehaviour, OptionPanelChildren
{
    private Button[] languageButtons;

    public void Init()
    {
        languageButtons = transform.Find("Group").GetComponentsInChildren<Button>();

        for (var i = 0; i < languageButtons.Length; i++)
        {
            var index = i;
            languageButtons[i].onClick.RemoveAllListeners(); // 清除旧监听
            languageButtons[i].onClick.AddListener(() => OnLanguageButtonClick(index));
        }
    }

    private void OnLanguageButtonClick(int index)
    {
        switch (index)
        {
            case 0:
                // 切换到中文 (zh-CN)
                SwitchToLanguage("zh-CN");
                break;
            case 1:
                // 切换到英文 (en-US) 
                SwitchToLanguage("en-US");
                break;
        }
    }

    // 方法一：通过语言标识符切换（推荐）
    private void SwitchToLanguage(string languageCode)
    {
        // 遍历所有可用的语言环境
        foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
            if (locale.Identifier.Code == languageCode)
            {
                LocalizationSettings.SelectedLocale = locale;
                Debug.Log($"已切换到语言: {locale.LocaleName}");
                return;
            }

        Debug.LogError($"未找到对应的语言设置: {languageCode}");
    }
}