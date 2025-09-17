using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("��ǰ ������ ����")]
    [SerializeField] private int maxProductSlots = 3;                   // ��ǰ ����Ʈ �ִ� ����
    [SerializeField] private Transform productListParent;               // ��ǰ ����Ʈ ��ġ
    [SerializeField] private GameObject productSlotPrefab;              // ��ǰ ���� ������
    [SerializeField] private Text productPageTxt;                       // ��ǰ������
    private List<GameObject> productSlotList = new List<GameObject>();  // ��ǰ ����Ʈ

    // ��Ÿ�� �� �׸�(Asset(Item) �������� ����)
    private List<ShopItem> shopItems = new List<ShopItem>();

    [Header("����īƮ ����")]
    [SerializeField] private Transform shoppingCartListParent;          // ���� ī�� ����Ʈ ��ġ
    [SerializeField] private GameObject shoppingCartSlotPrefab;         // ���� ī�� ���� ������
    
    private List<ShopItem> cartList = new List<ShopItem>();

    [Header("�÷��̾� ����")]
    [SerializeField] private Text userMoneyTxt;                         // �÷��̾� ���� �ݾ�

    [Header("Value")]
    private int userMoney = 1000000;
    private int maxPage;
    private int currentPage;

    public event Action<int> OnMoneyChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitShopItems();
        CreateSlotsIfNeeded();
        RefreshPagination();
        UpdateProductList();
        OnMoneyChanged += (m) => userMoneyTxt.text = m.ToString();
        OnMoneyChanged?.Invoke(userMoney);
    }

    // ��ǰ ����Ʈ �ʱ�ȭ
    public void InitShopItems()
    {
        // Resource/Items ���� �ȿ� �ִ� ������ ������ �ҷ��� ����Ʈȭ
        Item[] loaded = Resources.LoadAll<Item>("Items");
        shopItems = new List<ShopItem>();

        int id = 0;
        foreach (var it in loaded)
        {
            shopItems.Add(new ShopItem
            {
                id = id++,
                itemDef = it,
                stock = it.stock, // ��Ÿ�� ��� ����
                price = it.price
            });
        }
    }

    private void CreateSlotsIfNeeded()
    {
        for (int i = productSlotList.Count; i < maxProductSlots; i++)
        {
            var go = Instantiate(productSlotPrefab, productListParent);
            go.SetActive(false);
            productSlotList.Add(go);

            var slot = go.GetComponent<ProductSlot>();
            // �ݹ��� bool ��ȯ(���� ���� ����)
            slot.OnBuyRequested = (itemId, qty) =>
            {
                var s = shopItems.FirstOrDefault(x => x.id == itemId);
                return Purchase(s, qty); // Purchase�� bool ��ȯ���� �����Ǿ� �־�� ��
            };
        }
    }

    // ������ �ʱ�ȭ
    private void RefreshPagination()
    {
        maxPage = Mathf.Max(1, Mathf.CeilToInt(shopItems.Count / (float)maxProductSlots));
        currentPage = Mathf.Clamp(currentPage, 1, maxPage);
        productPageTxt.text = $"{currentPage}/{maxPage}"; 
    }

    // ��ǰ ����Ʈ ����
    public void UpdateProductList()
    {
        RefreshPagination();

        // ���� ����
        int start = (currentPage - 1) * maxProductSlots;
        for (int slotIdx = 0; slotIdx < productSlotList.Count; slotIdx++)
        {
            int dataIdx = start + slotIdx;
            var go = productSlotList[slotIdx];
            if (dataIdx < shopItems.Count)
            {
                go.SetActive(true);
                go.GetComponent<ProductSlot>().SlotUpdate(shopItems[dataIdx]);
            }
            else
            {
                go.GetComponent<ProductSlot>().ClearSlot();
                go.SetActive(false);
            }
        }
    }

    // ������ �� �ִ��� üũ
    public bool TryPurchase(ShopItem s, int qty)
    {
        if (s == null) return false;
        if (qty <= 0) return false;
        if (s.stock < qty) return false;
        return s.price * qty <= userMoney;
    }

    // �����ϱ�
    public bool Purchase(ShopItem s, int qty)
    {
        if (!TryPurchase(s, qty)) return false;

        s.stock -= qty;
        userMoney -= s.price * qty;

        // �� UI ����
        OnMoneyChanged?.Invoke(userMoney);

        //  ��� 0�̾ slot�� �״�� �ΰ� UI���� "��� ����"�� ǥ��
        if (s.stock < 0) s.stock = 0; // ���� ����

        UpdateProductList();
        return true;
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

    [System.Serializable]
    public class ShopItem
    {
        public int id;
        public Item itemDef;
        public int stock;
        public int price;
    }
}
