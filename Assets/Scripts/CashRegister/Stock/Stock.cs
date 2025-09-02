using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Stock : MonoBehaviour
{
    List<ProductInteractable> productsInWorld;
    [SerializeField] private ProductDataBase database;
    [SerializeField] private GameObject stockItemPrefab;
    [SerializeField] private Transform itemsStockContainer;
    [SerializeField] private GameObject categoriesButtonPrefab;
    [SerializeField] private Transform categoriesButtonsContainer;
    [SerializeField] private Sprite pressButton;
    private Sprite normalButton;
    private Dictionary<productType, Button> categoriesButtons = new Dictionary<productType, Button>();
    private Dictionary<Product, GameObject> productsButtons = new Dictionary<Product, GameObject>();

    public static List<StockController> allStock = new List<StockController>();

    private void Start()
    {
        CategoriesButtons();
        ButtonType(productType.Chocolates);
    }

    public void PopulateStore()
    {
        productsInWorld = StoreUI.productsInWorld;

        if (productsInWorld.Count > 0)
        {
            foreach (ProductInteractable product in productsInWorld)
            {
                if (productsButtons.ContainsKey(product.ProductData))
                    continue;

                GameObject stockGO = Instantiate(stockItemPrefab, itemsStockContainer);
                TMP_Text nameProduct = stockGO.GetComponentInChildren<TMP_Text>();
                nameProduct.text = $"{product.ProductData.Name}";

                StockController controller = stockGO.GetComponent<StockController>();
                controller.Product = product.ProductData;
                allStock.Add(controller);

                productsButtons.Add(product.ProductData, stockGO);
            }
        }
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
            if (button == buttons.Key)
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
