using UnityEngine;
using HotUpdate.Manager;
using HotUpdate.Utility;
using Object = UnityEngine.Object;

public class LobbyUI
{
    private readonly Transform parentTransform;
    private GameObject lobbyFormPrefab;
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
        Debug.Log("Initializing LobbyUI...");
        AddressablesManager.Instance.LoadAssetAsync<GameObject>(
            AddressableKeys.LobbyForm_Prefab,
            handle =>
            {
                lobbyFormPrefab = handle.Result;
                var lobbyFormObj = Object.Instantiate(lobbyFormPrefab, parentTransform);
                lobbyForm = lobbyFormObj.GetComponent<LobbyForm>();
                lobbyForm.Init();
            }
        );
    }
}
