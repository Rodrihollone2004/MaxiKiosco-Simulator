using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomizeProducts : MonoBehaviour
{
    List<ProductInteractable> productsPlaced;
    [SerializeField] GameObject inputPricePrefab;
    [SerializeField] RectTransform contentInputPrice;
    [SerializeField] private ProductDataBase database;
    [SerializeField] private GameObject categoriesButtonPrefab;
    [SerializeField] private Transform categoriesButtonsContainer;
    [SerializeField] private Sprite pressButton;
    private Sprite normalButton;
    private Dictionary<Product, GameObject> productsButtons = new Dictionary<Product, GameObject>();
    private List<ProductInteractable> toRemove = new List<ProductInteractable>();
    private Dictionary<productType, Button> categoriesButtons = new Dictionary<productType, Button>();

    private productType currentType;
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
                if (product.CurrentAmountProduct <= 0 && productsButtons.ContainsKey(product.ProductData))
                {
                    toRemove.Add(product);
                    Destroy(productsButtons[product.ProductData]);
                    productsButtons.Remove(product.ProductData);
                    continue;
                }
                else if (product == null)
                {
                    toRemove.Add(product);
                    continue;
                }

                if (productsButtons.ContainsKey(product.ProductData) || !product.IsPlaced || product.CurrentAmountProduct <= 0)
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

        foreach (ProductInteractable product in toRemove)
        {
            productsPlaced.Remove(product);
        }
        
        ButtonType(currentType);
    }

    private void CategoriesButtons()
    {
        foreach (ProductCategory category in database.categories)
        {
            GameObject buttonGO = Instantiate(categoriesButtonPrefab, categoriesButtonsContainer);
            normalButton = buttonGO.GetComponent<Image>().sprite;
            TMP_Text text = buttonGO.GetComponentInChildren<TMP_Text>();
            text.text = $"{category.Type}";

            Button button = buttonGO.GetComponent<UnityEngine.UI.Button>();
            button.onClick.AddListener(() => ButtonType(category.Type));
            button.onClick.AddListener(() => CheckButtonPressed(category.Type));

            categoriesButtons.Add(category.Type, button);
        }
    }

    public void ButtonType(productType buttonType)
    {
        currentType = buttonType;
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

    private void CheckButtonPressed(productType button)
    {
        foreach (var buttons in categoriesButtons)
        {
            if(button == buttons.Key)
            {
                buttons.Value.image.sprite = pressButton;
            }
            else
            {
                buttons.Value.image.sprite = normalButton;
            }
        }
    }
}
