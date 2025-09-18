using System;
using UnityEngine;
using UnityEngine.UI;
using static ShopManager;

public class ProductSlot : SlotBase
{
    [SerializeField] protected Text itemStockTxt;       // ��ǰ ���

    // �ݹ�: (itemId, qty) => bool success
    public Func<int, int, bool> OnBuyRequested;

    // ���� ����
    public override void SlotUpdate(ShopManager.ShopItem itemData)
    {
        base.SlotUpdate(itemData);

        // �⺻ ���� ����
        purchaseCount = (shopItem.stock > 0) ? 1 : 0;
        totalPrice = shopItem.price * purchaseCount;

        // �ؽ�Ʈ ����
        totalPriceTxt.text = totalPrice.ToString();
        purchaseCountTxt.text = $"x{purchaseCount}";

        // ��� ǥ��
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

    // ��ǰ �߰�
    public void Increse()
    {
        if (shopItem == null) return;
        if (purchaseCount >= shopItem.stock) return;

        purchaseCount++;
        totalPrice = shopItem.price * purchaseCount;

        totalPriceTxt.text = totalPrice.ToString();
        purchaseCountTxt.text = $"x{purchaseCount}";
    }

    // ��ǰ ����
    public void Decrese()
    {
        if (shopItem == null) return;
        if (purchaseCount <= 0) return;

        purchaseCount--;
        totalPrice = shopItem.price * purchaseCount;

        totalPriceTxt.text = totalPrice.ToString();
        purchaseCountTxt.text = $"x{purchaseCount}";
    }

    // ��ǰ ����
    public void Purchase()
    {
        if (shopItem == null) return;
        if (purchaseCount <= 0) return;

        // ���� �õ� �� �����ϸ� �׳� ����
        bool success = false;
        if (OnBuyRequested != null)
        {
            success = OnBuyRequested.Invoke(shopItem.id, purchaseCount);
        }
        else
        {
            // �ݹ��� ���� ���(������ġ) ���� ShopManager ȣ��
            success = ShopManager.Instance.AddToCart(shopItem, purchaseCount);
        }

        if (!success)
        {
            Debug.Log("Purchase failed: insufficient money or stock.");
            return;
        }

        // ���� �� ���� �ʱ�ȭ
        purchaseCount = (shopItem.stock > 0) ? 1 : 0;
        totalPrice = shopItem.price * purchaseCount;

        totalPriceTxt.text = totalPrice.ToString();
        purchaseCountTxt.text = $"x{purchaseCount}";
    }
}
