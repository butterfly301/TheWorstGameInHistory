using HotUpdate.UI;
using UnityEngine;

public class BagPanel : MonoBehaviour, OptionPanelChildren
{
    private IInventory inventory;

    public void Init()
    {
        //inventory = GetComponent<IInventory>();
        //UIManager.Instance.GetInventory().Add(inventory);
    }
}