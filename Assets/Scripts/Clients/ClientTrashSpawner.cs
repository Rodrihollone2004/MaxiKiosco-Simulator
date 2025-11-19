using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ClientTrashSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] trashPrefabs;
    [SerializeField] private Transform[] spawnAreas;
    [SerializeField] private int maxTrash = 2;

    private int currentTrashCount = 0;

    [System.Serializable]
    public class TrashChangedEvent : UnityEvent<float> { }
    public TrashChangedEvent OnTrashChanged;

    [field: SerializeField] public float TrashPercentage { get; private set; } = 0;

    public void SpawnTrash()
    {
        if (currentTrashCount >= maxTrash || trashPrefabs.Length == 0 || spawnAreas.Length == 0)
            return;

        Transform spawnArea = spawnAreas[Random.Range(0, spawnAreas.Length)];
        Vector3 spawnPosition = new Vector3(
            Random.Range(spawnArea.position.x - spawnArea.localScale.x / 2, spawnArea.position.x + spawnArea.localScale.x / 2),
            spawnArea.position.y,
            Random.Range(spawnArea.position.z - spawnArea.localScale.z / 2, spawnArea.position.z + spawnArea.localScale.z / 2)
        );

        GameObject trashPrefab = trashPrefabs[Random.Range(0, trashPrefabs.Length)];
        Instantiate(trashPrefab, spawnPosition, Quaternion.identity);
        currentTrashCount++;

        //NotifyTrashChanged();
    }

    public void TrashCleaned()
    {
        currentTrashCount = Mathf.Max(0, currentTrashCount - 1);
        //NotifyTrashChanged();
    }

    private void NotifyTrashChanged()
    {
        TrashPercentage = GetTrashPercentage();
        OnTrashChanged?.Invoke(TrashPercentage);
    }

    public float GetTrashPercentage()
    {
        if (maxTrash == 0) return 0f;
        return (float)currentTrashCount / maxTrash * 100f;
    }
}
