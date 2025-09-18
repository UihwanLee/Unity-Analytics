using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("ShopItem Info")]
    [SerializeField] private int maxProductSlots = 3;                   // ��ǰ ����Ʈ �ִ� ����
    [SerializeField] private Transform productListParent;               // ��ǰ ����Ʈ ��ġ
    [SerializeField] private GameObject productSlotPrefab;              // ��ǰ ���� ������
    [SerializeField] private Text productPageTxt;                       // ��ǰ������
    private List<GameObject> productSlotList = new List<GameObject>();  // ��ǰ ����Ʈ

    [Header("Shopping Cart Info")]
    [SerializeField] private Transform shoppingCartListParent;          // ���� ī�� ����Ʈ ��ġ
    [SerializeField] private GameObject shoppingCartSlotPrefab;         // ���� ī�� ���� ������
    [SerializeField] private Text totalPriceTxt;                        // ��ü ǰ�� ����
   
    [Header("Player Info")]
    [SerializeField] private Text userMoneyTxt;                         // �÷��̾� ���� �ݾ�

    [Header("List")]
    // ��Ÿ�� �� �׸�(Asset(Item) �������� ����)
    [SerializeField] private List<ShopItem> shopItems = new List<ShopItem>();
    [SerializeField] private List<CartEntry> cartList = new List<CartEntry>();          

    [Header("Value")]
    private int totalPirce = 0;
    private int userMoney = 1000000;
    private int maxPage;
    private int currentPage;

    public event Action<int> OnTotalPriceChanged;
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

        OnTotalPriceChanged += (m) => totalPriceTxt.text = $"$ {m}";
        OnTotalPriceChanged?.Invoke(totalPirce);

        OnMoneyChanged += (m) => userMoneyTxt.text = m.ToString();
        OnMoneyChanged?.Invoke(userMoney);
    }

    // ��ǰ ����Ʈ �ʱ�ȭ
    public void InitShopItems()
    {
        // Resource/Items ���� �ȿ� �ִ� ������ ������ �ҷ��� ����Ʈȭ
        Item[] loaded = Resources.LoadAll<Item>("Items");
        shopItems = new List<ShopItem>();

        foreach (var it in loaded)
        {
            shopItems.Add(new ShopItem
            {
                id = it.id,
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
                return AddToCart(s, qty); // Purchase�� bool ��ȯ���� �����Ǿ� �־�� ��
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

    // ��ٱ��Ͽ� ���� �� �ִ��� üũ
    public bool TryPurchase(ShopItem s, int qty)
    {
        if (s == null) return false;
        if (qty <= 0) return false;
        if (s.stock < qty) return false;
        return s.price * qty <= userMoney;
    }

    // ��ٱ��� ���
    public bool AddToCart(ShopItem s, int qty)
    {
        if (!TryPurchase(s, qty)) return false;

        // �̹� ��ٱ��Ͽ� ���� ��ǰ�� ������ ������ ����
        var existing = cartList.FirstOrDefault(x => x.item.id == s.id);
        if (existing != null)
        {
            existing.qty += qty;
        }
        else
        {
            cartList.Add(new CartEntry { item = s, qty = qty });
        }

        Debug.Log($"��ٱ��Ͽ� ����: {s.itemDef.name} x{qty}");

        s.stock -= qty;
        //  ��� 0�̾ slot�� �״�� �ΰ� UI���� "��� ����"�� ǥ��
        if (s.stock < 0) s.stock = 0; // ���� ����

        UpdateProductList();
        RefreshCartUI();

        return true;
    }

    // ��ٱ��� ��ü ���
    public void ClearCart()
    {
        cartList.Clear();
        RefreshCartUI();
    }

    // ��ٱ��� ����
    public void CancelAllCart()
    {
        // cartList�� �ִ� �͵� ��� ����
        foreach (var entry in cartList)
        {
            var item = shopItems.Find(x => x.itemDef != null && x.id == entry.item.id);
            if (item != null)
            {
                item.stock += entry.qty;
            }
        }

        cartList.Clear();
        RefreshCartUI();

        UpdateProductList();
    }

    // ��ٱ��� ���
    public void CancelCartItem(int id)
    {
        var entry = cartList.Find(x => x.item.id == id);
        if (entry != null)
        {
            // shopItems ��� ����
            var item = shopItems.Find(x => x.itemDef != null && x.id == id);
            if (item != null)
            {
                item.stock += entry.qty;
            }

            // ��ٱ��Ͽ��� ����
            cartList.Remove(entry);
        }

        RefreshCartUI();

        UpdateProductList();
    }

    private void RefreshCartUI()
    {
        // ���� ���� ����
        foreach (Transform child in shoppingCartListParent)
        {
            Destroy(child.gameObject);
        }

        totalPirce = 0;

        // ���� �׸���
        foreach (var entry in cartList)
        {
            var go = Instantiate(shoppingCartSlotPrefab, shoppingCartListParent);
            var slot = go.GetComponent<ShoppingCartSlot>();
            slot.SlopUpdateWithQty(entry.item, entry.qty);
            totalPirce += entry.item.price * entry.qty;
        }

        OnTotalPriceChanged?.Invoke(totalPirce);
    }

    // ��ٱ��� ��ü ����
    public void ConfirmPurchase()
    {
        int totalCost = cartList.Sum(x => x.item.price * x.qty);

        if (totalCost > userMoney)
        {
            Debug.Log("���� ����: �ܾ� ����");
            return;
        }

        // ���� ����
        foreach (var entry in cartList)
        {
            entry.item.stock -= entry.qty;
            if (entry.item.stock < 0) entry.item.stock = 0;
        }

        userMoney -= totalCost;
        OnMoneyChanged?.Invoke(userMoney);

        Debug.Log($"�� {totalCost}�� ���� �Ϸ�");
        ClearCart();
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

    [System.Serializable]
    public class ShopItem
    {
        public int id;
        public Item itemDef;
        public int stock;
        public int price;
    }

    [System.Serializable]
    public class CartEntry
    {
        public ShopItem item;
        public int qty;
    }
}
