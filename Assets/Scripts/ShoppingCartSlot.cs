using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ShoppingCartSlot : SlotBase
{
    [SerializeField] private Text itemId;

    public override void SlotUpdate(ShopManager.ShopItem itemData)
    {
        base.SlotUpdate(itemData);
    }

    public void SlopUpdateWithQty(ShopManager.ShopItem itemData, int qty)
    {
        SlotUpdate(itemData);

        // 텍스트 갱신
        itemId.text = $"id: {shopItem.id}";
        totalPrice = qty * shopItem.price;
        purchaseCount = qty;
        totalPriceTxt.text = totalPrice.ToString();
        purchaseCountTxt.text = $"x{purchaseCount}";
    }

    // 장바구니 취소
    public void CacelSlot()
    {
        ShopManager.Instance.CancelCartItem(shopItem.id);
    }
}
