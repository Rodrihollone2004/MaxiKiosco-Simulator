using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CashRegisterUI : MonoBehaviour
{
    [SerializeField] private TMP_Text payText;

    public void UpdatePaymentText(Client client, List<int> clientPayment, int playerGivenChange, NPC_Controller nPC)
    {
        if (client == null) return;

        if (nPC.isInCashRegister)
        {
            int totalToPay = client.CalculateCartTotal();
            int simulatedTotal = clientPayment.Sum();
            int change = simulatedTotal - totalToPay;

            List<ProductInteractable> cart = client.GetCart();
            string cartInfo = "Carrito:\n";
            foreach (var product in cart)
            {
                cartInfo += $"- {product.ProductData.Name} (${product.ProductData.Price})\n";
            }

            payText.text =
                $"Total: ${totalToPay}\n" +
                $"Pago: ${simulatedTotal}\n" +
                $"Vuelto esperado: ${change}\n" +
                $"Vuelto entregado: ${playerGivenChange}\n\n" +
                $"{cartInfo}";
        }
    }

    public void ClearText()
    {
        payText.text = "";
    }
}
