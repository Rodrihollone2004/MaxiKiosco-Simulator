using System.Collections;
using UnityEngine;

public class ClientTrashSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] trashPrefabs;
    [SerializeField] private Transform[] spawnAreas;
    [SerializeField] private int maxTrash = 2;

    private int currentTrashCount = 0;

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
    }

    public void TrashCleaned()
    {
        currentTrashCount = Mathf.Max(0, currentTrashCount - 1);
    }
}
