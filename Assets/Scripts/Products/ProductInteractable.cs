using System.Collections;
using TMPro;
using UnityEngine;

public class ProductInteractable : MonoBehaviour, IInteractable
{
    public static ProductInteractable currentHintOwner;

    [SerializeField] private Product _productData;
    [SerializeField] int currentAmountProduct;

    public Product ProductData => _productData;
    public bool CanBePickedUp => false;

    public void Initialize(Product productData)
    {
        _productData = productData;
        currentAmountProduct = _productData.CurrentAmount;
    }

    public void Interact()
    {
        ProductUIManager.Instance.ShowInfo(_productData.Name, _productData.Price, currentAmountProduct);
    }

    public void SubtractAmount()
    {
        currentAmountProduct--;
    }

    public void CheckDelete()
    {
        if (currentAmountProduct <= 0)
            Destroy(gameObject);
    }

    public void Highlight()
    {
    }

    public void Unhighlight()
    {
    }
}
