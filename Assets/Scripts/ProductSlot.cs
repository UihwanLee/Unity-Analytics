using UnityEngine;
using UnityEngine.UI;

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
    private int totalPrice;          // ���� �����Ϸ��� �� �ݾ�
    private int purchaseCount;       // ���� �����Ϸ��� ����

    // ���� ����
    public void SlotUpdate(Item _item)
    {
        if(_item == null)
        {
            Debug.Log("No have item in this slot!");
        }

        item = _item;

        productNameTxt.text = item.name;
        productCountTxt.text = "x" + item.count.ToString();
        productPriceTxt.text = item.price.ToString();
        if(item.sprite != null) productSprite.sprite = (Sprite)item.sprite;

        purchaseCount = (item.count > 0) ? 1 : 0;
        totalPrice = item.price * purchaseCount;

        totalPriceTxt.text = totalPrice.ToString();
        purchaseCountTxt.text = "x" + purchaseCount.ToString();
    }

    // ��ǰ �߰�
    public void Increse()
    {
        if (purchaseCount >= item.count) return;

        purchaseCount++;
        totalPrice = item.price * purchaseCount;

        totalPriceTxt.text = totalPrice.ToString();
        purchaseCountTxt.text = "x" + purchaseCount.ToString();
    }

    // ��ǰ ����
    public void Decrese()
    {
        if (purchaseCount <= 0) return;

        purchaseCount--;
        totalPrice = item.price * purchaseCount;

        totalPriceTxt.text = totalPrice.ToString();
        purchaseCountTxt.text = "x" + purchaseCount.ToString();
    }

    // ��ǰ ����
    public void Purchase()
    {
        // ����ڰ� ������ �� �ִ��� üũ
        if (ShopManager.Insatnce.TryPurchase(totalPrice) == false) return;

        // ���� üũ
        if (purchaseCount <= 0) return;

        ShopManager.Insatnce.Purchase(item, purchaseCount);
    }
}
