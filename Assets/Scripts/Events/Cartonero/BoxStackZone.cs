using System.Collections.Generic;
using UnityEngine;

public class BoxStackZone : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private GameObject brokenBoxPrefab;
    [SerializeField] private float boxHeight = 0.5f;
    [SerializeField] private float yOffset = 0f;
    [SerializeField] private int limitBoxes;
    private int currentBoxes;
    private DailySummary dailySummary;


    private List<GameObject> stackedBoxes = new List<GameObject>();

    public List<GameObject> StackedBoxes { get => stackedBoxes; set => stackedBoxes = value; }

    private void Awake()
    {
        dailySummary = FindObjectOfType<DailySummary>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentBoxes < limitBoxes)
        {
            ProductPlaceManager box = other.GetComponent<ProductPlaceManager>();
            FurnitureBox furnitureBox = other.GetComponent<FurnitureBox>();

            if (box != null && box.IsEmpty || furnitureBox != null && furnitureBox.IsEmpty)
            {
                Vector3 spawnPos = transform.position + Vector3.up * (stackedBoxes.Count * boxHeight + yOffset);

                GameObject newBrokenBox = Instantiate(brokenBoxPrefab, spawnPos, brokenBoxPrefab.transform.rotation);

                newBrokenBox.transform.SetParent(transform);

                stackedBoxes.Add(newBrokenBox);

                Destroy(other.gameObject);

                PlayerInteraction playerInteraction = FindObjectOfType<PlayerInteraction>();
                if (playerInteraction != null)
                    playerInteraction.DropHintUI.SetActive(false);

                if (dailySummary != null)
                    dailySummary.IncrementBoxesThrownAway();
            }
        }
    }
}
