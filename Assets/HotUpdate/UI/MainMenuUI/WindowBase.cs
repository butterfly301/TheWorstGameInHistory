using HotUpdate.Core;
using HotUpdate.Data.Model;
using HotUpdate.Interface;
using HotUpdate.UI;
using QFramework;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class WindowBase : MonoBehaviour, IController,IAutoBind
{
    protected GameData gameData;
    private Vector2 initialRectTransformPosition;
    protected MainMenu mainMenu;
    [SerializeField]protected Button quit;
    private RectTransform rectTransform;

private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        initialRectTransformPosition = rectTransform.anchoredPosition;
    }

protected virtual void OnEnable()
    {
        //每次启用时使其复位
        rectTransform.anchoredPosition = initialRectTransformPosition;
    }

public IArchitecture GetArchitecture()
    {
        return TheWorstGameInHistory.Interface;
    }

public virtual void Init(MainMenu mainMenuVar)
    {
        mainMenu = mainMenuVar;
        gameData = mainMenu.GameData;
        quit.onClick.AddListener(CloseWindow);
    }

public void CloseWindow()
    {
        gameObject.SetActive(false);
    }
}