using UnityEngine;

public class PreviewObject : MonoBehaviour
{
    [SerializeField] private LayerMask floorLayer;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float maxDistance = 8f;

    void Update()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance, floorLayer))
        {
            transform.position = hit.point;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.Rotate(0, 0, 90);
        }
    }
}
