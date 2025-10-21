using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CashRegisterUI : MonoBehaviour
{
    [SerializeField] private TMP_Text cartText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private TMP_Text payText;
    [SerializeField] private TMP_Text changeText;
    [SerializeField] private GameObject changeUI;
    int totalToPay;
    int simulatedTotal;
    int change;
    Dictionary<ProductInteractable, int> cart;
    string cartInfo;
    string priceInfo;
    string amountInfo;


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
                cartInfo = "Carrito:\n";
                priceInfo = "Precio c/u:\n";
                amountInfo = "Cantidad:\n";

                foreach (KeyValuePair<ProductInteractable, int> item in cart)
                {
                    ProductInteractable product = item.Key;
                    int amount = item.Value;

                    cartInfo += $"- {product.ProductData.Name}\n";
                    priceInfo += $"${product.ProductData.Price}\n";
                    amountInfo += $"x{amount}\n";
                }
            }



            if (client.paymentMethod == Client.PaymentMethod.QR)
            {
                payText.text =
                $"Total: ${totalToPay}\n";
                cartText.text = $"{cartInfo}";
                priceText.text = $"{priceInfo}";
                amountText.text = $"{amountInfo}";
                changeUI.SetActive(false);

            }
            else
            {
                changeUI.SetActive(true);

                payText.text =
                    $"Total: ${totalToPay}\n" +
                    $"Pago con ${simulatedTotal}\n";
                changeText.text =
                    $"Vuelto esperado: ${change}\n" +
                    $"Vuelto entregado: ${playerGivenChange}\n\n";

                cartText.text = $"{cartInfo}";
                priceText.text = $"{priceInfo}";
                amountText.text = $"{amountInfo}";
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
    }
}
