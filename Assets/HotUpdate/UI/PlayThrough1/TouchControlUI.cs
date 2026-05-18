using HotUpdate.Manager;
using HotUpdate.Utility;
using UnityEngine;

namespace HotUpdate.UI
{
    public class TouchControlUI
    {
        private readonly Transform parentTransform;
        private GameObject touchControlsPrefab;
        private GameObject touchControlsObj;

        public TouchControlUI(Transform parent)
        {
            parentTransform = parent;
        }

        public void Init()
        {
            if (touchControlsObj != null) return;

            if (!Application.isMobilePlatform)
            {
                return;
            }

            AddressablesManager.Instance.LoadAssetAsync<GameObject>(
                AddressableKeys.TouchControls_Prefab,
                handle =>
                {
                    touchControlsPrefab = handle.Result;
                    touchControlsObj = Object.Instantiate(touchControlsPrefab, parentTransform);
                    Close();
                }
            );
        }

        public void Open()
        {
            if (touchControlsObj != null)
            {
                touchControlsObj.SetActive(true);
            }
        }

        public void Close()
        {
            if (touchControlsObj != null)
            {
                touchControlsObj.SetActive(false);
            }
        }
    }
}
