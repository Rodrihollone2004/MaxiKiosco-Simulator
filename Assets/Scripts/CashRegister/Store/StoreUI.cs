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

    [SerializeField] private GameObject newProductsUI;

    [SerializeField] private GameObject cartButtonPrefab;
    [SerializeField] private Transform cartButtonContainer;

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private LayerMask productLayer;
    [SerializeField] private Sprite pressButton;
    private Sprite normalButton;
    private Dictionary<productType, Button> categoriesButtons = new Dictionary<productType, Button>();

    [SerializeField] private DayNightCycle dayNightCycle;

    public bool updateProducts;

    private Dictionary<Product, GameObject> productsButtons = new Dictionary<Product, GameObject>();
    private List<Product> productsToBuy = new List<Product>();
    private List<GameObject> cartPrefabs = new List<GameObject>();

    public ProductDataBase Database { get => database; set => database = value; }

    public static List<ProductInteractable> productsInWorld = new List<ProductInteractable>();
    public static List<StockController> allStock = new List<StockController>();

    [SerializeField] private TMP_Text cartTotalText;
    private int currentCartTotal = 0;

    public productType CurrentType { get; private set; }

    private void Awake()
    {
        CategoriesButtons();
        ButtonType(productType.Chocolates);
        CheckButtonPressed(productType.Chocolates);
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
        CurrentType = buttonType;
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

    public void SpawnProduct()
    {
        if (productsToBuy.Count > 0)
        {
            int totalCost = 0;
            foreach (Product p in productsToBuy)
                totalCost += p.PackPrice;

            if (!playerEconomy.HasEnoughMoney(totalCost))
            {
                Debug.Log("No tenes suficiente dinero para comprar todo.");
                return;
            }

            playerEconomy.DeductMoney(totalCost);
            playerEconomy.ShowFeedback(-totalCost);

            if (TutorialContent.Instance.CurrentIndexGuide < 11)
                return;

            if (!TutorialContent.Instance.IsComplete)
                TutorialContent.Instance.IsComplete = true;

            foreach (Product capturedProduct in productsToBuy)
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

                foreach (StockController controllers in allStock)
                    controllers.AddDeposit(interactable);

                TutorialContent.Instance.CompleteStep(11);
            }
            ClearCart();
        }
        else
            Debug.Log("Nada carrito");
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

                    //imagen Stock
                    Image imageStock = buttonGO.GetComponentInChildren<Image>();
                    TMP_Text textStock = imageStock.GetComponentInChildren<TMP_Text>();
                    textStock.text = product.Name;

                    //imagen boton de compra
                    Button button = buttonGO.GetComponentInChildren<Button>();
                    TMP_Text textButton = button.GetComponentInChildren<TMP_Text>();
                    textButton.text = $"X{product.PackSize} - ${product.PackPrice}";

                    Product capturedProduct = product;
                    if (!productsButtons.ContainsKey(capturedProduct))
                        productsButtons.Add(capturedProduct, buttonGO);

                    button.onClick.AddListener(() =>
                    {
                        if (TutorialContent.Instance.CurrentIndexGuide < 11)
                            return;

                        StockController controller = buttonGO.GetComponentInChildren<StockController>();
                        controller.Product = product;

                        if (!allStock.Contains(controller))
                            allStock.Add(controller);

                        productsToBuy.Add(capturedProduct);
                        CreateCartProduct(capturedProduct, controller);
                        UpdateCartTotal();

                    });

                }
            }
        }
    }

    private void UpdateCartTotal()
    {
        currentCartTotal = 0;
        foreach (Product p in productsToBuy)
        {
            currentCartTotal += p.PackPrice;
        }

        cartTotalText.text = $"Total: ${currentCartTotal}";
    }


    public void DeleteCart(GameObject cartProduct, Product product, StockController controller)
    {
        productsToBuy.Remove(product);
        cartPrefabs.Remove(cartProduct);
        Destroy(cartProduct);
        UpdateCartTotal();
    }

    private void ClearCart()
    {
        foreach (GameObject cart in cartPrefabs)
            Destroy(cart);

        cartPrefabs.Clear();
        productsToBuy.Clear();
        UpdateCartTotal();
    }

    private void CreateCartProduct(Product product, StockController controller)
    {
        GameObject buttonGO = Instantiate(cartButtonPrefab, cartButtonContainer);

        TMP_Text textStock = buttonGO.GetComponentInChildren<TMP_Text>();
        textStock.text = $"{product.Name}\n ${product.PackPrice}";

        Button button = buttonGO.GetComponentInChildren<Button>();

        cartPrefabs.Add(buttonGO);

        button.onClick.AddListener(() =>
        {
            DeleteCart(buttonGO, product, controller);
        });
    }

    public void CheckUpdate()
    {
        foreach (LevelUpdates updateProducts in levelUpdate)
            if (updateProducts.numberUpdate == dayNightCycle.DayNumber)
                dayNightCycle.StartCoroutine(dayNightCycle.NewProducts());
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