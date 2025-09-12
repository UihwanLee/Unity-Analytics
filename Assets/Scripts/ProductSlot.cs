using UnityEngine;
using UnityEngine.UI;

public class ProductSlot : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] private Image productSprite;
    [SerializeField] private Text productName;
    [SerializeField] private Text productPrice;
    [SerializeField] private Text productCount;
    [SerializeField] private Text totalPrice;

    [Header("Item")]
    public Item item;
    
    // ½½·Ô °»½Å
    public void SlotUpdate(Item _item)
    {
        if(_item == null)
        {
            Debug.Log("No have item in this slot!");
        }

        item = _item;

        productName.text = item.name;
        productCount.text = "x" + item.count.ToString();
        productPrice.text = item.price.ToString();
        if(item.sprite != null) productSprite.sprite = (Sprite)item.sprite;
    }
}
