using System;
using UnityEngine;
using UnityEngine.UI;
using static ShopManager;

public class ProductSlot : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] private Image productSprite;       // ��ǰ �̹���
    [SerializeField] private Text productNameTxt;       // ��ǰ �̸�
    [SerializeField] private Text productPriceTxt;      // ��ǰ ����
    [SerializeField] private Text productCountTxt;      // ��ǰ ����
    [SerializeField] private Text totalPriceTxt;        // ���� �ݾ�
    [SerializeField] private Text purchaseCountTxt;     // ���� ����

    [Header("Item")]
    public Item item;

    [Header("Info")]
    private ShopManager.ShopItem shopItem;
    private int totalPrice;          // ���� �����Ϸ��� �� �ݾ�
    private int purchaseCount;       // ���� �����Ϸ��� ����

    // �ݹ�: (itemId, qty) => bool success
    public Func<int, int, bool> OnBuyRequested;

    // ���� ����
    public void SlotUpdate(ShopManager.ShopItem itemData)
    {
        if(itemData == null)
        {
            Debug.LogWarning("No item assigned to this slot!");
            ClearSlot();
            return;
        }

        shopItem = itemData;

        // UI ����
        productNameTxt.text = shopItem.itemDef.name;
        productPriceTxt.text = shopItem.price.ToString();
        if (shopItem.itemDef.sprite != null)
            productSprite.sprite = shopItem.itemDef.sprite;

        // �⺻ ���� ���� (��� ���� ���� 1��)
        purchaseCount = (shopItem.stock > 0) ? 1 : 0;
        totalPrice = shopItem.price * purchaseCount;

        // �ؽ�Ʈ ����
        totalPriceTxt.text = totalPrice.ToString();
        purchaseCountTxt.text = "x" + purchaseCount.ToString();

        // ��� ǥ��
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

    // ���� ����
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

    // ��ǰ �߰�
    public void Increse()
    {
        if (shopItem == null) return;
        if (purchaseCount >= shopItem.stock) return;

        purchaseCount++;
        totalPrice = shopItem.price * purchaseCount;

        totalPriceTxt.text = totalPrice.ToString();
        purchaseCountTxt.text = "x" + purchaseCount.ToString();
    }

    // ��ǰ ����
    public void Decrese()
    {
        if (shopItem == null) return;
        if (purchaseCount <= 0) return;

        purchaseCount--;
        totalPrice = shopItem.price * purchaseCount;

        totalPriceTxt.text = totalPrice.ToString();
        purchaseCountTxt.text = "x" + purchaseCount.ToString();
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
            success = ShopManager.Instance.Purchase(shopItem, purchaseCount);
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
        purchaseCountTxt.text = "x" + purchaseCount.ToString();
    }
}
