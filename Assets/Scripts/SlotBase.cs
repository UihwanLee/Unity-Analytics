using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class SlotBase : MonoBehaviour, ISlot
{
    [Header("상품 정보")]
    [SerializeField] protected Image itemSprite;        // 상품 이미지
    [SerializeField] protected Text itemNameTxt;        // 상품 이름
    [SerializeField] protected Text itemPriceTxt;       // 상품 가격
    [SerializeField] protected Text totalPriceTxt;      // 구매 금액
    [SerializeField] protected Text purchaseCountTxt;   // 구매 수량

    [Header("Item 데이터")]
    public Item item;

    [Header("정보")]
    protected ShopManager.ShopItem shopItem;            // 상품 복사본
    protected int totalPrice;                           // 구매 금액
    protected int purchaseCount;                        // 구매 수량

    public virtual void SlotUpdate(ShopManager.ShopItem itemData)
    {
        if (itemData == null)
        {
            Debug.LogWarning("No item assigned to this slot!");
            ClearSlot();
            return;
        }

        shopItem = itemData;

        // UI 갱신
        itemNameTxt.text = shopItem.itemDef.name;
        itemPriceTxt.text = shopItem.price.ToString();
        if (shopItem.itemDef.sprite != null)
            itemSprite.sprite = shopItem.itemDef.sprite;
    }

    public virtual void ClearSlot()
    {
        shopItem = null;
        itemNameTxt.text = "";
        itemPriceTxt.text = "";
        itemSprite.sprite = null;
        totalPriceTxt.text = "0";
        purchaseCountTxt.text = "x0";
        purchaseCount = 0;
        totalPrice = 0;
    }
}
