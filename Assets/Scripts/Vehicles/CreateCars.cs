using UnityEngine;

public class CreateCars : MonoBehaviour
{
    [SerializeField] private GameObject[] carsPrefabs;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private WayPoints[] waypoints;

    private void Start()
    {
        SpawnCars();
    }

    private void SpawnCars()
    {
        foreach (GameObject car in carsPrefabs)
        {
            GameObject entorno = Instantiate(car, spawnPoint.position, spawnPoint.rotation, transform);

            if (entorno.TryGetComponent<Animator>(out Animator animator))
                animator.SetBool("IsWalking", true);
            else
            {
                Animator childrenAnim = entorno.GetComponentInChildren<Animator>();
                if (childrenAnim != null)
                    childrenAnim.SetBool("IsWalking", true);
            }

        }
    }

    public Vector3 GetSpawnPoint(int path)
    {
        foreach (WayPoints waypoint in waypoints)
        {
            if (waypoint.pathNumber == path)
                return waypoint.spawnPoint.position;
        }

        return Vector3.zero;
    }

    public WayPoints[] GetWayPoints()
    {
        return waypoints;
    }
}

[System.Serializable]
public class WayPoints
{
    public int pathNumber;
    public Transform[] waypoints;
    public Transform spawnPoint;
}
