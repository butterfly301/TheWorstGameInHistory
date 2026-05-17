using UnityEngine;
using HotUpdate.UI;


public class UIManager3 : UIManager
{
    [SerializeField] private Transform lobbyUITrans;
    [SerializeField] private Transform blurUITrans;
    [SerializeField] private Transform popUpUITrans;

    public LobbyUI Lobby { get; private set; }
    public BlurUI Blur { get; private set; }
    public PopUpUI PopUp { get; private set; }

    public override void Init()
    {
        uiLayerTrans.Add(UILayer.LobbyUI, lobbyUITrans);
        uiLayerTrans.Add(UILayer.BlurUI, blurUITrans);
        uiLayerTrans.Add(UILayer.PopUpUI, popUpUITrans);
        Lobby = new LobbyUI(uiLayerTrans[UILayer.LobbyUI]);
        Blur = new BlurUI(uiLayerTrans[UILayer.BlurUI]);
        PopUp = new PopUpUI(uiLayerTrans[UILayer.PopUpUI]);

        Lobby.Init();
        Blur.Init();
        PopUp.Init();
    }

    private enum UILayer
    {
        LobbyUI,
        BlurUI,
        PopUpUI,
    }
}
