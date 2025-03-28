using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour
{
    [Header("Configuracion de Billetes")]
    [SerializeField] private int[] billDenominations = { 1000, 500, 100, 50, 20, 10 };
    [SerializeField] private int minTotalMoney = 500;
    [SerializeField] private int maxTotalMoney = 10000;

    private Dictionary<int, int> wallet = new Dictionary<int, int>();
    private float totalMoney;
    private List<Product> cart = new List<Product>();
    private Product[] allProducts;

    private void Awake()
    {
        allProducts = FindObjectsOfType<Product>();     // no se porque se ve asi
    }

    private void Start()
    {
        InitializeWallet();
        GenerateRandomMoney();
        LogWalletInfo();
        AddRandomProductsToCart();
    }

    // para probar que funciona metodo. metodo para agregar productos random
    private void AddRandomProductsToCart()
    {
        int productCount = Random.Range(1, 4);

        for (int i = 0; i < productCount; i++)
        {
            Product randomProduct = allProducts[Random.Range(0, allProducts.Length)];
            cart.Add(randomProduct);
            Debug.Log("Productos" + randomProduct);
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
        totalMoney = Random.Range(minTotalMoney, maxTotalMoney + 1);
        float remaining = totalMoney;

        for (int i = 0; i < billDenominations.Length; i++)
        {
            int bill = billDenominations[i];
            if (remaining <= 0) break;

            int maxPossible = Mathf.FloorToInt(remaining / bill);
            if (maxPossible > 0)
            {
                int count = Random.Range(1, maxPossible + 1);
                wallet[bill] = count;
                remaining -= count * bill;
            }
        }

        if (GetTotalMoney() == 0)
        {
            int randomBill = billDenominations[Random.Range(0, billDenominations.Length)];
            wallet[randomBill] = 1;
        }
    }

    public bool CanAfford(float amount)
    {
        return GetTotalMoney() >= amount;
    }

    public PaymentResult TryMakePayment(float amount)
    {
        if (!CanAfford(amount))
        {
            return new PaymentResult(false, 0f, null);
        }

        Dictionary<int, int> payment = new Dictionary<int, int>();
        List<int> denominations = new List<int>(billDenominations);
        denominations.Sort((a, b) => b.CompareTo(a));

        if (TryCalculatePayment(amount, denominations, 0, payment, GetTotalMoney()))
        {
            float paidAmount = CalculatePaymentTotal(payment);
            return new PaymentResult(true, paidAmount - amount, payment);
        }

        return new PaymentResult(false, 0f, null);
    }

    private bool TryCalculatePayment(float remaining, List<int> denominations, int index, Dictionary<int, int> payment, float currentTotal)
    {
        if (remaining <= 0) return true;

        if (index >= denominations.Count || remaining > currentTotal) return false;

        int currentBill = denominations[index];
        int available = wallet[currentBill];
        int maxPossible = Mathf.Min((int)(remaining / currentBill), available);

        for (int i = maxPossible; i >= 0; i--)
        {
            float newRemaining = remaining - (i * currentBill);

            if (i > 0) payment[currentBill] = i;

            if (newRemaining < 0 && (-newRemaining) < (remaining - newRemaining))
            {
                return true;
            }

            if (TryCalculatePayment(newRemaining, denominations, index + 1, payment, currentTotal - (i * currentBill)))
            {
                return true;
            }

            if (i > 0) payment.Remove(currentBill);
        }

        return false;
    }

    public void AddToCart(Product product)
    {
        cart.Add(product);
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

    public void CompletePurchase(Dictionary<int, int> payment)
    {
        foreach (var kvp in payment)
        {
            wallet[kvp.Key] -= kvp.Value;
        }
        cart.Clear();
    }

    private float GetTotalMoney()
    {
        float total = 0f;
        foreach (var kvp in wallet)
        {
            total += kvp.Key * kvp.Value;
        }
        return total;
    }

    private float CalculatePaymentTotal(Dictionary<int, int> payment)
    {
        float total = 0f;
        foreach (var kvp in payment)
        {
            total += kvp.Key * kvp.Value;
        }
        return total;
    }

    public Dictionary<int, int> GetPaymentDetails(float amountToPay)
    {
        Dictionary<int, int> payment = new Dictionary<int, int>();

        if (TryCalculatePayment(amountToPay, new List<int>(billDenominations), 0, payment, GetTotalMoney()))
        {
            return payment;
        }

        return new Dictionary<int, int>();
    }

    private void LogWalletInfo()
    {
        string info = $"Cliente tiene ${GetTotalMoney():F2} en total. Billetes: ";
        foreach (var kvp in wallet)
        {
            if (kvp.Value > 0)
            {
                info += $"{kvp.Value}x ${kvp.Key}, ";
            }
        }
        Debug.Log(info.TrimEnd(',', ' '));
    }

    public struct PaymentResult
    {
        public bool success;
        public float change;
        public Dictionary<int, int> paymentUsed;

        public PaymentResult(bool success, float change, Dictionary<int, int> paymentUsed)
        {
            this.success = success;
            this.change = change;
            this.paymentUsed = paymentUsed;
        }
    }
}
