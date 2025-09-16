using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Insatnce;

    [Header("ProductArea")]
    private int MAX_PRODUCTSLOT = 4;                                                                // ��ǰ ����Ʈ �ִ� ����

    [SerializeField] private int category;                                                          // ���� ī�װ�
    [SerializeField] private Transform listParent;                                                  // ��ǰ ����Ʈ ��ġ
    [SerializeField] private GameObject productSlotPrefab;                                          // ��ǰ ���� ������
    [SerializeField] private List<GameObject> productSlotList = new List<GameObject>();             // ��ǰ ���� ����Ʈ
    [SerializeField] private List<Item> productList = new List<Item>();                             // ��ǰ ����Ʈ

    [SerializeField] private Text pageTxt;                                                          // ��ǰ ������

    [Header("PlayerArea")]
    [SerializeField] private Text userMoneyTxt;                                                     // �÷��̾� ���� �ݾ�

    [Header("Value")]
    private int userMoney = 10000;
    private int maxPage;
    private int currentPage;

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

        userMoneyTxt.text = userMoney.ToString();
    }

    // ��ǰ ����Ʈ �ʱ�ȭ
    public void InitProductList()
    {
        // Resource/Items ���� �ȿ� �ִ� ������ ������ �ҷ��� ����Ʈȭ
        Item[] loadItems = Resources.LoadAll<Item>("Items");
        productList = new List<Item>(loadItems);

        // ������ �ʱ�ȭ
        maxPage = Mathf.Max(1, Mathf.CeilToInt(productList.Count / (float)MAX_PRODUCTSLOT));
        currentPage = Mathf.Clamp(currentPage, 1, maxPage);

        // ��ǰ ���� 4�� ����
        for (int i=0; i<MAX_PRODUCTSLOT; i++)
        {
            GameObject newProductSlot = Instantiate(productSlotPrefab, listParent);
            productSlotList.Add(newProductSlot);
        }

        UpdateProductList();
    }

    // ��ǰ ����Ʈ ����
    public void UpdateProductList()
    {
        // ���� ����
        int startIndex = (currentPage - 1) * 4;
        int endIndex = startIndex + MAX_PRODUCTSLOT;
        int slotIndex = 0;

        // 4���� ������ ������ ���� �� ����Ʈ ����
        for (int i = startIndex; i < endIndex; i++)
        {
            if (i >= productList.Count || productList[i] == null) break;

            productSlotList[slotIndex].GetComponent<ProductSlot>().SlotUpdate(productList[i]);
            productSlotList[slotIndex].SetActive(true);
            slotIndex++;
        }

        // �� ��ǰ ���� ��Ȱ��ȭ
        for(int i=slotIndex; i < MAX_PRODUCTSLOT; i++)
        {
            productSlotList[i].SetActive(false);
        }

        // ��ǰ ������ ����
        pageTxt.text = currentPage.ToString() + "/" + maxPage.ToString();
    }

    // ������ �� �ִ��� üũ
    public bool TryPurchase(int totalPrice)
    {
        return (totalPrice <= userMoney);
    }

    // �����ϱ�
    public void Purchase(Item item, int purchaseCount)
    {
        // ItemSlot���� ������ ����Ʈ ����
        item.count -= purchaseCount;
        userMoney -= item.price * purchaseCount;
        userMoneyTxt.text = userMoney.ToString();

        // ������ �κ��丮�� itme �߰�

        UpdateProductList();
    }

    public void Prev()
    {
        if (currentPage <= 1) return;

        currentPage--;
        UpdateProductList();
    }

    public void Next()
    {
        if (currentPage >= maxPage) return;

        currentPage++;
        UpdateProductList();
    }
}
