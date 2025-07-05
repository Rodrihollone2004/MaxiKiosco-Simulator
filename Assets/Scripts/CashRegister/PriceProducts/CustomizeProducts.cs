using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CustomizeProducts : MonoBehaviour
{
    ProductInteractable[] productsInWorld;
    [SerializeField] GameObject inputFieldPrefab;
    [SerializeField] RectTransform contentInputField;

    private HashSet<string> productsFields = new HashSet<string>();

    public void PopulateStore()
    {
        productsInWorld = FindObjectsOfType<ProductInteractable>();

        if (productsInWorld.Length > 0)
            foreach (ProductInteractable product in productsInWorld)
            {
                if (productsFields.Contains(product.name))
                    continue;

                GameObject inputGO = Instantiate(inputFieldPrefab, contentInputField);

                TMP_InputField inputField = inputGO.GetComponent<TMP_InputField>();
                inputField.caretWidth = 0;
                TMP_Text nameProduct = inputGO.GetComponentInChildren<TMP_Text>();

                ProductPriceInput handler = inputGO.GetComponent<ProductPriceInput>();
                if (handler != null)
                    handler.Initialize(product.ProductData);

                inputField.text = $"{product.ProductData.Price}";
                nameProduct.text = $"{product.ProductData.Name}";
                productsFields.Add(product.name);
            }
        else
            Debug.Log("No hay productos en el mostrador");
    }
}
