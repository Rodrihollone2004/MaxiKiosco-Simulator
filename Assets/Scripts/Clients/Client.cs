using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Client : MonoBehaviour
{
    public NPC_Controller NpcController { get; set; }
    private Dictionary<ProductInteractable, int> cart = new Dictionary<ProductInteractable, int>();
    private Wallet wallet;
    private CanvasClientManager canvasClientManager;
    private List<ProductInteractable> matchedProducts;
    List<ProductInteractable> allProducts;
    List<ProductInteractable> productsInWorld;

    public List<int> ClientPayment = new List<int>();
    public int totalCart;
    public int MaxProductsToBuy { get; set; } = 6;
    public int MaxAmountToBuy { get; set; } = 4;

    public enum PaymentMethod { Cash, QR }
    public PaymentMethod paymentMethod;

    public CanvasClientManager CanvasClientManager { get => canvasClientManager; set => canvasClientManager = value; }

    [SerializeField] ProductDataBase dataBase;

    private void Awake()
    {
        allProducts = new List<ProductInteractable>();
        productsInWorld = new List<ProductInteractable>();
        wallet = new Wallet();
        matchedProducts = new List<ProductInteractable>();
        canvasClientManager = GetComponentInChildren<CanvasClientManager>();
    }

    public void CalculateCost()
    {
        totalCart = CalculateCartTotal();
        ClientPayment = wallet.TryMakePayment(totalCart);

        paymentMethod = (PaymentMethod)Random.Range(0, 2);
        Debug.Log($"Este cliente paga con: {paymentMethod}");
    }

    public void AddRandomProductsToCart()
    {
        matchedProducts.Clear();
        allProducts.Clear();
        productsInWorld.Clear();

        productsInWorld = FindObjectsOfType<ProductInteractable>().ToList();

        foreach (ProductCategory updateProduct in dataBase.categories)
            foreach (Product product in updateProduct.products)
            {
                allProducts.Add(product.Prefab.GetComponentInChildren<ProductInteractable>(true));
            }

        List<ProductInteractable> availableProducts = new List<ProductInteractable>(allProducts);
        availableProducts = availableProducts.OrderBy(x => Random.value).ToList(); // Mezclar productos para que agarre random

        int total = 0;
        int newTotal = 0;

        wallet.RandomWallet();
        cart.Clear();
        PrintWallet();

        int productsToBuy = Random.Range(3, MaxProductsToBuy); //variable que va a variar con mejoras

        List<ProductInteractable> chosenProducts = availableProducts.Take(productsToBuy).ToList();

        foreach (ProductInteractable product in chosenProducts)
        {
            foreach (ProductInteractable productInWorld in productsInWorld)
            {
                int amountProduct = Random.Range(1, MaxAmountToBuy); //variable que va a variar con mejoras

                if (product.ProductData != productInWorld.ProductData)
                    continue;

                matchedProducts.Add(product);

                if (amountProduct > productInWorld.CurrentAmountProduct)
                    amountProduct = productInWorld.CurrentAmountProduct;

                newTotal = total + (productInWorld.ProductData.Price * amountProduct);

                if (newTotal < wallet.TotalMoney)
                {
                    cart.Add(productInWorld, amountProduct);
                    productInWorld.SubtractAmount(amountProduct);
                    productInWorld.CheckDelete();
                    total = newTotal;
                    Debug.Log($"Añadido al carrito: {productInWorld.ProductData.Name} (${productInWorld.ProductData.Price})");
                }
            }
        }

        List<ProductInteractable> notMatchedProducts = chosenProducts.Except(matchedProducts).ToList();
        canvasClientManager.UpdateCanvasClient(notMatchedProducts, this, matchedProducts);

        Debug.Log($"Total del carrito: ${CalculateCartTotal()} / Disponible: ${wallet.TotalMoney}");
    }

    public int CalculateCartTotal()
    {
        int total = 0;
        foreach (KeyValuePair<ProductInteractable, int> item in cart)
        {
            ProductInteractable product = item.Key;
            int amount = item.Value;

            total += product.ProductData.Price * amount;
        }
        return total;
    }

    public Dictionary<ProductInteractable, int> GetCart()
    {
        return cart;
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

        walletInfo = walletInfo.TrimEnd(',', ' ');
        walletInfo += $". Total: ${wallet.TotalMoney}";
        Debug.Log(walletInfo);
    }
}
