using HotUpdate.UI;
using UnityEngine;

namespace HotUpdate.Utility
{
    public class ToggleMap : MonoBehaviour
    {
        public bool wantToOpen;

        public void Toggle()
        {
            if (wantToOpen)
                UIManager.Instance.OpenMapPanel();
            else
                UIManager.Instance.CloseMapPanel();
        }

        public void ToggleWithItemData(IItemEventData itemEventData)
        {
            Toggle();
            itemEventData.SetSuccess(true);
        }
    }
}