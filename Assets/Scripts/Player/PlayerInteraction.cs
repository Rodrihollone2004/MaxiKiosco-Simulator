using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private KeyCode dropKey = KeyCode.G;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private Transform holdPosition;

    [Header("Throw")]
    [SerializeField] private float throwForce = 10f;

    [SerializeField] private GameObject dropHintUI;

    [Header("Efects")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioClip dropSound;
    [SerializeField] private AudioSource audioSource;

    //Esto es para el objeto a pickear, su rigidBody, Collider, etc.
    private IInteractable currentInteractable;
    private GameObject heldObject;
    private Rigidbody heldObjectRb;
    private Collider heldObjectCollider;
    
    private void Update()
    {
        HandleHighlight();

        if (Input.GetKeyDown(interactKey))
        {
            if (heldObject == null)
            {
                TryPickUp();
            }
        }

        if (Input.GetKeyDown(dropKey) && heldObject != null)
        {
            DropObject();
        }
    }

    // detecta los objetos con raycast y aplica el outline
    private void HandleHighlight()
    {
        if (heldObject != null)
        {
            if (currentInteractable != null)
            {
                if (currentInteractable is Product oldProduct)
                    oldProduct.Unhighlight();

                currentInteractable = null;
            }
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactRange, interactLayer))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                if (interactable != currentInteractable)
                {
                    if (currentInteractable is Product oldProduct)
                        oldProduct.Unhighlight();

                    currentInteractable = interactable;

                    if (currentInteractable is Product newProduct)
                        newProduct.Highlight();
                }
            }
        }
        else
        {
            if (currentInteractable is Product oldProduct)
                oldProduct.Unhighlight();

            currentInteractable = null;
        }
    }

    // intenta recoger un objeto con el raycast
    private void TryPickUp()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactRange, interactLayer))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact();
                PickUp(hit.collider.gameObject);
            }
        }
    }

    // hace el objeto hijo del holdposition al agarrarlo
    private void PickUp(GameObject objToPickUp)
    {
        if (currentInteractable != null)
        {
            (currentInteractable as Product)?.Unhighlight();
        }

        heldObject = objToPickUp;

        if (objToPickUp.TryGetComponent(out Rigidbody rb))
             heldObjectRb = rb;

        if (objToPickUp.TryGetComponent(out Collider col))
             heldObjectCollider = col;

        if (heldObjectRb != null)
            heldObjectRb.isKinematic = true;

        if (heldObjectCollider != null)
            heldObjectCollider.isTrigger = true;

        heldObject.transform.SetParent(holdPosition);
        heldObject.transform.localPosition = Vector3.zero;
        heldObject.transform.localRotation = Quaternion.identity;

        if (dropHintUI != null)
        {
            dropHintUI.SetActive(true);
        }

        if (audioSource != null && pickupSound != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }
    }

    // devuelve todas las propiedades al objeto y aplica fuerza de lanzamiento
    private void DropObject()
    {

        if (heldObject != null)
        {
            heldObject.transform.position += Vector3.up * 0.2f;

            if (heldObjectRb != null)
            {
                heldObjectRb.isKinematic = false;
                heldObjectRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                heldObjectRb.AddForce(cameraTransform.forward * throwForce, ForceMode.Impulse);
            }

            if (heldObjectCollider != null)
                heldObjectCollider.isTrigger = false;

            if (dropHintUI != null)
            {
                dropHintUI.SetActive(false);
            }

            heldObject.transform.SetParent(null);
            heldObject = null;
            heldObjectRb = null;
            heldObjectCollider = null;

            if (audioSource != null && dropSound != null)
            {
                audioSource.PlayOneShot(dropSound);
            }
        }
    }
}

