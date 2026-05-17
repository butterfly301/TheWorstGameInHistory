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
            if (UIManager1.Instance != null)
            {
                inventory = UIManager1.Instance.MapUI.GetInventory();
            }
            //pickUpItems = GetComponentInChildren<IPickUpItems>();
            //pickUpItems.Initialize(inventory);
        }
    }
}
