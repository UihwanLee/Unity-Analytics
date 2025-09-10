using Unity.Services.Core;
using Unity.Services.Analytics;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance;
    private bool _isInitalized = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();
            _isInitalized = true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Analytics 초기화 실패: {e.Message}");
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SendTransaction();
        }
    }

    public void NextLevel(int currentLevel)
    {
        if(!_isInitalized)
        {
            return;
        }

        CustomEvent myEvent = new CustomEvent("next_level")
        {
            {"level_index", currentLevel }
        };
        AnalyticsService.Instance.RecordEvent(myEvent);
        AnalyticsService.Instance.Flush();
        Debug.Log("Send NextLevel");
    }

    public void SendTransaction()
    {
        TransactionEvent transaction = new TransactionEvent
        {
            TransactionId = "100000576198248",
            TransactionName = "IAP - A Large Treasure Chest",
            TransactionType = TransactionType.PURCHASE,
            TransactionServer = TransactionServer.APPLE,
            TransactionReceipt = "ewok9Ja81............991KS=="
        };

        transaction.ReceivedItems.Add(new TransactionItem
        {
            ItemName = "Golden Battle Axe",
            ItemType = "Weapon",
            ItemAmount = 1
        });
        transaction.ReceivedItems.Add(new TransactionItem
        {
            ItemName = "Flaming Sword",
            ItemType = "Weapon",
            ItemAmount = 1
        });
        transaction.ReceivedItems.Add(new TransactionItem
        {
            ItemName = "Jewel Encrusted Shield",
            ItemType = "Armour",
            ItemAmount = 1
        });
        transaction.ReceivedVirtualCurrencies.Add(new TransactionVirtualCurrency
        {
            VirtualCurrencyName = "Gold",
            VirtualCurrencyType = VirtualCurrencyType.PREMIUM,
            VirtualCurrencyAmount = 100
        });

        transaction.SpentRealCurrency = new TransactionRealCurrency
        {
            RealCurrencyType = "USD",
            RealCurrencyAmount = AnalyticsService.Instance.ConvertCurrencyToMinorUnits("USD", 4.99)
        };

        AnalyticsService.Instance.RecordEvent(transaction);
        AnalyticsService.Instance.Flush();
        Debug.Log("Send Transaction");
    }

    public void RestartGame()
    {
        AnalyticsService.Instance.RecordEvent("restart_game");
        AnalyticsService.Instance.Flush();
    }
}
