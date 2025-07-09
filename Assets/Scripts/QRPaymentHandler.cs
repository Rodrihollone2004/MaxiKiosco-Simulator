using UnityEngine;
using TMPro;

public class QRPaymentHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField amountInput;
    [SerializeField] private CashRegisterInteraction cashRegisterInteraction;
    [SerializeField] private GameObject qrPaymentUI;
    [SerializeField] private PlayerCam playerCam;

    private Client currentClient;

    public void SetupQRPayment(Client client)
    {
        currentClient = client;
        qrPaymentUI.SetActive(true);
    }

    public void ConfirmQRPayment()
    {
        if (currentClient != null && int.TryParse(amountInput.text, out int amount))
        {
            if (amount == currentClient.totalCart)
            {
                cashRegisterInteraction.ProcessQRPayment(currentClient, amount);
                CancelQRPayment();
                amountInput.text = "";
            }
            else
            {
                Debug.Log("El monto ingresado no coincide con el total");
            }
        }
    }

    public void CancelQRPayment()
    {
        qrPaymentUI.SetActive(false);
    }
}
