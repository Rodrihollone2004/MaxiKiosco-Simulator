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
            if (!TutorialContent.Instance.IsFirstCartonero)
            {
                TutorialContent tutorial = TutorialContent.Instance;
                tutorial.CartoneroImage.SetActive(true);
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                TutorialContent.Instance.IsFirstCartonero = true;
            }

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

        StartCoroutine(TimeForSpawnThief());
    }

    private IEnumerator TimeForSpawnThief()
    {
        float randomTime = Random.Range(200f, 400f);
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

        if (!TutorialContent.Instance.IsFirstThief)
        {
            TutorialContent tutorial = TutorialContent.Instance;
            tutorial.ThiefImage.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            TutorialContent.Instance.IsFirstThief = true;
        }
    }
}
