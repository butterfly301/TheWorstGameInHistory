using System.Collections.Generic;
using HotUpdate.UI;

namespace HotUpdate.Character.Player
{
    public class PlayerReference : CharacterReference
    {
        private List<IInventory> inventory = new();
        private IPickUpItems pickUpItems;

public override void Init()
        {
            base.Init();
            if (UIManager.Instance is UIManager1 uiManager1)
            {
                inventory = uiManager1.MapUI.GetInventory();
            }
            //pickUpItems = GetComponentInChildren<IPickUpItems>();
            //pickUpItems.Initialize(inventory);
        }
    }
}