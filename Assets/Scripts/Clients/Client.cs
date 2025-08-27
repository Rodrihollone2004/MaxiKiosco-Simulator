using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Client : MonoBehaviour
{
    public NPC_Controller NpcController {get; set;}
    private Dictionary<ProductInteractable, int> cart = new Dictionary<ProductInteractable, int>();
    private Wallet wallet;
    List<ProductInteractable> allProducts;

    public List<Money> Bills { get; private set; }

    public List<int> ClientPayment = new List<int>();
    public int totalCart;

    public enum PaymentMethod { Cash, QR }
    public PaymentMethod paymentMethod;

    private void Awake() 
    {
        allProducts = new List<ProductInteractable>();
        wallet = new Wallet(); 
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
        allProducts.Clear();

        ProductInteractable[] productsInWorld = FindObjectsOfType<ProductInteractable>();

        foreach (ProductInteractable product in productsInWorld) { allProducts.Add(product); }

        List<ProductInteractable> availableProducts = new List<ProductInteractable>(allProducts);
        availableProducts = availableProducts.OrderBy(x => Random.value).ToList(); // Mezclar productos para que agarre random

        int total = 0;
        int newTotal = 0;

        cart.Clear();
        PrintWallet();

        int productsToBuy = Random.Range(3, 6); // 6 porque el max es exclusivo

        List<ProductInteractable> chosenProducts = availableProducts.Take(productsToBuy).ToList();

        foreach (ProductInteractable product in chosenProducts)
        {

            int amountProduct = Random.Range (1, 4); //el maximo despues va a ser variable para las mejoras

            if(amountProduct > product.CurrentAmountProduct)
                amountProduct = product.CurrentAmountProduct;

            newTotal = total + (product.ProductData.Price * amountProduct);

            if (newTotal < wallet.TotalMoney)
            {
                cart.Add(product, amountProduct);
                product.SubtractAmount();
                product.CheckDelete();
                total = newTotal;
                Debug.Log($"Añadido al carrito: {product.ProductData.Name} (${product.ProductData.Price})");
            }
        }

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
