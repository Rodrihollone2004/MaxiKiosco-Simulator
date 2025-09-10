using UnityEngine;
using TMPro;

public class QRPaymentHandler : MonoBehaviour
{
    [SerializeField] private CashRegisterInteraction cashRegisterInteraction;
    [SerializeField] private GameObject qrPaymentUI;
    [SerializeField] private PlayerCam playerCam;
    [SerializeField] private TMP_Text qrPaymentText;

    private Client currentClient;

    public void SetupQRPayment(Client client)
    {
        currentClient = client;
        qrPaymentText.text = "";
        qrPaymentUI.SetActive(true);
    }

    public void AddNumber(int number)
    {
        qrPaymentText.text += $"{number}";
    }

    public void SubtractNumber()
    {
        if (!string.IsNullOrEmpty(qrPaymentText.text))
        {
            qrPaymentText.text = qrPaymentText.text.Remove(qrPaymentText.text.Length - 1);
        }
    }

    public void ConfirmQRPayment()
    {
        if (currentClient != null && int.TryParse(qrPaymentText.text, out int amount))
        {
            if (amount == currentClient.totalCart)
            {
                cashRegisterInteraction.ProcessQRPayment(currentClient, amount);
                CancelQRPayment();
                qrPaymentText.text = "";
                cashRegisterInteraction.PeekClient();
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
