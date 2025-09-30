using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsSpawner : MonoBehaviour
{
    [SerializeField] private GameObject cartoneroPrefab;
    [SerializeField] private GameObject thiefPrefab;
    [SerializeField] private DayNightCycle dayNightCycle;

    private List<GameObject> cartonerosPool = new List<GameObject>();
    private List<GameObject> thiefPool = new List<GameObject>();
    private Transform spawnPoint;

    private void Start()
    {
        spawnPoint = AStarManager.instance.StartNode.transform;
    }

    public void CheckSpawnerEvent()
    {
        if (TutorialContent.Instance.CurrentIndexGuide < 15)
            return;

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
        GameObject a = Instantiate(thiefPrefab, spawnPoint.position, Quaternion.identity);


        if (Random.value < 0.05f) // valor de si va a aparecer un ladron o no, està en 5% por ahora. Despuès podemos hacer para que se fije mas de una vez a ver si puede o no spawnear
            StartCoroutine(TimeForSpawnThief());
    }

    private IEnumerator TimeForSpawnThief()
    {
        float randomTime = Random.Range(200f, 500f);
        Debug.Log("Time Thief: " + randomTime);
        yield return new WaitForSeconds(randomTime);

        if (thiefPool.Count == 0)
        {
            GameObject instance = Instantiate(thiefPrefab, spawnPoint.position, Quaternion.identity);
            thiefPool.Add(instance);
        }
        else
        {
            GameObject thief = thiefPool.Find(inactive => !inactive.activeInHierarchy);
            thief.SetActive(true);
        }
    }
}
