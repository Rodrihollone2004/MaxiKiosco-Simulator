using System.Collections;
using TMPro;
using UnityEngine;

public class ProductInteractable : MonoBehaviour, IInteractable
{
    public static ProductInteractable currentHintOwner;

    [SerializeField] private Product _productData;
    [SerializeField] private float _highlightWidth = 1.03f;
    [SerializeField] private GameObject amountHintUI;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private TMP_Text priceText;
    public Product ProductData => _productData;

    [SerializeField] int currentAmountProduct;

    public bool CanBePickedUp => false;

    public void Initialize(Product productData)
    {
        _productData = productData;
        currentAmountProduct = _productData.CurrentAmount;
    }

    public void Interact()
    {
        if (currentHintOwner != null && currentHintOwner != this)
        {
            currentHintOwner.amountHintUI.SetActive(false);
            currentHintOwner.StopAllCoroutines();
        }

        amountText.text = $"Restantes: {currentAmountProduct}";
        priceText.text = $"Precio: {_productData.Price}";
        amountHintUI.SetActive(true);
        Debug.Log($"Interactuando con {_productData.Name} (${_productData.Price})");

        if (currentHintOwner == null)
            StartCoroutine(HideSummaryAfterDelay(3));

        currentHintOwner = this;
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

    private IEnumerator HideSummaryAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        amountHintUI.SetActive(false);
        currentHintOwner = null;

    }

    public void Highlight()
    {
    }

    public void Unhighlight()
    {
    }
}
