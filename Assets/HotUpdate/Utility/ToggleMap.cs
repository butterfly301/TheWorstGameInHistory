using HotUpdate.UI;
using UnityEngine;

namespace HotUpdate.Utility
{
    public class ToggleMap : MonoBehaviour
    {
        public bool wantToOpen;

public void Toggle()
        {
            if (UIManager1.Instance != null)
            {
                if (wantToOpen)
                {
                    UIManager1.Instance.MapUI.OpenMapPanel();
                }
                else
                {
                    UIManager1.Instance.MapUI.CloseMapPanel();
                }
            }
        }

public void ToggleWithItemData(IItemEventData itemEventData)
        {
            Toggle();
            itemEventData.SetSuccess(true);
        }
    }
}
