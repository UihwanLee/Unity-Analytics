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
    [SerializeField] private int maxProductSlots = 3;                   // 상품 리스트 최대 개수
    [SerializeField] private Transform productListParent;               // 상품 리스트 위치
    [SerializeField] private GameObject productSlotPrefab;              // 상품 슬롯 프리팹
    [SerializeField] private Text productPageTxt;                       // 상품페이지
    private List<GameObject> productSlotList = new List<GameObject>();  // 상품 리스트

    [Header("Shopping Cart Info")]
    [SerializeField] private Transform shoppingCartListParent;          // 쇼핑 카드 리스트 위치
    [SerializeField] private GameObject shoppingCartSlotPrefab;         // 쇼핑 카드 슬롯 프리팹
    [SerializeField] private Text totalPriceTxt;                        // 전체 품목 가격
   
    [Header("Player Info")]
    [SerializeField] private Text userMoneyTxt;                         // 플레이어 보유 금액

    [Header("List")]
    // 런타임 샵 항목(Asset(Item) 수정하지 않음)
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

    // 상품 리스트 초기화
    public void InitShopItems()
    {
        // Resource/Items 폴더 안에 있는 아이템 정보를 불러와 리스트화
        Item[] loaded = Resources.LoadAll<Item>("Items");
        shopItems = new List<ShopItem>();

        foreach (var it in loaded)
        {
            shopItems.Add(new ShopItem
            {
                id = it.id,
                itemDef = it,
                stock = it.stock, // 런타임 재고 복사
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
            // 콜백은 bool 반환(구매 성공 여부)
            slot.OnBuyRequested = (itemId, qty) =>
            {
                var s = shopItems.FirstOrDefault(x => x.id == itemId);
                return AddToCart(s, qty); // Purchase는 bool 반환으로 구현되어 있어야 함
            };
        }
    }

    // 페이지 초기화
    private void RefreshPagination()
    {
        maxPage = Mathf.Max(1, Mathf.CeilToInt(shopItems.Count / (float)maxProductSlots));
        currentPage = Mathf.Clamp(currentPage, 1, maxPage);
        productPageTxt.text = $"{currentPage}/{maxPage}"; 
    }

    // 상품 리스트 갱신
    public void UpdateProductList()
    {
        RefreshPagination();

        // 범위 설정
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

    // 장바구니에 담을 수 있는지 체크
    public bool TryPurchase(ShopItem s, int qty)
    {
        if (s == null) return false;
        if (qty <= 0) return false;
        if (s.stock < qty) return false;
        return s.price * qty <= userMoney;
    }

    // 장바구니 담기
    public bool AddToCart(ShopItem s, int qty)
    {
        if (!TryPurchase(s, qty)) return false;

        // 이미 장바구니에 같은 상품이 있으면 수량만 증가
        var existing = cartList.FirstOrDefault(x => x.item.id == s.id);
        if (existing != null)
        {
            existing.qty += qty;
        }
        else
        {
            cartList.Add(new CartEntry { item = s, qty = qty });
        }

        Debug.Log($"장바구니에 담음: {s.itemDef.name} x{qty}");

        s.stock -= qty;
        //  재고가 0이어도 slot은 그대로 두고 UI에서 "재고 없음"만 표시
        if (s.stock < 0) s.stock = 0; // 음수 방지

        UpdateProductList();
        RefreshCartUI();

        return true;
    }

    // 장바구니 전체 취소
    public void ClearCart()
    {
        cartList.Clear();
        RefreshCartUI();
    }

    // 장바구니 비우기
    public void CancelAllCart()
    {
        // cartList에 있는 것들 재고 복구
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

    // 장바구니 취소
    public void CancelCartItem(int id)
    {
        var entry = cartList.Find(x => x.item.id == id);
        if (entry != null)
        {
            // shopItems 재고 복구
            var item = shopItems.Find(x => x.itemDef != null && x.id == id);
            if (item != null)
            {
                item.stock += entry.qty;
            }

            // 장바구니에서 제거
            cartList.Remove(entry);
        }

        RefreshCartUI();

        UpdateProductList();
    }

    private void RefreshCartUI()
    {
        // 기존 슬롯 제거
        foreach (Transform child in shoppingCartListParent)
        {
            Destroy(child.gameObject);
        }

        totalPirce = 0;

        // 새로 그리기
        foreach (var entry in cartList)
        {
            var go = Instantiate(shoppingCartSlotPrefab, shoppingCartListParent);
            var slot = go.GetComponent<ShoppingCartSlot>();
            slot.SlopUpdateWithQty(entry.item, entry.qty);
            totalPirce += entry.item.price * entry.qty;
        }

        OnTotalPriceChanged?.Invoke(totalPirce);
    }

    // 장바구니 전체 구매
    public void ConfirmPurchase()
    {
        int totalCost = cartList.Sum(x => x.item.price * x.qty);

        if (totalCost > userMoney)
        {
            Debug.Log("구매 실패: 잔액 부족");
            return;
        }

        // 결제 진행
        foreach (var entry in cartList)
        {
            entry.item.stock -= entry.qty;
            if (entry.item.stock < 0) entry.item.stock = 0;
        }

        userMoney -= totalCost;
        OnMoneyChanged?.Invoke(userMoney);

        Debug.Log($"총 {totalCost}원 결제 완료");
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
