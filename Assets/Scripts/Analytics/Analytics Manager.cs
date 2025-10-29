using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance;
    private bool isInitialized = false;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
        isInitialized = true;
    }

    public void ClientsServed(int clientsAmount)
    {
        if (!isInitialized)
            return;

        CustomEvent clientsEvent = new CustomEvent("Clients_Served")
        {
            {"clients_variable", clientsAmount}
        };

        AnalyticsService.Instance.RecordEvent(clientsEvent);
        AnalyticsService.Instance.Flush();
        Debug.Log("Client served");
    }

    public void Trash70()
    {
        AnalyticsService.Instance.RecordEvent("trash_60%");
        AnalyticsService.Instance.Flush();
    }

    public void DayFive()
    {
        AnalyticsService.Instance.RecordEvent("Day_Five");
        AnalyticsService.Instance.Flush();
    }

    public void ChangeColor()
    {
        AnalyticsService.Instance.RecordEvent("Change_Color");
        AnalyticsService.Instance.Flush();
    }

    public void TheftPrevented()
    {
        AnalyticsService.Instance.RecordEvent("Intercept_Thief");
        AnalyticsService.Instance.Flush();
    }

    public void CompletedRobbery()
    {
        AnalyticsService.Instance.RecordEvent("Thief_Success");
        AnalyticsService.Instance.Flush();
    }

    public void ProductsPlaced(int products)
    {
        if (!isInitialized)
            return;

        CustomEvent productsEvent = new CustomEvent("Products_Placed")
        {
            {"products_amount", products}
        };

        AnalyticsService.Instance.RecordEvent(productsEvent);
        AnalyticsService.Instance.Flush();
    }

    public void LimitQueueClient()
    {
        AnalyticsService.Instance.RecordEvent("Queue_Limit");
        AnalyticsService.Instance.Flush();
    }
}
