using HotUpdate.Interface;
using HotUpdate.UI;
using QFramework;
using UnityEngine;

public class UIManager3 : MonoSingleton<UIManager3>,IAutoBind
{
    public static bool HasInstance => mInstance != null;

    public static UIManager3 GetOrCreate(GameObject prefab)
    {
        if (mInstance != null)
        {
            return mInstance;
        }

        var uiManagerObj = Object.Instantiate(prefab);
        return uiManagerObj.GetComponent<UIManager3>();
    }

    [SerializeField] private Transform lobbyUITrans;
    [SerializeField] private Transform blurUITrans;
    [SerializeField] private Transform popUpUITrans;
    [SerializeField] private Transform confirmUITrans;
    [SerializeField] private Transform tipUITrans;

    private readonly System.Collections.Generic.Dictionary<object, Transform> uiLayerTrans = new();

    public LobbyUI Lobby { get; private set; }
    public BlurUI Blur { get; private set; }
    public PopUpUI PopUp { get; private set; }
    public ConfirmUI Confirm { get; private set; }
    public TipUI Tip { get; private set; }

    private void Awake()
    {
        if (mInstance != null && mInstance != this)
        {
            Destroy(gameObject);
            return;
        }

        mInstance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Init()
    {
        uiLayerTrans.Clear();
        uiLayerTrans.Add(UILayer.LobbyUI, lobbyUITrans);
        uiLayerTrans.Add(UILayer.BlurUI, blurUITrans);
        uiLayerTrans.Add(UILayer.PopUpUI, popUpUITrans);
        uiLayerTrans.Add(UILayer.ConfirmUI, confirmUITrans);
        uiLayerTrans.Add(UILayer.TipUI, tipUITrans);
        Lobby ??= new LobbyUI(uiLayerTrans[UILayer.LobbyUI]);
        Blur ??= new BlurUI(uiLayerTrans[UILayer.BlurUI]);
        PopUp ??= new PopUpUI(uiLayerTrans[UILayer.PopUpUI]);
        Confirm ??= new ConfirmUI(uiLayerTrans[UILayer.ConfirmUI]);
        Tip ??= new TipUI(uiLayerTrans[UILayer.TipUI]);

        Lobby.Init();
        Blur.Init();
        PopUp.Init();
        Confirm.Init();
        Tip.Init();
    }

    private enum UILayer
    {
        LobbyUI,
        BlurUI,
        PopUpUI,
        ConfirmUI,
        TipUI,
    }
}
