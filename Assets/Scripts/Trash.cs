using UnityEngine;

public class Trash : MonoBehaviour
{
    private ClientTrashSpawner spawner;
    public bool CanBeCleaned => true;

    private void Awake()
    {
        spawner = FindObjectOfType<ClientTrashSpawner>();
    }

    public void Clean()
    {
        if (spawner != null)
            spawner.TrashCleaned();

        Destroy(gameObject);
    }
}
