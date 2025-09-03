using System.Collections;
using TMPro;
using UnityEngine;

public class ProductInteractable : MonoBehaviour, IInteractable
{
    public static ProductInteractable currentHintOwner;

    [SerializeField] private Product _productData;
    [SerializeField] int currentAmountProduct;
    [SerializeField] private GameObject originalPrefab;

    [Header("Config")]
    [SerializeField] private bool showNameOnHighlight = true;

    public bool IsPlaced { get; set; }

    public bool WasPlaced { get; set; }

    public bool ShowNameOnHighlight => showNameOnHighlight;

    public Product ProductData => _productData;
    public bool CanBePickedUp => false;

    public int CurrentAmountProduct { get => currentAmountProduct; set => currentAmountProduct = value; }
    public GameObject OriginalPrefab => originalPrefab;

    public void Initialize(Product productData, GameObject prefab = null)
    {
        _productData = productData;
        currentAmountProduct = _productData.CurrentAmount;
        if (prefab != null) originalPrefab = prefab;
    }

    public void Interact()
    {
        ProductUIManager.Instance.ShowInfo(_productData.Name, _productData.Price, currentAmountProduct);
    }

    public void SubtractAmount(int amount)
    {
        currentAmountProduct -= amount;

        foreach (StockController controllers in Stock.allStock)
            controllers.SubtractProduct(this);
    }

    public void CheckDelete()
    {
        if (currentAmountProduct <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Highlight()
    {
    }

    public void Unhighlight()
    {
    }
}
