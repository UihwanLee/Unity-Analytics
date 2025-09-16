using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Insatnce;

    [Header("ProductArea")]
    private int MAX_PRODUCTSLOT = 4;                                                                // 상품 리스트 최대 개수

    [SerializeField] private int category;                                                          // 상점 카테고리
    [SerializeField] private Transform listParent;                                                  // 상품 리스트 위치
    [SerializeField] private GameObject productSlotPrefab;                                          // 상품 슬롯 프리팹
    [SerializeField] private List<GameObject> productSlotList = new List<GameObject>();             // 상품 슬롯 리스트
    [SerializeField] private List<Item> productList = new List<Item>();                             // 상품 리스트

    [SerializeField] private Text pageTxt;                                                          // 상품 페이지

    [Header("PlayerArea")]
    [SerializeField] private Text userMoneyTxt;                                                     // 플레이어 보유 금액

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

    // 상품 리스트 초기화
    public void InitProductList()
    {
        // Resource/Items 폴더 안에 있는 아이템 정보를 불러와 리스트화
        Item[] loadItems = Resources.LoadAll<Item>("Items");
        productList = new List<Item>(loadItems);

        // 페이지 초기화
        maxPage = Mathf.Max(1, Mathf.CeilToInt(productList.Count / (float)MAX_PRODUCTSLOT));
        currentPage = Mathf.Clamp(currentPage, 1, maxPage);

        // 상품 슬롯 4개 생성
        for (int i=0; i<MAX_PRODUCTSLOT; i++)
        {
            GameObject newProductSlot = Instantiate(productSlotPrefab, listParent);
            productSlotList.Add(newProductSlot);
        }

        UpdateProductList();
    }

    // 상품 리스트 갱신
    public void UpdateProductList()
    {
        // 범위 설정
        int startIndex = (currentPage - 1) * 4;
        int endIndex = startIndex + MAX_PRODUCTSLOT;
        int slotIndex = 0;

        // 4가지 아이템 슬롯을 생성 및 리스트 저장
        for (int i = startIndex; i < endIndex; i++)
        {
            if (i >= productList.Count || productList[i] == null) break;

            productSlotList[slotIndex].GetComponent<ProductSlot>().SlotUpdate(productList[i]);
            productSlotList[slotIndex].SetActive(true);
            slotIndex++;
        }

        // 빈 상품 슬롯 비활성화
        for(int i=slotIndex; i < MAX_PRODUCTSLOT; i++)
        {
            productSlotList[i].SetActive(false);
        }

        // 상품 페이지 갱신
        pageTxt.text = currentPage.ToString() + "/" + maxPage.ToString();
    }

    // 구매할 수 있는지 체크
    public bool TryPurchase(int totalPrice)
    {
        return (totalPrice <= userMoney);
    }

    // 구매하기
    public void Purchase(Item item, int purchaseCount)
    {
        // ItemSlot에서 가져온 리스트 구매
        item.count -= purchaseCount;
        userMoney -= item.price * purchaseCount;
        userMoneyTxt.text = userMoney.ToString();

        // 유저의 인벤토리에 itme 추가

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
