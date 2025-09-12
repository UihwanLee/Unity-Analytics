using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/Item")]
public class Item : ScriptableObject
{
    public string name;
    public Sprite sprite;
    public int price;
    public int count;

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
