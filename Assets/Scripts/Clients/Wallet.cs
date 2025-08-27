using System;
using System.Collections.Generic;
using UnityEngine;

public class Wallet
{
    public List<int> Payment = new List<int>();

    public List<Money> Bills { get; private set; }
    public int TotalMoney { get; private set; } = 0;
    public int Step { get; private set; } = 0;

    public Wallet()
    {
        Bills = new List<Money>();
        TotalMoney = 0;
        Step = 0;

        for (int i = Money.billDenominations.Length - 1; i >= 0; i--)
        {
            int amount = UnityEngine.Random.Range(1, 10);
            int hasBill = UnityEngine.Random.Range(1, 2);

            if (amount > 0 && hasBill > 0)
                Bills.Add(new Money(Money.billDenominations[i], amount));
        }

        foreach (Money bill in Bills)
            TotalMoney += bill.Total;
    }

    public override string ToString()
    {
        string text = string.Empty;

        foreach (Money bill in Bills)
            text += bill.ToString() + "\n";
        text += "Total: " + TotalMoney + "\n";

        return text;
    }

    public List<int> TryMakePayment(int cost)
    {
        Console.WriteLine("\n\n");
        List<int> allBills = new List<int>();

        for (int i = 0; i < Bills.Count; i++)
            for (int j = 0; j < Bills[i].Amount; j++)
                allBills.Add(Bills[i].Value);

        if (allBills.Count == 0)
            return null; // No tiene plata

        allBills.Reverse();

        List<int> bestCombination = new List<int>();
        List<int> minBills = new List<int>();

        bool found = TryCalculatePayment(cost, 0, allBills, 0, new List<int>(), bestCombination, minBills);

        if (found)
            return bestCombination; // Cantidad exacta

        return minBills; // Necesita vuelto
    }

    private bool TryCalculatePayment(int cost, int current, List<int> allBills, int index,
                                     List<int> currentBills, List<int> bestCombination, List<int> minBills)
    {
        Step++;

        Debug.Log("\n\nStep: " + Step + " || Indice: " + index + " || Costo: " + cost);

        if (current == cost)
        {
            bestCombination.Clear();
            bestCombination.AddRange(currentBills);
            return true;
        }

        if (current > cost)
        {
            if (minBills.Count == 0 || currentBills.Count < minBills.Count)
            {
                minBills.Clear();
                minBills.AddRange(currentBills);
            }

            return false;
        }

        if (index >= allBills.Count)
            return false;

        int currentBillValue = allBills[index];

        currentBills.Add(currentBillValue);
        if (TryCalculatePayment(cost, current + currentBillValue, allBills, index + 1, currentBills, bestCombination, minBills))
            return true;

        currentBills.RemoveAt(currentBills.Count - 1);

        int nextIndex = index + 1;
        while (nextIndex < allBills.Count && allBills[nextIndex] == currentBillValue)
            nextIndex++;

        return TryCalculatePayment(cost, current, allBills, nextIndex, currentBills, bestCombination, minBills);
    }
}
public class Money
{
    static public int[] billDenominations { get; private set; } = { 1000, 500, 100, 50, 20, 10, 1 };

    public int Amount;
    public int Value;
    public int Total;

    public Money (int billDenominations, int amount)
    {
        Amount = amount;
        Value = billDenominations;

        Total = Value * Amount;
    }
}