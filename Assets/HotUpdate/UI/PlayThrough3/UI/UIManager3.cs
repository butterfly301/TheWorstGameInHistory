using HotUpdate.UI;
using QFramework;
using UnityEngine;

public class UIManager3 : MonoSingleton<UIManager3>, UIManager
{
    [SerializeField] private Transform lobbyUITrans;
    [SerializeField] private Transform blurUITrans;
    [SerializeField] private Transform popUpUITrans;
    [SerializeField] private Transform confirmUITrans;

    private readonly System.Collections.Generic.Dictionary<object, Transform> uiLayerTrans = new();

    public LobbyUI Lobby { get; private set; }
    public BlurUI Blur { get; private set; }
    public PopUpUI PopUp { get; private set; }
    public ConfirmUI Confirm { get; private set; }

    public void Init()
    {
        uiLayerTrans.Add(UILayer.LobbyUI, lobbyUITrans);
        uiLayerTrans.Add(UILayer.BlurUI, blurUITrans);
        uiLayerTrans.Add(UILayer.PopUpUI, popUpUITrans);
        uiLayerTrans.Add(UILayer.ConfirmUI, confirmUITrans);
        Lobby = new LobbyUI(uiLayerTrans[UILayer.LobbyUI]);
        Blur = new BlurUI(uiLayerTrans[UILayer.BlurUI]);
        PopUp = new PopUpUI(uiLayerTrans[UILayer.PopUpUI]);
        Confirm = new ConfirmUI(uiLayerTrans[UILayer.ConfirmUI]);

        Lobby.Init();
        Blur.Init();
        PopUp.Init();
        Confirm.Init();
    }

    private enum UILayer
    {
        LobbyUI,
        BlurUI,
        PopUpUI,
        ConfirmUI,
    }
}
