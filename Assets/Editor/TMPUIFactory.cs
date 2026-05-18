using HotUpdate.Utility;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class TMPUIFactory
{
    [MenuItem("GameObject/UI/Project TMP Text", false, 2001)]
    private static void CreateProjectTMPText(MenuCommand menuCommand)
    {
        var parent = GetOrCreateUIParent(menuCommand);
        var textObject = new GameObject("Text (TMP)", typeof(RectTransform));
        GameObjectUtility.SetParentAndAlign(textObject, parent);

        var rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(160f, 40f);

        var text = Undo.AddComponent<TextMeshProUGUI>(textObject);
        text.text = "New Text";
        text.fontSize = 36f;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.font = LoadFontAsset();

        FinalizeCreation(textObject);
    }

    [MenuItem("GameObject/UI/Project Button", false, 2002)]
    private static void CreateProjectButton(MenuCommand menuCommand)
    {
        var parent = GetOrCreateUIParent(menuCommand);
        var buttonObject = new GameObject("Button", typeof(RectTransform), typeof(Image), typeof(Button));
        GameObjectUtility.SetParentAndAlign(buttonObject, parent);

        var rectTransform = buttonObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(160f, 40f);

        var image = buttonObject.GetComponent<Image>();
        image.color = new Color32(255, 255, 255, 255);

        var button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;

        var textObject = new GameObject("Text (TMP)", typeof(RectTransform));
        GameObjectUtility.SetParentAndAlign(textObject, buttonObject);

        var textRectTransform = textObject.GetComponent<RectTransform>();
        textRectTransform.anchorMin = Vector2.zero;
        textRectTransform.anchorMax = Vector2.one;
        textRectTransform.offsetMin = Vector2.zero;
        textRectTransform.offsetMax = Vector2.zero;

        var text = Undo.AddComponent<TextMeshProUGUI>(textObject);
        text.text = "Button";
        text.fontSize = 36f;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.black;
        text.font = LoadFontAsset();

        FinalizeCreation(buttonObject);
    }

    private static TMP_FontAsset LoadFontAsset()
    {
        return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AddressableKeys.SourceHanSans_Medium_SDF_Asset);
    }

    private static GameObject GetOrCreateUIParent(MenuCommand menuCommand)
    {
        var contextObject = menuCommand.context as GameObject;
        if (contextObject != null && contextObject.GetComponentInParent<Canvas>() != null)
        {
            return contextObject;
        }

        var canvas = Object.FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            return canvas.gameObject;
        }

        var canvasObject = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvasComponent = canvasObject.GetComponent<Canvas>();
        canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObject.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920f, 1080f);

        Undo.RegisterCreatedObjectUndo(canvasObject, "Create Canvas");

        if (Object.FindObjectOfType<EventSystem>() == null)
        {
            var eventSystemObject = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            Undo.RegisterCreatedObjectUndo(eventSystemObject, "Create EventSystem");
        }

        return canvasObject;
    }

    private static void FinalizeCreation(GameObject gameObject)
    {
        Undo.RegisterCreatedObjectUndo(gameObject, "Create UI");
        Selection.activeGameObject = gameObject;
        EditorSceneManager.MarkSceneDirty(gameObject.scene);
    }
}
