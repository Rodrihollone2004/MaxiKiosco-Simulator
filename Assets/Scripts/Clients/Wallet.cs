using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallet
{
    static public int[] billDenominations { get; private set; } = { 1000, 500, 100, 50, 20, 10, 1 };
    private int minTotalMoney = 1500;
    private int maxTotalMoney = 10000;

    private Dictionary<int, int> wallet = new Dictionary<int, int>();

    public int[] BillDenominations => billDenominations;
    public Dictionary<int, int> WalletData => wallet;

    public Wallet()
    {
        InitializeWallet();
        GenerateRandomMoney();
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

    public void CompletePurchase(Dictionary<int, int> paymentUsed)
    {
        foreach (KeyValuePair<int, int> kvp in paymentUsed)
        {
            wallet[kvp.Key] -= kvp.Value;
        }
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
        int totalMoney = 0;
        do
        {
            totalMoney = 0;
            foreach (int bill in billDenominations)
            {
                int count = Random.Range(0, 3);
                wallet[bill] = count;
                totalMoney += count * bill;
            }
        } while (totalMoney < minTotalMoney);
    }
}
