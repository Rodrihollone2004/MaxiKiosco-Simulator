using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    [SerializeField] private int[] billDenominations = { 1000, 500, 100, 50, 20, 10 };
    [SerializeField] private int minTotalMoney = 500;
    [SerializeField] private int maxTotalMoney = 10000;

    private Dictionary<int, int> wallet = new Dictionary<int, int>();

    public int[] BillDenominations => billDenominations;
    public Dictionary<int, int> WalletData => wallet;

    protected virtual void Start()
    {
        InitializeWallet();
        GenerateRandomMoney();
    }

    private void InitializeWallet()
    {
        foreach (int denomination in billDenominations)
        {
            wallet[denomination] = 0;
        }
    }

    private void GenerateRandomMoney()
    {
        float totalMoney = Random.Range(minTotalMoney, maxTotalMoney + 1);
        float remaining = totalMoney;

        foreach (int bill in billDenominations)
        {
            if (remaining <= 0) break;
            int maxPossible = Mathf.FloorToInt(remaining / bill);
            if (maxPossible > 0)
            {
                int count = Random.Range(1, maxPossible + 1);
                wallet[bill] = count;
                remaining -= count * bill;
            }
        }
    }

    public bool CanAfford(float amount)
    {
        return GetTotalMoney() >= amount;
    }

    public float GetTotalMoney()
    {
        float total = 0;
        foreach (KeyValuePair<int, int> kvp in wallet)
        {
            total += kvp.Key * kvp.Value;
        }
        return total;
    }

    public void Initialize()
    {
        InitializeWallet();
        GenerateRandomMoney();
    }

    public void CompletePurchase(Dictionary<int, int> paymentUsed)
    {
        foreach (KeyValuePair<int, int> kvp in paymentUsed)
        {
            wallet[kvp.Key] -= kvp.Value;
        }
    }
}
