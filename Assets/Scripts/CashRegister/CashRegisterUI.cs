using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CashRegisterUI : MonoBehaviour
{
    [SerializeField] private TMP_Text payText;
    int totalToPay;
    int simulatedTotal;
    int change;
    List<ProductInteractable> cart;
    string cartInfo;


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
                
                foreach (ProductInteractable product in cart)
                {
                    if (!cartInfo.Contains(product.ProductData.Name))
                        cartInfo += $"- {product.ProductData.Name} (${product.ProductData.Price})\n";
                }
            }



            if (client.paymentMethod == Client.PaymentMethod.QR)
            {
                payText.text =
                $"Total: ${totalToPay}\n" +
                $"{cartInfo}";
            }
            else
            {
                payText.text =
                    $"Total: ${totalToPay}\n" +
                    $"Pago: ${simulatedTotal}\n" +
                    $"Vuelto esperado: ${change}\n" +
                    $"Vuelto entregado: ${playerGivenChange}\n\n" +
                    $"{cartInfo}";
            }
        }
    }

    public void ClearText()
    {
        payText.text = "";
    }
}
