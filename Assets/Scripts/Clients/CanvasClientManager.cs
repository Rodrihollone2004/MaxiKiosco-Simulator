using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CanvasClientManager : MonoBehaviour
{
    [SerializeField] private TMP_Text noProducts;

    private void Start()
    {
        gameObject.SetActive(false);
        noProducts.text = "";
    }

    public void UpdateCanvasClient(List<ProductInteractable> productsNotFound, List<ProductInteractable> productsExpensive, List<ProductInteractable> productsFound)
    {
        if (productsNotFound.Count > 0)
        {
            gameObject.SetActive(true);

            foreach (ProductInteractable notProduct in productsNotFound)
            {
                noProducts.text += $"{notProduct.ProductData.Name} <sprite name=CruzRoja>\n";
            }
        }
    }

    public void ClearText()
    {
        gameObject.SetActive(false);
        noProducts.text = "";
    }
}
