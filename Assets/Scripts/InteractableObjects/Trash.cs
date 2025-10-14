using UnityEngine;

public class Trash : MonoBehaviour
{
    private ClientTrashSpawner spawner;
    private DailySummary dailySummary;
    public bool CanBeCleaned => true;
    public bool ShowNameOnHighlight => false;


    private void Awake()
    {
        spawner = FindObjectOfType<ClientTrashSpawner>();
        dailySummary = FindObjectOfType<DailySummary>();
    }

    public void Clean()
    {
        if (spawner != null)
            spawner.TrashCleaned();

        if (dailySummary != null)
            dailySummary.IncrementTrashCleaned();

        Destroy(gameObject);
    }
}
