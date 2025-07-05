using TMPro;
using UnityEngine;

public class CustomizeProducts : MonoBehaviour
{
    ProductInteractable[] productsInWorld;
    [SerializeField] GameObject inputFieldPrefab;
    [SerializeField] RectTransform contentInputField;

    public void PopulateStore()
    {
        productsInWorld = FindObjectsOfType<ProductInteractable>();

        if (productsInWorld.Length > 0)
            foreach (ProductInteractable product in productsInWorld)
            {
                GameObject buttonGO = Instantiate(inputFieldPrefab, contentInputField);
                TMP_InputField text = buttonGO.GetComponent<TMP_InputField>();
                TMP_Text nameProduct = buttonGO.GetComponentInChildren<TMP_Text>();
                text.text = $"{product.ProductData.Price}";
                nameProduct.text = $"{product.ProductData.Name}";
            }
        else
            Debug.Log("No hay productos en el mostrador");
    }
}
/*
public class CustomTMP_InputField :TMP_InputField
{
    override void 
}

*/