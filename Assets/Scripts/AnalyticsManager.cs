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
        await UnityServices.InitializeAsync(); // Analytics SDK UCG SDK �ʱ�ȭ
        AnalyticsService.Instance.StartDataCollection();
    }

    public void OnPlayerLevelUp(int playerLevel)
    {
        // �̺�Ʈ ���� �� �Ķ���� ä���
        var evt = new PlayerLevelUpEvent
        {
            playerLevel = playerLevel,
        };

        // Analytics ������ ����
        AnalyticsService.Instance.RecordEvent(evt);

        Debug.Log("PlayerLevelUp event send to Analytics!");
    }
}
