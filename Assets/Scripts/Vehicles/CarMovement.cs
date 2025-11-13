using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private int currentPath;

    private CreateCars createCars;
    private int currentWaypointIndex = 0;
    private Vector3[] offsetWaypoints;
    private WayPoints[] waypointsToTravel;


    private void Start()
    {
        createCars = GetComponentInParent<CreateCars>();
        waypointsToTravel = createCars.GetWayPoints();

        foreach (WayPoints path in waypointsToTravel)
        {
            if (path.pathNumber == currentPath)
            {
                transform.position = path.spawnPoint.position + offset;

                offsetWaypoints = new Vector3[path.waypoints.Length];
                for (int i = 0; i < path.waypoints.Length; i++)
                {
                    offsetWaypoints[i] = path.waypoints[i].position;
                }
            }
        }
    }

    private void Update()
    {
        if (offsetWaypoints == null || offsetWaypoints.Length == 0) return;

        Vector3 target = offsetWaypoints[currentWaypointIndex];
        Vector3 direction = (target - transform.position).normalized;

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (direction.magnitude > 0.1f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }

        if (Vector3.Distance(transform.position, target) < 0.3f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % offsetWaypoints.Length;
        }
    }
}