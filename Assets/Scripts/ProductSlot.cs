using System;
using UnityEngine;
using UnityEngine.UI;
using static ShopManager;

public class ProductSlot : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] private Image productSprite;       // 상품 이미지
    [SerializeField] private Text productNameTxt;       // 상품 이름
    [SerializeField] private Text productPriceTxt;      // 상품 가격
    [SerializeField] private Text productCountTxt;      // 상품 수량
    [SerializeField] private Text totalPriceTxt;        // 구매 금액
    [SerializeField] private Text purchaseCountTxt;     // 구매 수량

    [Header("Item")]
    public Item item;

    [Header("Info")]
    private ShopManager.ShopItem shopItem;
    private int totalPrice;          // 현재 구매하려는 총 금액
    private int purchaseCount;       // 현재 구매하려는 수량

    // 콜백: (itemId, qty) => bool success
    public Func<int, int, bool> OnBuyRequested;

    // 슬롯 갱신
    public void SlotUpdate(ShopManager.ShopItem itemData)
    {
        if(itemData == null)
        {
            Debug.LogWarning("No item assigned to this slot!");
            ClearSlot();
            return;
        }

        shopItem = itemData;

        // UI 갱신
        productNameTxt.text = shopItem.itemDef.name;
        productPriceTxt.text = shopItem.price.ToString();
        if (shopItem.itemDef.sprite != null)
            productSprite.sprite = shopItem.itemDef.sprite;

        // 기본 구매 수량 (재고 있을 때만 1개)
        purchaseCount = (shopItem.stock > 0) ? 1 : 0;
        totalPrice = shopItem.price * purchaseCount;

        // 텍스트 갱신
        totalPriceTxt.text = totalPrice.ToString();
        purchaseCountTxt.text = "x" + purchaseCount.ToString();

        // 재고 표시
        if (shopItem.stock <= 0)
        {
            productCountTxt.text = "Sold Out";
            productCountTxt.color = Color.red;
        }
        else
        {
            productCountTxt.text = "x" + shopItem.stock;
            productCountTxt.color = Color.black;
        }
    }

    // 슬롯 비우기
    public void ClearSlot()
    {
        shopItem = null;
        productNameTxt.text = "";
        productCountTxt.text = "";
        productPriceTxt.text = "";
        productSprite.sprite = null;
        totalPriceTxt.text = "0";
        purchaseCountTxt.text = "x0";
        purchaseCount = 0;
        totalPrice = 0;
    }

    // 상품 추가
    public void Increse()
    {
        if (shopItem == null) return;
        if (purchaseCount >= shopItem.stock) return;

        purchaseCount++;
        totalPrice = shopItem.price * purchaseCount;

        totalPriceTxt.text = totalPrice.ToString();
        purchaseCountTxt.text = "x" + purchaseCount.ToString();
    }

    // 상품 제거
    public void Decrese()
    {
        if (shopItem == null) return;
        if (purchaseCount <= 0) return;

        purchaseCount--;
        totalPrice = shopItem.price * purchaseCount;

        totalPriceTxt.text = totalPrice.ToString();
        purchaseCountTxt.text = "x" + purchaseCount.ToString();
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
            success = ShopManager.Instance.Purchase(shopItem, purchaseCount);
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
        purchaseCountTxt.text = "x" + purchaseCount.ToString();
    }
}
