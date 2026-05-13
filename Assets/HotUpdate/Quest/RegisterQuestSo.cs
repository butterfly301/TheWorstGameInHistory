using HotUpdate.Manager;
using UnityEngine;

namespace HotUpdate.Quest
{
    [RequireComponent(typeof(IQuest))]
    public class RegisterQuestSo : MonoBehaviour
    {
        public string questName;
        private readonly string persistentQuestPath = "Assets/ScriptableObjects/Quest/";
        private IQuest quest;

        private void Awake()
        {
            var questPath = persistentQuestPath + questName + ".asset";
            AddressablesManager.Instance.LoadAssetAsync<IQuestSO>(questPath,
                handle => { GetComponent<IQuest>().SetQuestSO(handle.Result); });
        }
    }
}