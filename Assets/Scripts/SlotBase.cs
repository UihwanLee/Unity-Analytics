using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class SlotBase : MonoBehaviour, ISlot
{
    [Header("��ǰ ����")]
    [SerializeField] protected Image itemSprite;        // ��ǰ �̹���
    [SerializeField] protected Text itemNameTxt;        // ��ǰ �̸�
    [SerializeField] protected Text itemPriceTxt;       // ��ǰ ����
    [SerializeField] protected Text totalPriceTxt;      // ���� �ݾ�
    [SerializeField] protected Text purchaseCountTxt;   // ���� ����

    [Header("Item ������")]
    public Item item;

    [Header("����")]
    protected ShopManager.ShopItem shopItem;            // ��ǰ ���纻
    protected int totalPrice;                           // ���� �ݾ�
    protected int purchaseCount;                        // ���� ����

    public virtual void SlotUpdate(ShopManager.ShopItem itemData)
    {
        if (itemData == null)
        {
            Debug.LogWarning("No item assigned to this slot!");
            ClearSlot();
            return;
        }

        shopItem = itemData;

        // UI ����
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
