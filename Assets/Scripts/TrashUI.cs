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
    }
}
