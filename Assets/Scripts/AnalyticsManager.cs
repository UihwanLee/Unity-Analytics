using Unity.Services.Core;
using Unity.Services.Analytics;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UnityServices.InitializeAsync(); // Analytics SDK UCG SDK √ ±‚»≠
        AnalyticsService.Instance.StartDataCollection();
    }
}
