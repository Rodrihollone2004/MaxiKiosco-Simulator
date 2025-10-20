using TMPro;
using UnityEngine;

public class PlayerEconomy : MonoBehaviour
{
    [Header("Player Money")]
    [SerializeField] private int currentMoney = 100;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private int currentChange = 0;

    [Header("Feedback UI")]
    [SerializeField] private GameObject feedbackPrefab;
    [SerializeField] private Transform feedbackParent;

    private void Awake()
    {
        MoneyBill.onPickBill += MoneyBill_onPickBill;
    }

    public void ShowFeedback(int amount)
    {
        if (feedbackPrefab == null || feedbackParent == null) return;

        GameObject feedback = Instantiate(feedbackPrefab, feedbackParent);
        feedback.transform.localPosition = Vector3.zero;

        string prefix = amount >= 0 ? "+" : "";
        Color color = amount >= 0 ? Color.green : Color.red;

        feedback.GetComponent<MoneyFeedbackText>().Setup($"{prefix}{amount}", color);
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
        ShowFeedback(amount);
        currentChange = 0;
    }

    public bool TryPurchaseUpgrade(Upgrade upgrade)
    {
        if (upgrade == null) return false;

        if (currentMoney >= upgrade.Price)
        {
            currentMoney -= upgrade.Price;
            moneyText.text = $"{currentMoney}";
            ShowFeedback(-upgrade.Price);
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
            ShowFeedback(-product.PackPrice);
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