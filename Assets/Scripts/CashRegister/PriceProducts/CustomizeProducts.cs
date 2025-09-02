using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CustomizeProducts : MonoBehaviour
{
    List<ProductInteractable> productsPlaced;
    [SerializeField] GameObject inputPricePrefab;
    [SerializeField] RectTransform contentInputPrice;
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
        productsPlaced = ProductPlaceManager.productsPlaced;

        if (productsPlaced.Count > 0)
            foreach (ProductInteractable product in productsPlaced)
            {
                if (productsButtons.ContainsKey(product.ProductData) || !product.IsPlaced)
                    continue;

                GameObject inputGO = Instantiate(inputPricePrefab, contentInputPrice);
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
