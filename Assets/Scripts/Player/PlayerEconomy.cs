using UnityEngine;
using TMPro;

public class PlayerEconomy : MonoBehaviour
{
    [Header("Player Money")]
    [SerializeField] private int currentMoney = 100;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private int currentChange = 0;

    private void Awake()
    {
        MoneyBill.onPickBill += MoneyBill_onPickBill;
    }

    public bool HasEnoughMoney(int amount)
    {
        return currentMoney >= amount;
    }

    public void DeductMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            moneyText.text = $"{currentMoney}";
        }
    }

    private void MoneyBill_onPickBill(int billValue, bool isAdding)
    {
        if (isAdding)
        {
            currentChange += billValue;
            TryGiveChange(billValue);
        }
        else
        {
            if (currentChange >= billValue)
            {
                currentChange -= billValue;
                currentMoney += billValue;
                moneyText.text = $"{currentMoney}";
            }
        }

    }

    public int GetCurrentChange()
    {
        return currentChange;
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

    public bool TryPurchaseUpgrade(Upgrade upgrade)
    {
        if (upgrade == null) return false;

        if (currentMoney >= upgrade.Price)
        {
            currentMoney -= upgrade.Price;
            moneyText.text = $"{currentMoney}";
            return true;
        }
        else
        {
            return false;
        }
    }

    // Restar plata al comprar
    public bool TryPurchase(Product product)
    {
        if (product == null) return false;

        if (currentMoney >= product.PackPrice)
        {
            currentMoney -= product.PackPrice;
            moneyText.text = $"{currentMoney}";
            return true;
        }
        else
        {
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