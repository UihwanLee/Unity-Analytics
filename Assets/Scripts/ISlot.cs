using UnityEngine;

public interface ISlot
{
    void SlotUpdate(ShopManager.ShopItem itemData);
    void ClearSlot();
}
