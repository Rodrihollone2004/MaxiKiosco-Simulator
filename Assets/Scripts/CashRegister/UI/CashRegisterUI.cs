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
    [SerializeField] private TMP_Text changeText;
    [SerializeField] private TMP_Text changeTitle;
    [SerializeField] private GameObject changeUI;
    int totalToPay;
    int simulatedTotal;
    int change;
    Dictionary<ProductInteractable, int> cart;
    string cartInfo;
    string priceInfo;
    string amountInfo;

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
                totalToPay = client.CalculateCartTotal();
                simulatedTotal = clientPayment.Sum();
                change = simulatedTotal - totalToPay;
                cart = client.GetCart();

                int index = 0;

                foreach (KeyValuePair<ProductInteractable, int> item in cart)
                {
                    ProductInteractable product = item.Key;
                    int amount = item.Value;

                    contentParent.GetChild(index).gameObject.SetActive(true);

                    cartInfo += $"{product.ProductData.Name}\n";
                    priceInfo += $"${product.ProductData.Price}\n";
                    amountInfo += $"x{amount}\n";

                    index++;
                }
            }



            if (client.paymentMethod == Client.PaymentMethod.QR)
            {
                payText.text =
                $"${totalToPay}\n";
                cartText.text = $"{cartInfo}";
                priceText.text = $"{priceInfo}";
                amountText.text = $"{amountInfo}";
                changeUI.SetActive(false);
                changeTitle.text = "";
            }
            else
            {
                changeUI.SetActive(true);

                payText.text =
                    $"${totalToPay}\n" +
                    $"${simulatedTotal}\n";
                changeText.text =
                    $"${change} / " +
                    $"${playerGivenChange}\n\n";

                cartText.text = $"{cartInfo}";
                priceText.text = $"{priceInfo}";
                amountText.text = $"{amountInfo}";
                changeTitle.text = "Vuelto: ";
            }
        }
    }

    public void ClearText()
    {
        payText.text = "";
        changeText.text = "";
        cartText.text = "";
        priceText.text = "";
        amountText.text = "";
        cartInfo = "";
        priceInfo = "";
        amountInfo = "";

        foreach (Transform child in contentParent)
            child.gameObject.SetActive(false);
    }
}
