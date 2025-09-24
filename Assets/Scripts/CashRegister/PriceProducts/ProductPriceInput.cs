using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProductPriceInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private TMP_Text priceText;
    private Product _productData;
    private float _originalPrice;

    [SerializeField] private float holdDelay = 0.2f; // Tiempo entre cada suma/resta
    private float holdTimer = 0f;

    private bool isHolding = false;

    private enum ActionType { None, Add, Subtract }
    private ActionType currentAction = ActionType.None;

    private int currentAmount = 0;

    public void Initialize(Product data)
    {
        _productData = data;
        _originalPrice = data.Price;
        _inputField.onEndEdit.AddListener(UpdatePriceFromInput);
        UpdatePriceText();
    }

    private void Update()
    {
        if (isHolding)
        {
            holdTimer += Time.deltaTime;

            if (holdTimer >= holdDelay)
            {
                ApplyPriceChange();
                holdTimer = 0f;
            }
        }
    }

    private void ApplyPriceChange()
    {
        switch (currentAction)
        {
            case ActionType.Add:
                _productData.Price += currentAmount;
                break;

            case ActionType.Subtract:
                if (_productData.Price - currentAmount >= 0)
                {
                    _productData.Price -= currentAmount;
                }
                else
                {
                    _productData.Price = 0;
                    isHolding = false;
                }
                break;
        }

        UpdatePriceText();
    }

    private void UpdatePriceText()
    {
        _inputField.text = $"{_productData.Price}";

        float priceIncreasePercentage = ((float)_productData.Price - _originalPrice) / _originalPrice * 100f;

        if (priceIncreasePercentage > 80f) //esto capaz después cambia si tenemos mas nivel o algo
        {
            priceText.color = Color.red;
        }
        else
        {
            priceText.color = Color.black;
        }
    }

    // Llamá estos métodos desde el EventTrigger, pasando el valor
    public void StartAddingPrice(int amount)
    {
        isHolding = true;
        currentAmount = amount;
        currentAction = ActionType.Add;
        ApplyPriceChange();
    }

    public void StartSubtractingPrice(int amount)
    {
        isHolding = true;
        currentAmount = amount;
        currentAction = ActionType.Subtract;

        if (_productData.Price - amount >= 0)
        {
            ApplyPriceChange();
        }
        else
        {
            _productData.Price = 0;
            UpdatePriceText();
            isHolding = false;
        }
    }

    public void StopButton()
    {
        isHolding = false;
        currentAction = ActionType.None;
        holdTimer = 0f;
    }

    public void UpdatePriceFromInput(string value)
    {
        if (int.TryParse(value, out int result))
        {
            _productData.Price = result;
        }
        else
        {
            Debug.LogWarning("Entrada inválida para el precio.");
            _inputField.text = _productData.Price.ToString("0.00"); // Restaurar valor anterior
        }
    }
}