using System.Collections;
using UnityEngine;

public class ClientTrashSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] trashPrefabs;
    [SerializeField] private Transform[] spawnAreas;
    [SerializeField] private float minSpawnTime = 10f;
    [SerializeField] private float maxSpawnTime = 30f;
    [SerializeField] private int maxTrash = 2;

    private int currentTrashCount = 0;
    private Coroutine spawnCoroutine;
    private bool isSpawningActive = true;

    private void Start()
    {
        StartSpawning();
    }

    private IEnumerator SpawnTrashRoutine()
    {
        while (isSpawningActive && currentTrashCount < maxTrash)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));

            if (isSpawningActive && currentTrashCount < maxTrash)
                SpawnTrash();
            else
                yield return new WaitForSeconds(10f);
        }
    }

    private void SpawnTrash()
    {
        if (trashPrefabs.Length == 0 || spawnAreas.Length == 0) return;

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
        currentTrashCount--;

        if (!IsSpawning() && currentTrashCount < maxTrash)
        {
            StartSpawning();
        }
    }

    public void StartSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
        isSpawningActive = true;
        spawnCoroutine = StartCoroutine(SpawnTrashRoutine());
    }

    public void StopSpawning()
    {
        isSpawningActive = false;
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
    }

    public bool IsSpawning()
    {
        return isSpawningActive;
    }
}
