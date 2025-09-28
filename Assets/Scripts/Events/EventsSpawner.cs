using System.Collections.Generic;
using UnityEngine;

public class EventsSpawner : MonoBehaviour
{
    [SerializeField] private GameObject cartoneroPrefab;
    [SerializeField] private DayNightCycle dayNightCycle;

    private List<GameObject> cartonerosPool = new List<GameObject>();
    private Transform spawnPoint;

    private void Start()
    {
        spawnPoint = AStarManager.instance.StartNode.transform;
    }

    public void CheckSpawnerEvent()
    {
        if (dayNightCycle.DayNumber % 2 != 0 && dayNightCycle.DayNumber > 2)
        {
            if (cartonerosPool.Count == 0)
            {
                GameObject instance = Instantiate(cartoneroPrefab, spawnPoint.position, Quaternion.identity);
                cartonerosPool.Add(instance);
            }
            else
            {
                GameObject cartonero = cartonerosPool.Find(inactive => !inactive.activeInHierarchy);
                cartonero.SetActive(true);
            }
        }
    }

}
