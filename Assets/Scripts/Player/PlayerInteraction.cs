using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private KeyCode interactKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode subtractKey = KeyCode.Mouse1;
    [SerializeField] private KeyCode dropKey = KeyCode.G;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private Transform holdPosition;

    private FurnitureBox furnitureBox;
    private ProductPlaceManager productPlace;

    [Header("Throw")]
    [SerializeField] private float throwForce = 10f;

    [SerializeField] private GameObject dropHintUI;
    [SerializeField] private TMP_Text hintText;

    public GameObject DropHintUI { get => dropHintUI; private set => dropHintUI = value; }

    [Header("Efects")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioClip dropSound;
    [SerializeField] private AudioSource audioSource;

    //Esto es para el objeto a pickear, su rigidBody, Collider, etc.
    private IInteractable currentInteractable;
    private GameObject heldObject;
    private Rigidbody heldObjectRb;
    private Collider heldObjectCollider;

    [Header("Cleaning")]
    [SerializeField] private LayerMask trashLayer;
    private Broom heldBroom;

    private void Update()
    {
        HandleHighlight();

        if (Input.GetKeyDown(interactKey))
        {
            if (heldObject == null)
            {
                TryPickUp();
            }
            else if (heldBroom != null)
            {
                TryClean();
            }
        }

        if (Input.GetKeyDown(subtractKey))
            TrySubtractBill();

        if (Input.GetKeyDown(dropKey) && heldObject != null)
            DropObject();

        if (Input.GetKeyDown(KeyCode.E) && furnitureBox != null && furnitureBox.CurrentPreview != null && furnitureBox.CurrentPreview.activeSelf)
            furnitureBox.PlaceFurniture();

        if (Input.GetKeyDown(KeyCode.E) && productPlace != null && productPlace.CurrentPreview != null && productPlace.CurrentPreview.activeSelf)
            productPlace.PlaceProduct();
    }
    private void TrySubtractBill()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactRange, interactLayer))
            if (hit.collider.TryGetComponent(out MoneyBill moneyBill))
                moneyBill.InteractSubtract();
    }

    // detecta los objetos con raycast y aplica el outline
    private void HandleHighlight()
    {
        if (heldObject != null)
        {
            if (currentInteractable != null)
            {
                currentInteractable.Unhighlight();
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
                    if (currentInteractable != null)
                        currentInteractable.Unhighlight();

                    currentInteractable = interactable;
                    currentInteractable.Highlight();
                }
            }
        }
        else
        {
            if (currentInteractable != null)
            {
                currentInteractable.Unhighlight();
                currentInteractable = null;
            }
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
                if (interactable.CanBePickedUp)
                    PickUp(hit.collider.gameObject);
            }
        }
    }

    private void TryClean()
    {
        if (heldBroom == null) return;

        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactRange, trashLayer))
        {
            if (hit.collider.TryGetComponent(out Trash trash) && trash.CanBeCleaned)
            {
                trash.Clean();
            }
        }
    }

    // hace el objeto hijo del holdposition al agarrarlo
    private void PickUp(GameObject objToPickUp)
    {
        if (currentInteractable != null)
        {
            currentInteractable.Unhighlight();
            currentInteractable = null;
        }

        heldObject = objToPickUp;

        if (objToPickUp.TryGetComponent(out Broom broom))
        {
            heldBroom = broom;
            heldBroom.SetHeld(true);
        }

        if (objToPickUp.TryGetComponent(out FurnitureBox fur))
            furnitureBox = fur;

        if (objToPickUp.TryGetComponent(out ProductPlaceManager productPlace))
            this.productPlace = productPlace;

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

            string productName = "Heladera";

            ProductInteractable interactable = heldObject.GetComponentInChildren<ProductInteractable>(true);
            if (interactable != null && interactable.ProductData != null)
            {
                productName = interactable.ProductData.Name;
            }

            hintText.text = $"{productName}\n" +
                    $"E  para colocar\n" +
                    $"R  para rotar\n" +
                    $"G  para soltar\n";
        }

        if (audioSource != null && pickupSound != null)
            audioSource.PlayOneShot(pickupSound);
    }

    // devuelve todas las propiedades al objeto y aplica fuerza de lanzamiento
    private void DropObject()
    {
        if (heldObject != null)
        {
            heldObject.transform.position += Vector3.up * 0.2f;

            if (heldBroom != null)
            {
                heldBroom.SetHeld(false);
                heldBroom = null;
            }

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
                hintText.text = "";
            }

            heldObject.transform.SetParent(null);
            heldObject = null;
            heldObjectRb = null;
            heldObjectCollider = null;
            currentInteractable = null;

            if (furnitureBox != null && furnitureBox.CurrentPreview != null && furnitureBox.AllZones.Length > 0)
            {
                furnitureBox.CurrentPreview.SetActive(false);
                foreach (PlacementZone zone in furnitureBox.AllZones)
                    zone.HideVisual();
            }

            if (furnitureBox != null)
                furnitureBox = null;

            if (productPlace != null && productPlace.CurrentPreview != null && productPlace.AllZones.Length > 0)
            {
                productPlace.CurrentPreview.SetActive(false);
                foreach (PlacementZoneProducts zone in productPlace.AllZones)
                    zone.HideVisual();
            }

            if (productPlace != null)
                furnitureBox = null;

            if (audioSource != null && dropSound != null)
                audioSource.PlayOneShot(dropSound);
        }
    }
}

