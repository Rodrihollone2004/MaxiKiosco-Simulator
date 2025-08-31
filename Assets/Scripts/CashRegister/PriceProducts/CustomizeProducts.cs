using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CustomizeProducts : MonoBehaviour
{
    List<ProductInteractable> productsInWorld;
    [SerializeField] GameObject inputFieldPrefab;
    [SerializeField] RectTransform contentInputField;
    [SerializeField] private ProductDataBase database;
    [SerializeField] private GameObject categoriesButtonPrefab;
    [SerializeField] private Transform categoriesButtonsContainer;
    private Dictionary<Product, GameObject> productsButtons = new Dictionary<Product, GameObject>();

    private void Start()
    {
        CategoriesButtons();
        ButtonType(productType.Chocolates);
    }

    public void PopulateStore()
    {
        productsInWorld = ProductPlaceManager.productsInWorld;

        if (productsInWorld.Count > 0)
            foreach (ProductInteractable product in productsInWorld)
            {
                if (productsButtons.ContainsKey(product.ProductData))
                    continue;

                GameObject inputGO = Instantiate(inputFieldPrefab, contentInputField);
                TMP_Text nameProduct = inputGO.GetComponentInChildren<TMP_Text>();
                nameProduct.text = $"{product.ProductData.Name}";

                ProductPriceInput handler = inputGO.GetComponent<ProductPriceInput>();
                if (handler != null)
                    handler.Initialize(product.ProductData);

                productsButtons.Add(product.ProductData, inputGO);
            }
        else
            Debug.Log("No hay productos en el mostrador");
    }

    private void CategoriesButtons()
    {
        foreach (ProductCategory category in database.categories)
        {
            GameObject buttonGO = Instantiate(categoriesButtonPrefab, categoriesButtonsContainer);
            TMP_Text text = buttonGO.GetComponentInChildren<TMP_Text>();
            text.text = $"{category.Type}";

            buttonGO.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => ButtonType(category.Type));
        }
    }

    public void ButtonType(productType buttonType)
    {
        foreach (KeyValuePair<Product, GameObject> buttons in productsButtons)
        {
            if (buttonType == buttons.Key.Type)
            {
                buttons.Value.SetActive(true);
            }
            else
                buttons.Value.SetActive(false);
        }
    }
}
