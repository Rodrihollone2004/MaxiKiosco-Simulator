using UnityEngine;
using TMPro;
using System;

public class PlayerEconomy : MonoBehaviour
{
    [Header("Player Money")]
    [SerializeField] private float currentMoney = 100f;
    [SerializeField] private TMP_Text moneyText;
    public event Action<int> onFinishPay;
    [SerializeField] private int vuelto = 0;

    public float CurrentMoney => currentMoney;
    private void Awake()
    {
        MoneyBill.onPickBill += MoneyBill_onPickBill;
    }

    private void MoneyBill_onPickBill(int obj)
    {
        vuelto += obj;
        if (TryGiveChange(vuelto))
        {
            onFinishPay?.Invoke(vuelto);
        }
    }

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
