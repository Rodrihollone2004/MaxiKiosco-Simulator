using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Client : MonoBehaviour
{
    public NPC_Controller NpcController { get; set; }
    private List<CartItem> cart = new List<CartItem>();
    private Wallet wallet;
    private CanvasClientManager canvasClientManager;
    private List<ProductInteractable> foundedProducts;
    List<ProductInteractable> allProducts;
    List<ProductInteractable> productsInWorld;

    public List<int> ClientPayment = new List<int>();
    public int totalCart;
    public static int MaxProductsToBuy { get; set; } = 6;
    public static int MaxAmountToBuy { get; set; } = 4;

    public enum PaymentMethod { Cash, QR }
    public PaymentMethod paymentMethod;

    public CanvasClientManager CanvasClientManager { get => canvasClientManager; set => canvasClientManager = value; }

    public List<ProductInteractable> NotFoundedProducts { get; private set; }
    public List<ProductInteractable> ExpensiveProducts { get; private set; }
    public List<Upgrade> UpgradesEarned { get; private set; }

    [SerializeField] ProductDataBase dataBase;

    private void Awake()
    {
        allProducts = new List<ProductInteractable>();
        productsInWorld = new List<ProductInteractable>();
        wallet = new Wallet();
        foundedProducts = new List<ProductInteractable>();
        NotFoundedProducts = new List<ProductInteractable>();
        ExpensiveProducts = new List<ProductInteractable>();
        UpgradesEarned = new List<Upgrade>();
        canvasClientManager = GetComponentInChildren<CanvasClientManager>();
    }
    public int GetProductsCount()
    {
        int totalProducts = 0;
        foreach (var item in cart)
        {
            totalProducts += item.Amount;
        }
        return totalProducts;
    }

    public void CalculateCost()
    {
        totalCart = CalculateCartTotal();
        if (wallet.TotalMoney < totalCart)
        {
            Debug.LogWarning("Cliente no tiene suficiente dinero.");
            return;
        }
        ClientPayment = wallet.TryMakePayment(totalCart);
        paymentMethod = (PaymentMethod)Random.Range(0, 2);
    }

    public void CheckUpgradesEarn()
    {
        UpgradesEarned.Clear();

        UpgradeInteractable[] upgradeInWorld = FindObjectsOfType<UpgradeInteractable>();

        foreach (UpgradeInteractable upgrade in upgradeInWorld)
        {
            if (upgrade.IsPlaced && upgrade.UpgradeData.IsUpgradeChange && upgrade.UpgradeData.ValueForUpgrade > 0)
            {
                float random = Random.value;
                if (random > 0.5f)
                {
                    upgrade.UpgradeMoneyEarned(upgrade.UpgradeData.ValueForUpgrade);
                    UpgradesEarned.Add(upgrade.UpgradeData);
                }
            }
        }
    }

    public void AddRandomProductsToCart()
    {
        foundedProducts.Clear();
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

                foundedProducts.Add(product);

                int productPriceLimit = (int) (productInWorld.ProductData.OriginalPrice * 1.8f) + 1;

                if (productInWorld.ProductData.Price > productPriceLimit)
                {
                    ExpensiveProducts.Add(productInWorld);
                    continue;
                }

                if (amountProduct > productInWorld.CurrentAmountProduct)
                    amountProduct = productInWorld.CurrentAmountProduct;

                newTotal = total += (productInWorld.ProductData.Price * amountProduct);

                if (newTotal < wallet.TotalMoney)
                {
                    cart.Add(new CartItem(productInWorld, amountProduct, productInWorld.ProductData.Price));
                    productInWorld.SubtractAmount(amountProduct);
                    productInWorld.CheckDelete();
                    total = newTotal;
                }
            }
        }

        NotFoundedProducts = chosenProducts.Except(foundedProducts).ToList();
    }

    public int CalculateCartTotal()
    {
        int total = 0;
        foreach (var item in cart)
        {
            ProductInteractable product = item.Product;
            int amount = item.Amount;

            total += product.ProductData.Price * amount;
        }
        return total;
    }

    public List<CartItem> GetCart()
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
    }
}

[System.Serializable]
public class CartItem
{
    public ProductInteractable Product;
    public int Amount;
    public int PurchasePrice;

    public CartItem(ProductInteractable product, int amount, int purchasePrice)
    {
        Product = product;
        Amount = amount;
        PurchasePrice = purchasePrice;
    }
}
