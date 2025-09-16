using UnityEngine;
using UnityEngine.UI;

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
    private int totalPrice;          // 현재 구매하려는 총 금액
    private int purchaseCount;       // 현재 구매하려는 수량

    // 슬롯 갱신
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

    // 상품 추가
    public void Increse()
    {
        if (purchaseCount >= item.count) return;

        purchaseCount++;
        totalPrice = item.price * purchaseCount;

        totalPriceTxt.text = totalPrice.ToString();
        purchaseCountTxt.text = "x" + purchaseCount.ToString();
    }

    // 상품 제거
    public void Decrese()
    {
        if (purchaseCount <= 0) return;

        purchaseCount--;
        totalPrice = item.price * purchaseCount;

        totalPriceTxt.text = totalPrice.ToString();
        purchaseCountTxt.text = "x" + purchaseCount.ToString();
    }

    // 상품 구매
    public void Purchase()
    {
        // 사용자가 구매할 수 있는지 체크
        if (ShopManager.Insatnce.TryPurchase(totalPrice) == false) return;

        // 수량 체크
        if (purchaseCount <= 0) return;

        ShopManager.Insatnce.Purchase(item, purchaseCount);
    }
}
