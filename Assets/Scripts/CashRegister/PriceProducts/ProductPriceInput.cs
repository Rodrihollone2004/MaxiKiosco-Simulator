using TMPro;
using UnityEngine;

public class ProductPriceInput : MonoBehaviour
{
    private Product _productData;
    private TMP_InputField _inputField;

    public void Initialize(Product data)
    {
        _productData = data;
        _inputField = GetComponent<TMP_InputField>();
        _inputField.onEndEdit.AddListener(UpdatePriceFromInput);
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