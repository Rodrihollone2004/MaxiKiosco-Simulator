using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour
{
    private List<Product> cart = new List<Product>();
    private Wallet wallet;

    private void Awake()
    {
        wallet = GetComponent<Wallet>();
    }
    private void Start()
    {
        wallet.Initialize();
        AddRandomProductsToCart();
        PrintWallet();
    }

    private void AddRandomProductsToCart()
    {
        Product[] allProducts = FindObjectsOfType<Product>();
        int productCount = Random.Range(1, 4);

        for (int i = 0; i < productCount; i++)
        {
            Product randomProduct = allProducts[Random.Range(0, allProducts.Length)];
            cart.Add(randomProduct);
        }
    }

    public float CalculateCartTotal()
    {
        float total = 0;
        foreach (var product in cart)
        {
            total += product.Price;
        }
        return total;
    }

    public PaymentResult TryMakePayment(float amount)
    {
        if (!wallet.CanAfford(amount))
        {
            return new PaymentResult(false, 0f, null);
        }

        Dictionary<int, int> paymentUsed = new Dictionary<int, int>();
        List<int> denominations = new List<int>(wallet.BillDenominations);
        denominations.Sort((a, b) => b.CompareTo(a));

        if (TryCalculatePayment(amount, denominations, 0, paymentUsed))
        {
            return new PaymentResult(true, amount, paymentUsed);
        }

        return new PaymentResult(false, 0f, null);
    }

    private bool TryCalculatePayment(float remaining, List<int> denominations, int index, Dictionary<int, int> payment)
    {
        if (remaining <= 0) return true;
        if (index >= denominations.Count) return false;

        int currentBill = denominations[index];
        int available = wallet.WalletData[currentBill];
        int maxPossible = Mathf.Min((int)(remaining / currentBill), available);

        for (int i = maxPossible; i >= 0; i--)
        {
            float newRemaining = remaining - (i * currentBill);
            if (i > 0) payment[currentBill] = i;

            if (newRemaining < 0 && (-newRemaining) < (remaining - newRemaining))
            {
                return true;
            }

            if (TryCalculatePayment(newRemaining, denominations, index + 1, payment))
            {
                return true;
            }

            if (i > 0) payment.Remove(currentBill);
        }

        return false;
    }
    public class PaymentResult
    {
        public bool Success { get; }
        public float AmountPaid { get; }
        public Dictionary<int, int> BillsUsed { get; }

        public PaymentResult(bool success, float amountPaid, Dictionary<int, int> billsUsed)
        {
            Success = success;
            AmountPaid = amountPaid;
            BillsUsed = billsUsed ?? new Dictionary<int, int>();
        }
    }

    private void PrintWallet()
    {
        string walletInfo = $"Cliente {name} tiene: ";
        foreach (var kvp in wallet.WalletData)
        {
            walletInfo += $"{kvp.Value}x${kvp.Key}, ";
        }

        walletInfo = walletInfo.TrimEnd(',', ' ');
        walletInfo += $". Total: ${wallet.GetTotalMoney()}";
        Debug.Log(walletInfo);
    }
}
