using TMPro;
using UnityEngine;

public class TrashUI : MonoBehaviour
{
    [SerializeField] private ClientTrashSpawner trashSpawner;
    [SerializeField] private TMP_Text trashText;

    private void OnEnable()
    {
        trashSpawner.OnTrashChanged.AddListener(UpdateTrashText);
    }

    private void OnDisable()
    {
        trashSpawner.OnTrashChanged.RemoveListener(UpdateTrashText);
    }

    private void UpdateTrashText(float percentage)
    {
        trashText.text = $"Basura: {percentage:0}%";

        if (percentage >= 70f)
        {
            trashText.color = Color.red;
            trashText.text = $"Basura: {percentage:0}% <sprite name=Advertencia>\n";
        }
        else if (percentage >= 50f)
            trashText.color = Color.yellow;
        else
            trashText.color = Color.black;
    }
}
