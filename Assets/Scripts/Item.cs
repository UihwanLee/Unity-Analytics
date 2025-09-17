using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/Item")]
public class Item : ScriptableObject
{
    public int id;
    public string name;
    public Sprite sprite;
    public int price;
    public int stock;

    public enum itemType
    {
        Weapon,
        Portion
    }

    public enum itemState
    {
        Product,
        Item
    }
}
