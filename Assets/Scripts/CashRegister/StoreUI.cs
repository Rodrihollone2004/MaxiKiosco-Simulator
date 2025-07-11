using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StoreUI : MonoBehaviour
{
    [SerializeField] private List<Product> levelUpdate;
    [SerializeField] private ProductDataBase database;
    [SerializeField] private PlayerEconomy playerEconomy;
    [SerializeField] private Transform productButtonContainer;
    [SerializeField] private GameObject productButtonPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private LayerMask productLayer;

    void Start()
    {
        PopulateStore();
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

    void PopulateStore()
    {
        foreach (ProductCategory category in database.categories)
        {
            foreach (Product product in category.products)
            {
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

    public void UpdateProducts()
    {
        foreach (Product product in levelUpdate)
        {
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