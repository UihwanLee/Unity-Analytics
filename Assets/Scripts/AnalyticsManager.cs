using Unity.Services.Core;
using Unity.Services.Analytics;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance;

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
        await UnityServices.InitializeAsync(); // Analytics SDK UCG SDK 초기화
        AnalyticsService.Instance.StartDataCollection();
    }

    public void OnPlayerLevelUp(int playerLevel)
    {
        // 이벤트 생성 및 파라미터 채우기
        var evt = new PlayerLevelUpEvent
        {
            playerLevel = playerLevel,
        };

        // Analytics 서버에 전송
        AnalyticsService.Instance.RecordEvent(evt);

        Debug.Log("PlayerLevelUp event send to Analytics!");
    }
}
