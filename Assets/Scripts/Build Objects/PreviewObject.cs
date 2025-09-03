using UnityEngine;

public class PreviewObject : MonoBehaviour
{
    [SerializeField] private LayerMask floorLayer;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float maxDistance = 8f;

    private Vector3 originalScale;
    private bool scaleInitialized = false;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    private void OnEnable()
    {
        if (!scaleInitialized)
        {
            originalScale = transform.localScale;
            scaleInitialized = true;
        }
        transform.localScale = originalScale;
    }

    void Update()
    {
        if (!enabled) return;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance, floorLayer))
        {
            transform.position = hit.point;
            transform.localScale = originalScale;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.Rotate(0, 90, 0);
            transform.localScale = originalScale;
        }
    }

    private void OnDisable()
    {
        if (scaleInitialized)
        {
            originalScale = transform.localScale;
        }
    }
}
