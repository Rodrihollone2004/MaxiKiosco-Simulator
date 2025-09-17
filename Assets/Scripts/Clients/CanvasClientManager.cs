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

    public void UpdateCanvasClient(List<ProductInteractable> productsNotFound, Client currentClient, List<ProductInteractable> productsFound)
    {
        if (productsNotFound.Count > 0 && !currentClient.IsThief)
        {
            gameObject.SetActive(true);

            foreach (ProductInteractable notProduct in productsNotFound)
            {
                noProducts.text += $"{notProduct.ProductData.Name}\n";
            }
        }
        else if (currentClient.IsThief)
        {
            gameObject.SetActive(true);

            noProducts.text = $"IS THIEF\n";

            if (productsFound.Count > 0)
            {
                foreach (ProductInteractable productFound in productsFound)
                {
                    noProducts.text += $"{productFound.ProductData.Name}\n";
                }
            }

            return;
        }
    }

    public void ClearText()
    {
        gameObject.SetActive(false);
        noProducts.text = "";
    }
}
