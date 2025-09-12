using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Insatnce;

    private int MAX_PRODUCTSLOT = 4;    // 상품 리스트 최대 개수

    [SerializeField] private int category;  // 상점 카테고리
    [SerializeField] private Transform listParent;  // 상품 리스트 위치
    [SerializeField] private GameObject productSlotPrefab; // 상품 슬롯 프리팹
    [SerializeField] private List<GameObject> productSlotList = new List<GameObject>(); // 상품 리스트
    [SerializeField] private List<Item> productList = new List<Item>(); // 상품 리스트

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

    // 상품 리스트 초기화
    public void InitProductList()
    {
        // Resource/Items 폴더 안에 있는 아이템 정보를 불러와 리스트화
        Item[] loadItems = Resources.LoadAll<Item>("Items");
        productList = new List<Item>(loadItems);

        // 4가지 아이템 슬롯을 생성 및 리스트 저장
        for (int i=0; i< productList.Count; i++)
        {
            if (i == MAX_PRODUCTSLOT) break;

            GameObject newProductSlot = Instantiate(productSlotPrefab, listParent);
            newProductSlot.GetComponent<ProductSlot>().SlotUpdate(productList[i]);
            productSlotList.Add(newProductSlot);
        }
    }

    // 상품 리스트 갱신
    public void UpdateProductList()
    {
        
    }

    // 구매하기
    public void Purchase(Item item)
    {
        // ItemSlot에서 가져온 리스트 구매
    }
}
