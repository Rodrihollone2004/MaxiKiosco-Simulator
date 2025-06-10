using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Client : MonoBehaviour
{
    public NPC_Controller NpcController {get; set;}
    private List<ProductInteractable> cart = new List<ProductInteractable>();
    private Wallet wallet;
    List<ProductInteractable> allProducts;

    public List<int> ClientPayment = new List<int>();
    public int totalCart;

    private void Awake() 
    {
        allProducts = new List<ProductInteractable>();
        wallet = new Wallet(); 
    }

    private void Start()
    {
        PrintWallet();
    }

    public void CalculateCost()
    {
        totalCart = CalculateCartTotal();
        ClientPayment = TryMakePayment(totalCart);
    }

    public void AddRandomProductsToCart()
    {
        allProducts.Clear();

        ProductInteractable[] productsInWorld = FindObjectsOfType<ProductInteractable>();

        foreach (ProductInteractable product in productsInWorld) { allProducts.Add(product); }

        List<ProductInteractable> availableProducts = new List<ProductInteractable>(allProducts);
        availableProducts = availableProducts.OrderBy(x => UnityEngine.Random.value).ToList(); // Mezclar productos para que agarre random

        int total = 0;
        int newTotal = 0;

        cart.Clear();
        wallet.GenerateRandomMoney();
        PrintWallet();

        foreach (ProductInteractable product in availableProducts)
        {
            newTotal = total + product.ProductData.Price;

            if (wallet.CanAfford(newTotal))
            {
                cart.Add(product);
                total = newTotal;
                Debug.Log($"Añadido al carrito: {product.ProductData.Name} (${product.ProductData.Price})");
            }
        }

        Debug.Log($"Total del carrito: ${CalculateCartTotal()} / Disponible: ${wallet.GetTotalMoney()}");
    }

    public int CalculateCartTotal()
    {
        int total = 0;
        foreach (ProductInteractable product in cart)
        {
            total += product.ProductData.Price;
        }
        return total;
    }

    public List<ProductInteractable> GetCart()
    {
        return cart;
    }

    public List<int> TryMakePayment(int cost)
    {
        List<int> allBills = new List<int>();

        foreach (var Bill in wallet.WalletData)
            for (int i = 0; i < Bill.Value; i++)
                allBills.Add(Bill.Key);

        if (allBills.Count == 0)
            return null; // No tiene plata

        allBills.Sort();

        List<int> bestCombination = new List<int>();
        List<int> minBills = new List<int>();

        bool found = TryCalculatePayment(cost, 0, allBills, 0, new List<int>(), bestCombination, minBills);

        if (found)
            return bestCombination; // Cantidad exacta

        return minBills; // Necesita vuelto
    }

    public bool TryCalculatePayment(int cost, int current, List<int> allBills, int index, List<int> currentBills, List<int> bestCombination, List<int> minBills)
    {
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

        currentBills.Add(allBills[index]);

        if (TryCalculatePayment(cost, currentBills.Sum(), allBills, index + 1, currentBills, bestCombination, minBills))
            return true;

        currentBills.RemoveAt(currentBills.Count - 1);

        if (TryCalculatePayment(cost, current, allBills, index + 1, currentBills, bestCombination, minBills))
            return true;
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
        foreach (KeyValuePair<int, int> kvp in wallet.WalletData)
        {
            walletInfo += $"{kvp.Value}x${kvp.Key}, ";
        }

        walletInfo = walletInfo.TrimEnd(',', ' ');
        walletInfo += $". Total: ${wallet.GetTotalMoney()}";
        Debug.Log(walletInfo);
    }
}
