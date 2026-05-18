using HotUpdate.Manager;
using HotUpdate.Utility;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HotUpdate.UI
{
    public class PauseUI
    {
        private readonly Transform parentTransform;
        private GameObject pauseUIPrefab;
        private GameObject pauseUIObj;

        public PauseUI(Transform parent)
        {
            parentTransform = parent;
        }

        public void Init()
        {
            if (pauseUIObj != null) return;

            AddressablesManager.Instance.LoadAssetAsync<GameObject>(
                AddressableKeys.PauseForm_Prefab,
                handle =>
                {
                    pauseUIPrefab = handle.Result;
                    pauseUIObj = Object.Instantiate(pauseUIPrefab, parentTransform);
                    pauseUIObj.GetComponent<global::PauseForm>().Init();
                    Close();
                }
            );

            RegisterPauseEvent();
        }

        public void Open()
        {
            if (pauseUIObj != null)
            {
                pauseUIObj.SetActive(true);
            }
        }

        public void Close()
        {
            if (pauseUIObj != null)
            {
                pauseUIObj.SetActive(false);
            }
        }

        private void RegisterPauseEvent()
        {
            WorldManagerBase.Instance.RegisterEvent("onPause", Open);
            WorldManagerBase.Instance.RegisterEvent("onUnpause", Close);
        }
    }
}
