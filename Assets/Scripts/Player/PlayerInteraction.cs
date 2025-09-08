using UnityEngine;
using TMPro;
using UnityEditor.Rendering.Universal;

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
    private ProductPlaceManager boxProduct;
    private GameObject productPlaced;
    private PreviewValidator previewValidator;

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

    [Header("UI Highlight Info")]
    [SerializeField] private GameObject highlightPanel;
    [SerializeField] private TMP_Text highlightNameText;

    private void Update()
    {
        HandleHighlight();

        if (Input.GetKeyDown(interactKey))
        {
            if (heldObject == null && productPlaced == null)
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

        if (Input.GetKeyDown(KeyCode.E) && boxProduct != null && boxProduct.CurrentPreview != null && boxProduct.CurrentPreview.activeSelf)
            boxProduct.PlaceProduct();

        if (Input.GetKeyDown(KeyCode.E) && productPlaced != null && previewValidator != null && previewValidator.IsValidPlacement)
            PlaceProduct();
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

            if (highlightPanel != null)
                highlightPanel.SetActive(false);
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactRange, interactLayer))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                ProductInteractable productPlaced = hit.collider.GetComponent<ProductInteractable>();
                if (productPlaced != null && Input.GetKeyDown(KeyCode.F) && productPlaced.IsPlaced)
                {
                    CheckProduct(productPlaced.gameObject);
                }

                if (interactable != currentInteractable)
                {
                    if (currentInteractable != null)
                        currentInteractable.Unhighlight();

                    currentInteractable = interactable;
                    currentInteractable.Highlight();

                    if (highlightPanel != null && highlightNameText != null)
                    {
                        ProductInteractable productInBox = hit.collider.GetComponentInChildren<ProductInteractable>(true);
                        UpgradeInteractable upgrade = hit.collider.GetComponentInChildren<UpgradeInteractable>(true);

                        if (productInBox != null && productInBox.ProductData != null)
                        {
                            if (productInBox.ShowNameOnHighlight && !productInBox.IsPlaced)
                            {
                                highlightPanel.SetActive(true);
                                highlightNameText.text = productInBox.ProductData.Name;
                            }
                            else
                            {
                                highlightPanel.SetActive(false);
                            }
                        }
                        else if (hit.collider.TryGetComponent<FurnitureBox>(out FurnitureBox furnitureBox))
                            if (furnitureBox != null && furnitureBox.ShowNameOnHighlight)
                            {
                                highlightPanel.SetActive(true);
                                highlightNameText.text = furnitureBox.name;
                            }
                            else
                            {
                                highlightPanel.SetActive(false);
                            }
                        else if (upgrade != null && upgrade.UpgradeData != null)
                            if (upgrade.ShowNameOnHighlight && !upgrade.IsPlaced)
                            {
                                highlightPanel.SetActive(true);
                                highlightNameText.text = upgrade.name;
                            }
                            else
                            {
                                highlightPanel.SetActive(false);
                            }
                    }
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
            if (highlightPanel != null)
                highlightPanel.SetActive(false);
        }
    }

    PlacementZoneProducts[] AllZones;

    public void CheckProduct(GameObject productPlaced)
    {
        this.productPlaced = productPlaced;

        dropHintUI.SetActive(true);

        if (productPlaced.TryGetComponent<PreviewObject>(out PreviewObject preview))
            preview.enabled = true;

        AllZones = FindObjectsOfType<PlacementZoneProducts>();

        Collider col = productPlaced.GetComponent<Collider>();
        col.enabled = false;

        if (productPlaced.TryGetComponent<PreviewValidator>(out PreviewValidator vallidator))
        {
            vallidator.enabled = true;
            previewValidator = vallidator;
        }
        else
        {
            vallidator = productPlaced.AddComponent<PreviewValidator>();
            vallidator.Initialize(new Color(0f, 1f, 0f, 0.5f), Color.red);
            previewValidator = vallidator;
        }

        if (productPlaced.TryGetComponent<ProductInteractable>(out ProductInteractable interactable))
        {
            ProductInteractable productInteractable = interactable;
            string productPlaceZone = productInteractable.ProductData.PlaceZone;
            productInteractable.IsPlaced = false;

            hintText.text = $"{productInteractable.ProductData.Name}\n" +
                    $"E para colocar\n";

            foreach (PlacementZoneProducts zone in AllZones)
                zone.ShowVisual(productPlaceZone);
        }
        else if (productPlaced.TryGetComponent<UpgradeInteractable>(out UpgradeInteractable upgrade))
        {
            UpgradeInteractable productInteractable = upgrade;
            string upgradePlaceZone = productInteractable.UpgradeData.PlaceZone;

            foreach (PlacementZoneProducts zone in AllZones)
                zone.ShowVisual(upgradePlaceZone);
        }
    }

    private void PlaceProduct()
    {
        PreviewObject moveObject = productPlaced.GetComponent<PreviewObject>();
        moveObject.enabled = false;

        Collider col = productPlaced.GetComponent<Collider>();
        col.enabled = true;

        previewValidator.BackToNormal();
        previewValidator.enabled = false;

        ProductInteractable product = productPlaced.GetComponent<ProductInteractable>();
        product.IsPlaced = true;

        productPlaced = null;

        dropHintUI.SetActive(false);

        foreach (PlacementZoneProducts zone in AllZones)
            zone.HideVisual();
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

            hintText.text = $"{broom.name}\n" +
                    $"LMB para limpiar\n" +
                    $"G  para soltar\n";
        }

        if (objToPickUp.TryGetComponent(out FurnitureBox fur))
            furnitureBox = fur;

        if (objToPickUp.TryGetComponent(out ProductPlaceManager productPlace))
            this.boxProduct = productPlace;

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

            string productName = heldObject.name;

            ProductInteractable interactable = heldObject.GetComponentInChildren<ProductInteractable>(true);
            UpgradeInteractable upgrade = heldObject.GetComponentInChildren<UpgradeInteractable>(true);
            if (interactable != null && interactable.ProductData != null)
            {
                productName = interactable.ProductData.Name;

                hintText.text = $"{productName}\n" +
                    $"E  para colocar\n" +
                    $"R  para rotar\n" +
                    $"G  para soltar\n";
            }
            else if (heldObject.TryGetComponent<FurnitureBox>(out FurnitureBox furnitureBox))
            {
                productName = furnitureBox.name;

                hintText.text = $"{productName}\n" +
                    $"E  para colocar\n" +
                    $"R  para rotar\n" +
                    $"G  para soltar\n";
            }
            else if (upgrade != null && upgrade.UpgradeData != null)
            {
                productName = upgrade.UpgradeData.Name;

                hintText.text = $"{productName}\n" +
                    $"E  para colocar\n" +
                    $"R  para rotar\n" +
                    $"G  para soltar\n";
            }
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

            if (boxProduct != null && boxProduct.CurrentPreview != null && boxProduct.AllZones.Length > 0)
            {
                boxProduct.CurrentPreview.SetActive(false);
                foreach (PlacementZoneProducts zone in boxProduct.AllZones)
                    zone.HideVisual();
            }

            if (boxProduct != null)
                furnitureBox = null;

            if (audioSource != null && dropSound != null)
                audioSource.PlayOneShot(dropSound);
        }
    }
}

