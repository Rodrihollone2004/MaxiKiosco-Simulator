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

    public void UpdateCanvasClient(List<ProductInteractable> productsNotFound, List<ProductInteractable> productsExpensive, float percentage)
    {
        if (productsNotFound.Count == 0 && productsExpensive.Count == 0 && percentage <= 40f)
        {
            gameObject.SetActive(true);

            noProducts.text += $"<sprite name=Client_1>\n";
        }
    }

    public void UpdateTrashIcon(float trashPercentage)
    {
        if (trashPercentage <= 40)
            noProducts.text += $"<sprite name=Basura_1>\n";
        else if (trashPercentage >= 50)
            noProducts.text += $"<sprite name=Basura_2>\n";
        else if (trashPercentage >= 70)
            noProducts.text += $"<sprite name=Basura_3>\n";
    }

    public void ClearText()
    {
        gameObject.SetActive(false);
        noProducts.text = "";
    }
}
