using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class StoreUI : MonoBehaviour
{
    [SerializeField] private List<LevelUpdates> levelUpdate;
    [SerializeField] private ProductDataBase database;
    [SerializeField] private PlayerEconomy playerEconomy;
    [SerializeField] private Transform productButtonContainer;
    [SerializeField] private GameObject productButtonPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private LayerMask productLayer;

    private DayNightCycle dayNightCycle;

    public List<LevelUpdates> LevelUpdate { get => levelUpdate; set => levelUpdate = value; }
    public bool updateProducts;

    private void Awake()
    {
        dayNightCycle = FindObjectOfType<DayNightCycle>();

        foreach (ProductCategory category in database.categories)
            category.products.Clear();
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