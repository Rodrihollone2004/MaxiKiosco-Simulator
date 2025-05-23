using UnityEngine;
using TMPro;
using System;

public class PlayerEconomy : MonoBehaviour
{
    [Header("Player Money")]
    [SerializeField] private int currentMoney = 100;
    [SerializeField] private TMP_Text moneyText;
    public event Action<int> onFinishPay;
    [SerializeField] private int currentChange = 0;

    public float CurrentMoney => currentMoney;
    private void Awake()
    {
        MoneyBill.onPickBill += MoneyBill_onPickBill;
    }

    private void MoneyBill_onPickBill(int obj)
    {
        currentChange += obj;
        if (TryGiveChange(obj))
        {
            onFinishPay?.Invoke(currentChange);
        }
    }

    private void Start()
    {
        moneyText.text = $"{currentMoney}";
    }

    // Sumar plata del cliente
    public void ReceivePayment(int amount)
    {
        currentMoney += amount;
        moneyText.text = $"{currentMoney}";
        currentChange = 0;
    }

    // Restar plata al comprar
    public bool TryPurchase(int cost)
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
    public bool TryGiveChange(int amount)
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
