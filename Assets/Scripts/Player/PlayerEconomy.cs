using UnityEngine;
using TMPro;

public class PlayerEconomy : MonoBehaviour
{
    [Header("Player Money")]
    [SerializeField] private float currentMoney = 100f;
    [SerializeField] private TMP_Text moneyText;

    public float CurrentMoney => currentMoney;

    private void Start()
    {
        moneyText.text = $"{currentMoney}";
    }

    // Sumar plata del cliente
    public void ReceivePayment(float amount)
    {
        currentMoney += amount;
        moneyText.text = $"{currentMoney}";
    }

    // Restar plata al comprar
    public bool TryPurchase(float cost)
    {
        if (currentMoney >= cost)
        {
            currentMoney -= cost;
            Debug.Log($"Compra realizada por ${cost}. Dinero restante: ${currentMoney}");
            moneyText.text = $"{currentMoney}";
            return true;
        }
        else
        {
            Debug.LogWarning("No hay suficiente dinero para realizar la compra.");
            return false;
        }
    }
    public bool TryGiveChange(float amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            moneyText.text = $"{currentMoney}";
            return true;
        }
        return false;
    }
}
