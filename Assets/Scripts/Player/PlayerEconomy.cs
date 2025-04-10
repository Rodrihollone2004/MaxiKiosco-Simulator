using System.Collections;
using System.Collections.Generic;
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
        Debug.Log($"Recibiendo ${amount} del cliente");
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
}
