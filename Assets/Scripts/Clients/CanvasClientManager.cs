using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CanvasClientManager : MonoBehaviour
{
    [SerializeField] private TMP_Text emojiClient;

    public void UpdateClientEmoji(List<ProductInteractable> productsNotFound, List<ProductInteractable> productsExpensive, float trashPercentage, ClientQueueManager queueManager)
    {
        queueManager.TotalFaces++;

        if (productsNotFound.Count == 0 && productsExpensive.Count == 0 && trashPercentage <= 40f)
        {
            emojiClient.text = $"<sprite name=Client_1>\n";
            queueManager.CountOfGoodFaces++;
        }
        else if (productsNotFound.Count > 1 && productsExpensive.Count > 1 && trashPercentage >= 70)
        {
            emojiClient.text = $"<sprite name=Client_3>\n";
            queueManager.CountOfBadFaces++;
        }
        else
        {
            emojiClient.text = $"<sprite name=Client_2>\n";
            queueManager.CountOfIntermediateFaces++;
        }
    }

    public void UpdateTrashIcon(float trashPercentage)
    {
        gameObject.SetActive(true);

        if (emojiClient == null)
        {
            emojiClient = GetComponentInChildren<TMP_Text>(true);
            emojiClient.text = ""; 
        }

        if (trashPercentage >= 50)
            emojiClient.text = $"<sprite name=Basura_2>\n";
        else if (trashPercentage >= 70)
            emojiClient.text = $"<sprite name=Basura_3>\n";
    }

    public void ClearText()
    {
        gameObject.SetActive(false);
        emojiClient.text = "";
    }
}
