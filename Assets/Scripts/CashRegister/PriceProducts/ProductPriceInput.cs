using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProductPriceInput : MonoBehaviour
{
    [SerializeField] private TMP_Text priceText;
    private Product _productData;
    private TMP_InputField _inputField;

    public void Initialize(Product data)
    {
        _productData = data;
        UpdatePriceText();
        //_inputField = GetComponent<TMP_InputField>();
        //_inputField.onEndEdit.AddListener(UpdatePriceFromInput);
    }

    private void UpdatePriceText()
    {
        priceText.text = $"{_productData.Price}";
    }

    public void AddPrice(int amount)
    {
        _productData.Price += amount;
        UpdatePriceText(); 
    }

    public void SubtractPrice(int amount)
    {
        _productData.Price -= amount;
        UpdatePriceText();
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