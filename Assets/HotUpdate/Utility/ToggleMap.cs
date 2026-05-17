using HotUpdate.UI;
using UnityEngine;

namespace HotUpdate.Utility
{
    public class ToggleMap : MonoBehaviour
    {
        public bool wantToOpen;

public void Toggle()
        {
            if (UIManager.Instance is UIManager1 uiManager1)
            {
                if (wantToOpen)
                {
                    uiManager1.MapUI.OpenMapPanel();
                }
                else
                {
                    uiManager1.MapUI.CloseMapPanel();
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