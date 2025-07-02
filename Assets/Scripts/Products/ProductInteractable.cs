using System.Collections;
using TMPro;
using UnityEngine;

public class ProductInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private Product _productData;
    [SerializeField] private Color _highlightColor = Color.red;
    [SerializeField] private float _highlightWidth = 1.03f;
    [SerializeField] private GameObject amountHintUI;
    [SerializeField] private TMP_Text amountText;
    public Product ProductData => _productData;

    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;
    [SerializeField] int currentAmountProduct;
    [SerializeField] int currentPrice; //para ajustar el precio de cada producto

    public bool CanBePickedUp => false;

    public GameObject AmountHintUI { get => amountHintUI; set => amountHintUI = value; }

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _propBlock = new MaterialPropertyBlock();
    }

    public void Initialize(Product productData)
    {
        _productData = productData;
        currentAmountProduct = _productData.CurrentAmount;
        currentPrice = _productData.Price;
    }

    public void Interact()
    {
        amountText.text = $"Restantes: {currentAmountProduct}";
        amountHintUI.SetActive(true);
        Debug.Log($"Interactuando con {_productData.Name} (${_productData.Price})");
        StartCoroutine(HideSummaryAfterDelay(3));
    }

    public void SubtractAmount()
    {
        currentAmountProduct--;
    }

    private IEnumerator HideSummaryAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        amountHintUI.SetActive(false);
    }

    public void Highlight()
    {
    }

    public void Unhighlight()
    {
    }
}
