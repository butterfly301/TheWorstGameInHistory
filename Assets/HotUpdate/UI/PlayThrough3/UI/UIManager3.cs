using UnityEngine;
using HotUpdate.UI;

public class UIManager3 : UIManager
{
    [SerializeField] private Transform lobbyUITrans;

    public LobbyUI Lobby { get; private set; }

    public override void Init()
    {
        uiLayerTrans.Add(UILayer.LobbyForm, lobbyUITrans);

        Lobby = new LobbyUI(uiLayerTrans[UILayer.LobbyForm]);
        Lobby.Init();
    }

    private enum UILayer
    {
        LobbyForm
    }
}
