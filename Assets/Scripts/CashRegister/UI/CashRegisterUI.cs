using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CashRegisterUI : MonoBehaviour
{
    [SerializeField] private TMP_Text cartText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private TMP_Text payText;
    [SerializeField] private TMP_Text simulatedPayText;
    [SerializeField] private TMP_Text changeClientText;
    [SerializeField] private TMP_Text changeGivenText;
    [SerializeField] private List<GameObject> changeUI;
    int totalToPay;
    int simulatedTotal;
    int change;
    List<CartItem> cart;

    [Header("Founded Products")]
    string cartFoundedInfo;
    string priceFoundedInfo;
    string amountFoundedInfo;

    [Header("Not Founded Products")]
    string cartNotFoundedInfo = "";

    [Header("Expensive Products")]
    string cartExpensiveInfo = "";

    [Header("Upgrades Earned")]
    string cartUpgradeText = "";

    [SerializeField] private Transform contentParent;

    private void Start()
    {
        foreach (Transform child in contentParent)
            child.gameObject.SetActive(false);
    }

    public void UpdatePaymentText(Client client, List<int> clientPayment, int playerGivenChange, NPC_Controller nPC)
    {
        if (client == null) return;

        if (nPC.isInCashRegister)
        {

            if (!nPC.isPaying)
            {
                totalToPay = client.totalCart;
                simulatedTotal = clientPayment.Sum();
                change = simulatedTotal - totalToPay;
                cart = client.GetCart();

                int index = 0;

                //productos encontrados
                foreach (var item in cart)
                {
                    ProductInteractable product = item.Product;
                    int amount = item.Amount;

                    contentParent.GetChild(index).gameObject.SetActive(true);

                    cartFoundedInfo += $"{product.ProductData.Name}\n";
                    priceFoundedInfo += $"${item.PurchasePrice}\n";
                    amountFoundedInfo += $"x{amount}\n";

                    index++;
                }

                //productos no encontrados
                if (client.NotFoundedProducts.Count > 0)
                    foreach (ProductInteractable notFoundProduct in client.NotFoundedProducts)
                    {
                        cartNotFoundedInfo += $"{notFoundProduct.ProductData.Name}\n";
                    }

                //productos mayores al 80%
                if (client.ExpensiveProducts.Count > 0)
                    foreach (ProductInteractable notFoundProduct in client.ExpensiveProducts)
                    {
                        cartExpensiveInfo += $"{notFoundProduct.ProductData.Name}\n";
                    }

                if(client.UpgradesEarned.Count > 0)
                    foreach (Upgrade upgradeEarned in client.UpgradesEarned)
                    {
                        cartUpgradeText += $"{upgradeEarned.Name} - ${upgradeEarned.ValueForUpgrade}\n";
                    }
            }



            if (client.paymentMethod == Client.PaymentMethod.QR)
            {
                payText.text = $"${totalToPay}\n";

                cartText.text = cartFoundedInfo;
                priceText.text = priceFoundedInfo;
                amountText.text = amountFoundedInfo;

                CheckNotFoundProducts();
                CheckExpensiveProducts();
                CheckUpgradeEarnedMoney();

                foreach (GameObject texts in changeUI)
                    texts.SetActive(false);
            }
            else
            {
                foreach (GameObject texts in changeUI)
                    texts.SetActive(true);

                payText.text =
                    $"${totalToPay}\n";

                simulatedPayText.text =
                    $"${simulatedTotal}\n";

                changeClientText.text =
                    $"${change}";

                changeGivenText.text =
                    $"${playerGivenChange}";

                cartText.text = $"{cartFoundedInfo}";
                priceText.text = $"{priceFoundedInfo}";
                amountText.text = $"{amountFoundedInfo}";

                CheckNotFoundProducts();
                CheckExpensiveProducts();
                CheckUpgradeEarnedMoney();
            }
        }
    }

    private void CheckNotFoundProducts()
    {
        if (!string.IsNullOrEmpty(cartNotFoundedInfo))
        {
            cartText.text += $"<color=#808080><s>{cartNotFoundedInfo}</s></color>";
        }
    }

    private void CheckExpensiveProducts()
    {
        if (!string.IsNullOrEmpty(cartExpensiveInfo))
        {
            cartText.text += $"<color=#FF0000>{cartExpensiveInfo}</color>";
        }
    }

    private void CheckUpgradeEarnedMoney()
    {
        if (!string.IsNullOrEmpty(cartUpgradeText))
        {
            cartText.text += $"<color=#00FF00>{cartUpgradeText}</color>";
        }
    }

    public void ClearText()
    {
        payText.text = "";
        simulatedPayText.text = "";
        changeClientText.text = "";
        changeGivenText.text = "";
        cartText.text = "";
        priceText.text = "";
        amountText.text = "";
        cartFoundedInfo = "";
        cartNotFoundedInfo = "";
        priceFoundedInfo = "";
        amountFoundedInfo = "";
        cartExpensiveInfo = "";
        cartUpgradeText = "";

        foreach (Transform child in contentParent)
            child.gameObject.SetActive(false);
    }
}
