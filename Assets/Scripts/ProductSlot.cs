using System;
using UnityEngine;
using UnityEngine.UI;
using static ShopManager;

public class ProductSlot : SlotBase
{
    [SerializeField] protected Text itemStockTxt;       // 상품 재고

    // 콜백: (itemId, qty) => bool success
    public Func<int, int, bool> OnBuyRequested;

    // 슬롯 갱신
    public override void SlotUpdate(ShopManager.ShopItem itemData)
    {
        base.SlotUpdate(itemData);

        // 기본 구매 수량
        purchaseCount = (shopItem.stock > 0) ? 1 : 0;
        totalPrice = shopItem.price * purchaseCount;

        // 텍스트 갱신
        totalPriceTxt.text = totalPrice.ToString();
        purchaseCountTxt.text = $"x{purchaseCount}";

        // 재고 표시
        if (shopItem.stock <= 0)
        {
            itemStockTxt.text = "Sold Out";
            itemStockTxt.color = Color.red;
        }
        else
        {
            itemStockTxt.text = $"x{shopItem.stock}";
            itemStockTxt.color = Color.black;
        }
    }

    public override void ClearSlot()
    {
        base.ClearSlot();
        itemStockTxt.text = "";
    }

    // 상품 추가
    public void Increse()
    {
        if (shopItem == null) return;
        if (purchaseCount >= shopItem.stock) return;

        purchaseCount++;
        totalPrice = shopItem.price * purchaseCount;

        totalPriceTxt.text = totalPrice.ToString();
        purchaseCountTxt.text = $"x{purchaseCount}";
    }

    // 상품 제거
    public void Decrese()
    {
        if (shopItem == null) return;
        if (purchaseCount <= 0) return;

        purchaseCount--;
        totalPrice = shopItem.price * purchaseCount;

        totalPriceTxt.text = totalPrice.ToString();
        purchaseCountTxt.text = $"x{purchaseCount}";
    }

    // 상품 구매
    public void Purchase()
    {
        if (shopItem == null) return;
        if (purchaseCount <= 0) return;

        // 구매 시도 → 실패하면 그냥 리턴
        bool success = false;
        if (OnBuyRequested != null)
        {
            success = OnBuyRequested.Invoke(shopItem.id, purchaseCount);
        }
        else
        {
            // 콜백이 없을 경우(안전장치) 직접 ShopManager 호출
            success = ShopManager.Instance.AddToCart(shopItem, purchaseCount);
        }

        if (!success)
        {
            Debug.Log("Purchase failed: insufficient money or stock.");
            return;
        }

        // 구매 후 수량 초기화
        purchaseCount = (shopItem.stock > 0) ? 1 : 0;
        totalPrice = shopItem.price * purchaseCount;

        totalPriceTxt.text = totalPrice.ToString();
        purchaseCountTxt.text = $"x{purchaseCount}";
    }
}
