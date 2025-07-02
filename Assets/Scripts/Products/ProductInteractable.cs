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
    }

    public void Interact()
    {
        amountText.text = $"Restantes: {_productData.CurrentAmount}";
        amountHintUI.SetActive(true);
        Debug.Log($"Interactuando con {_productData.Name} (${_productData.Price})");
        StartCoroutine(HideSummaryAfterDelay(3));
    }

    private IEnumerator HideSummaryAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        amountHintUI.SetActive(false);
    }

    public void Highlight()
    {
        if (!_renderer) return;
        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor("_Color", _highlightColor);
        _propBlock.SetFloat("_Scale", _highlightWidth);
        _renderer.SetPropertyBlock(_propBlock);
    }

    public void Unhighlight()
    {
        if (!_renderer) return;
        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetFloat("_Scale", 0f);
        _renderer.SetPropertyBlock(_propBlock);
    }
}
