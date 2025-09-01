using TMPro;
using UnityEngine;

public class StockController : MonoBehaviour
{
    public Product Product { get; set; }

    [SerializeField] TMP_Text placedAmount;
    [SerializeField] TMP_Text depositAmount;

    private int amountPlaced;
    private int amountDeposit;


    public void AddDeposit(ProductInteractable currentProduct)
    {
        if (Product == currentProduct.ProductData)
        {
            amountDeposit += Product.CurrentAmount;
            UpdateTextsStock();
        }
    }

    public void PlaceProduct(ProductInteractable currentProduct)
    {
        if (Product == currentProduct.ProductData)
            if (!currentProduct.WasPlaced && currentProduct.IsPlaced)
            {
                amountDeposit -= currentProduct.CurrentAmountProduct;
                amountPlaced += currentProduct.CurrentAmountProduct;
                UpdateTextsStock();
            }
    }

    public void DepositProduct(ProductInteractable currentProduct)
    {
        if (Product == currentProduct.ProductData)
            if (currentProduct.WasPlaced && !currentProduct.IsPlaced)
            {
                amountPlaced -= currentProduct.CurrentAmountProduct;
                amountDeposit += currentProduct.CurrentAmountProduct;
                UpdateTextsStock();
            }
    }

    public void UpdateTextsStock()
    {
        placedAmount.text = $"{amountPlaced}";
        depositAmount.text = $"{amountDeposit}";
    }
}