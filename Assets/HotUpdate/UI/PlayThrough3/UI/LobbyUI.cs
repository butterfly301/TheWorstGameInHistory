using UnityEngine;
using HotUpdate.Manager;
using HotUpdate.Utility;
using Object = UnityEngine.Object;

public class LobbyUI
{
    private readonly Transform parentTransform;
    private GameObject lobbyFormPrefab;
    private GameObject lobbyFormObj;
    private LobbyForm lobbyForm;

    public LobbyUI(Transform parent)
    {
        parentTransform = parent;
    }

/// <summary>
    /// 初始化大厅UI系统
    /// </summary>
    public void Init()
    {
        if (lobbyFormObj != null)   return;

        AddressablesManager.Instance.LoadAssetAsync<GameObject>(
            AddressableKeys.LobbyForm_Prefab,
            handle =>
            {
                lobbyFormPrefab = handle.Result;
                lobbyFormObj = Object.Instantiate(lobbyFormPrefab, parentTransform);
                lobbyForm = lobbyFormObj.GetComponent<LobbyForm>();
                lobbyForm.Init();
                Close();
            }
        );
    }

    public void Open()
    {
        lobbyFormObj?.SetActive(true);
    }

    public void Close()
    {
        lobbyFormObj?.SetActive(false);
    }
}
