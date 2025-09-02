using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreUI : MonoBehaviour
{
    [SerializeField] private List<LevelUpdates> levelUpdate;
    [SerializeField] private ProductDataBase database;
    [SerializeField] private PlayerEconomy playerEconomy;
    [SerializeField] private GameObject categoriesButtonPrefab;
    [SerializeField] private Transform categoriesButtonsContainer;
    [SerializeField] private Transform productButtonContainer;
    [SerializeField] private GameObject productButtonPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private LayerMask productLayer;
    [SerializeField] private Sprite pressButton;
    private Sprite normalButton;
    private Dictionary<productType, Button> categoriesButtons = new Dictionary<productType, Button>();

    [SerializeField] private DayNightCycle dayNightCycle;
    [SerializeField] private Stock stock;

    public bool updateProducts;

    private Dictionary<Product, GameObject> productsButtons = new Dictionary<Product, GameObject>();

    public ProductDataBase Database { get => database; set => database = value; }

    public static List<ProductInteractable> productsInWorld = new List<ProductInteractable>();

    private void Awake()
    {
        CategoriesButtons();
        ButtonType(productType.Chocolates);   
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

    private Vector3 GetSpawnPosition()
    {
        return spawnPoint.position;
    }

    private void SpawnProduct(Product capturedProduct)
    {
        GameObject spawned = Instantiate(capturedProduct.Prefab, GetSpawnPosition(), Quaternion.identity);

        SetLayerRecursive(spawned, LayerMaskToLayer(productLayer));

        if (!spawned.TryGetComponent<Rigidbody>(out _))
            spawned.AddComponent<Rigidbody>();

        if (!spawned.TryGetComponent<Collider>(out _))
            spawned.AddComponent<BoxCollider>();

        GameObject children = spawned.transform.GetChild(0).gameObject;

        ProductInteractable interactable = children.GetComponent<ProductInteractable>();
        interactable.Initialize(capturedProduct);
        productsInWorld.Add(interactable);
        stock.PopulateStore();

        foreach (StockController controllers in Stock.allStock)
            controllers.AddDeposit(interactable);
    }

    private void SetLayerRecursive(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursive(child.gameObject, layer);
        }
    }

    private int LayerMaskToLayer(LayerMask layerMask)
    {
        int layer = 0;
        int value = layerMask.value;
        while (value > 1)
        {
            value >>= 1;
            layer++;
        }
        return layer;
    }

    public void UpdateProducts()
    {
        foreach (LevelUpdates updateProducts in levelUpdate)
        {
            if (updateProducts.numberUpdate == dayNightCycle.DayNumber)
            {
                this.updateProducts = true;

                foreach (Product product in updateProducts.products)
                {
                    UpdateDataBase(product);
                    GameObject buttonGO = Instantiate(productButtonPrefab, productButtonContainer);
                    TMP_Text text = buttonGO.GetComponentInChildren<TMP_Text>();
                    text.text = $"{product.Name} - ${product.PackPrice}";

                    Product capturedProduct = product;
                    if (!productsButtons.ContainsKey(capturedProduct))
                        productsButtons.Add(capturedProduct, buttonGO);

                    buttonGO.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
                    {
                        bool purchased = playerEconomy.TryPurchase(capturedProduct);
                        if (purchased && capturedProduct.Prefab != null)
                        {
                            SpawnProduct(capturedProduct);
                        }
                    });
                }
            }
        }
    }

    private void UpdateDataBase(Product product)
    {
        foreach (ProductCategory category in database.categories)
            if (category.Type == product.Type)
            {
                if (!category.products.Contains(product))
                    category.products.Add(product);
            }
    }
}

[System.Serializable]
public class LevelUpdates
{
    public int numberUpdate;
    public List<Product> products;
}