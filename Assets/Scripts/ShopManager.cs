using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Insatnce;

    private int MAX_PRODUCTSLOT = 4;    // ��ǰ ����Ʈ �ִ� ����

    [SerializeField] private int category;  // ���� ī�װ�
    [SerializeField] private Transform listParent;  // ��ǰ ����Ʈ ��ġ
    [SerializeField] private GameObject productSlotPrefab; // ��ǰ ���� ������
    [SerializeField] private List<GameObject> productSlotList = new List<GameObject>(); // ��ǰ ����Ʈ
    [SerializeField] private List<Item> productList = new List<Item>(); // ��ǰ ����Ʈ

    private void Awake()
    {
        if(Insatnce == null)
        {
            Insatnce = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitProductList();
    }

    // ��ǰ ����Ʈ �ʱ�ȭ
    public void InitProductList()
    {
        // Resource/Items ���� �ȿ� �ִ� ������ ������ �ҷ��� ����Ʈȭ
        Item[] loadItems = Resources.LoadAll<Item>("Items");
        productList = new List<Item>(loadItems);

        // 4���� ������ ������ ���� �� ����Ʈ ����
        for (int i=0; i< productList.Count; i++)
        {
            if (i == MAX_PRODUCTSLOT) break;

            GameObject newProductSlot = Instantiate(productSlotPrefab, listParent);
            newProductSlot.GetComponent<ProductSlot>().SlotUpdate(productList[i]);
            productSlotList.Add(newProductSlot);
        }
    }

    // ��ǰ ����Ʈ ����
    public void UpdateProductList()
    {
        
    }

    // �����ϱ�
    public void Purchase(Item item)
    {
        // ItemSlot���� ������ ����Ʈ ����
    }
}
