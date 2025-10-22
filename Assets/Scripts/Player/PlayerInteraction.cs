using TMPro;
using Unity.Burst.CompilerServices;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private KeyCode interactKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode subtractKey = KeyCode.Mouse1;
    [SerializeField] private KeyCode dropKey = KeyCode.G;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private LayerMask clientLayer;
    [SerializeField] private LayerMask tutorialLayer;
    [SerializeField] private LayerMask fridgeLayer;
    [SerializeField] private LayerMask layersLock;
    [SerializeField] private Transform holdPosition;

    private FurnitureBox furnitureBox;
    private ProductPlaceManager boxProduct;
    private GameObject productPlaced;
    private PreviewValidator previewValidator;
    private PlayerEconomy playerEconomy;
    private CashRegisterInteraction cashRegisterInteraction;
    [SerializeField] private DailySummary dailySummary;

    [Header("Throw")]
    [SerializeField] private float throwForce = 10f;

    [SerializeField] private GameObject dropHintUI;
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private TMP_Text nameText;

    public GameObject DropHintUI { get => dropHintUI; private set => dropHintUI = value; }
    public AudioClip PlaceProduct_ { get => placeProduct; set => placeProduct = value; }
    public AudioClip ErrorSound { get => errorSound; set => errorSound = value; }

    [Header("Efects")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioClip dropSound;
    [SerializeField] private AudioClip placeProduct;
    [SerializeField] private AudioClip cleanTrashSound;
    [SerializeField] private AudioClip errorSound;
    private AudioSource audioSource;

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


    private int ignorePlayer;

    private void Awake()
    {
        playerEconomy = GetComponent<PlayerEconomy>();
        cashRegisterInteraction = GetComponent<CashRegisterInteraction>();
        audioSource = GetComponent<AudioSource>();

        ignorePlayer = ~LayerMask.GetMask("Player");
    }

    private void Update()
    {
        HandleHighlight();

        if (Input.GetKeyDown(interactKey))
        {
            if (heldObject == null && productPlaced == null)
            {
                TryPickUp();
            }
            else if (heldBroom != null && TutorialContent.Instance.CurrentIndexGuide > 11)
            {
                TryClean();
            }
        }

        if (Input.GetKeyDown(subtractKey))
            TrySubtractBill();

        if (Input.GetKeyDown(dropKey) && heldObject != null)
            DropObject();

        if (Input.GetKeyDown(KeyCode.E) && furnitureBox != null && furnitureBox.CurrentPreview != null && furnitureBox.CurrentPreview.activeSelf)
        {
            furnitureBox.PlaceFurniture();
        }

        if (Input.GetKeyDown(KeyCode.E) && boxProduct != null && boxProduct.CurrentPreview != null && boxProduct.CurrentPreview.activeSelf)
        {
            boxProduct.PlaceProduct();
        }

        if (Input.GetKeyDown(KeyCode.E) && productPlaced != null)
            PlaceRepick();

        if (boxProduct != null && boxProduct.IsEmpty && heldObject == boxProduct.gameObject
            || furnitureBox != null && furnitureBox.IsEmpty && heldObject == furnitureBox.gameObject)
            CheckTrashBoxes();

        if (Input.GetKeyDown(interactKey) && TutorialContent.Instance.CurrentIndexGuide == 1
            || Input.GetKeyDown(interactKey) && TutorialContent.Instance.CurrentIndexGuide == 14)
            CheckTutorialStart();
    }

    private void CheckTutorialStart()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactRange, tutorialLayer))
        {
            if (!TutorialContent.Instance.IsComplete)
                TutorialContent.Instance.CompleteStep(1);
            else
            {
                TutorialContent.Instance.CompleteStep(14);
                TutorialContent.Instance.DesactivateText();
            }
        }
    }

    private void CheckTrashBoxes()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactRange, trashLayer))
        {
            if (hit.collider.CompareTag("Trash"))
            {
                Destroy(heldObject);
                heldObject = null;
                dropHintUI.SetActive(false);
                hintText.text = "";
                playerEconomy.ReceivePayment(20);

                if (dailySummary != null)
                    dailySummary.IncrementBoxesThrownAway();
            }
        }
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


        RaycastHit hitFridge;
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactRange, ignorePlayer))
        {
            if (hit.collider.gameObject.layer != 3)
            {
                if (currentInteractable != null)
                {
                    currentInteractable.Unhighlight();
                    currentInteractable = null;
                }

                if (highlightPanel != null && highlightPanel.activeInHierarchy)
                    highlightPanel.SetActive(false);
                return;
            }

            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                if (Input.GetKeyDown(KeyCode.F) && !cashRegisterInteraction.InCashRegister && heldObject == null)
                {

                    ProductInteractable productPlaced = hit.collider.GetComponent<ProductInteractable>();
                    UpgradeInteractable upgradeInteractable = hit.collider.GetComponent<UpgradeInteractable>();

                    if (productPlaced != null && productPlaced.IsPlaced)
                        CheckRepick(productPlaced.gameObject);
                    else if (upgradeInteractable != null && upgradeInteractable.IsPlaced)
                        CheckRepick(upgradeInteractable.gameObject);
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
        else if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hitFridge, interactRange, fridgeLayer))
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                CheckRepick(hitFridge.collider.gameObject);
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
    PlacementZone furnitureZones;

    public void CheckRepick(GameObject productPlaced)
    {
        this.productPlaced = productPlaced;

        dropHintUI.SetActive(true);

        if (productPlaced.TryGetComponent<PreviewObject>(out PreviewObject preview))
            preview.enabled = true;
        else if (productPlaced.transform.parent.parent != null)
        {
            PreviewObject parentFridge = productPlaced.transform.parent.parent.GetComponent<PreviewObject>();
            parentFridge.enabled = true;
            productPlaced = parentFridge.gameObject;
            this.productPlaced = productPlaced;
        }

        heldObject = productPlaced;

        Collider col = productPlaced.GetComponent<Collider>();
        col.enabled = false;

        audioSource.PlayOneShot(pickupSound);

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
            AllZones = FindObjectsOfType<PlacementZoneProducts>();

            ProductInteractable productInteractable = interactable;
            string productPlaceZone = productInteractable.ProductData.PlaceZone;
            productInteractable.IsPlaced = false;

            nameText.text = productInteractable.ProductData.Name;
            hintText.text = $"E  para colocar\n" +
                    $"R  para rotar\n";

            foreach (PlacementZoneProducts zone in AllZones)
                zone.ShowVisual(productPlaceZone);
        }
        else if (productPlaced.TryGetComponent<UpgradeInteractable>(out UpgradeInteractable upgrade))
        {
            AllZones = FindObjectsOfType<PlacementZoneProducts>();

            UpgradeInteractable upgradeInteractable = upgrade;
            string upgradePlaceZone = upgradeInteractable.UpgradeData.PlaceZone;
            upgradeInteractable.IsPlaced = false;

            nameText.text = upgradeInteractable.UpgradeData.Name;
            hintText.text = $"E  para colocar\n" +
                    $"R  para rotar\n";

            foreach (PlacementZoneProducts zone in AllZones)
                zone.ShowVisual(upgradePlaceZone);
        }
        else
        {
            furnitureZones = FindObjectOfType<PlacementZone>();
            string fridgePlaceZone = productPlaced.GetComponentInChildren<PlacementZoneProducts>().ListZones[0].name;

            nameText.text = fridgePlaceZone;
            hintText.text = $"E  para colocar\n" +
                    $"R  para rotar\n";

            furnitureZones.ShowVisual();

            Collider[] colliders = productPlaced.GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders) collider.enabled = false;

        }
    }

    private void PlaceRepick()
    {
        if (previewValidator != null && previewValidator.IsValidPlacement)
        {
            PreviewObject moveObject = productPlaced.GetComponent<PreviewObject>();
            moveObject.enabled = false;

            Collider col = productPlaced.GetComponent<Collider>();
            col.enabled = true;

            previewValidator.BackToNormal();
            previewValidator.enabled = false;

            if (productPlaced.TryGetComponent<ProductInteractable>(out ProductInteractable product))
            {
                product.IsPlaced = true;
                product.CheckParent(productPlaced, product);
            }
            else if (productPlaced.TryGetComponent<UpgradeInteractable>(out UpgradeInteractable upgrade))
                upgrade.IsPlaced = true;
            else if (productPlaced.layer == LayerMask.NameToLayer("fridge"))
            {
                Collider[] colliders = productPlaced.GetComponentsInChildren<Collider>();
                foreach (Collider collider in colliders) collider.enabled = true;
            }

            heldObject = null;
            productPlaced = null;

            dropHintUI.SetActive(false);

            if (AllZones != null)
                foreach (PlacementZoneProducts zone in AllZones)
                {
                    zone.HideVisual();
                    AllZones = null;
                }
            else if (furnitureZones != null)
            {
                furnitureZones.HideVisual();
                furnitureZones = null;
            }

            audioSource.PlayOneShot(placeProduct);
        }
        else
            audioSource.PlayOneShot(errorSound);
    }

    // intenta recoger un objeto con el raycast
    private void TryPickUp()
    {
        RaycastHit hit;

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactRange, ignorePlayer))
        {
            if (hit.collider.gameObject.layer != 3)
                return;

            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact();

                if (interactable.CanBePickedUp && TutorialContent.Instance.CurrentIndexGuide > 11)
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

                if (audioSource != null && cleanTrashSound != null)
                    audioSource.PlayOneShot(cleanTrashSound);
            }
        }
        else if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactRange, clientLayer))
        {
            if (hit.collider.TryGetComponent(out ThiefController currentClient) && !currentClient.WasHit)
            {
                if (currentClient.IsStealing)
                {
                    currentClient.GetHit();
                    playerEconomy.ReceivePayment(currentClient.newTotal);

                    if (dailySummary != null)
                        dailySummary.IncrementThievesCaught();

                }
                else
                {
                    currentClient.GetHit();

                    if (dailySummary != null)
                        dailySummary.IncrementThievesCaught();
                }
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

            nameText.text = $"{broom.name}";
            hintText.text = $"LMB para limpiar\n" +
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
        heldObject.layer = LayerMask.NameToLayer("inHand");

        if (objToPickUp.TryGetComponent(out Broom broomComponent))
        {
            heldObject.transform.localPosition = broomComponent.holdOffset;
            heldObject.transform.localRotation = Quaternion.Euler(broomComponent.holdRotation);
        }
        else
        {
            // Para el resto de los objetos
            heldObject.transform.localPosition = Vector3.zero;
            heldObject.transform.localRotation = Quaternion.identity;
        }

        CheckUIText();

        if (audioSource != null && pickupSound != null)
            audioSource.PlayOneShot(pickupSound);
    }

    public void CheckUIText()
    {
        if (dropHintUI != null)
        {
            dropHintUI.SetActive(true);

            string productName = heldObject.name;

            if (heldObject.TryGetComponent(out Broom broom))
            {


                nameText.text = $"{broom.name}";
                hintText.text = $"LMB para limpiar\n" +
                                $"G  para soltar\n";
                return;
            }

            ProductInteractable interactable = heldObject.GetComponentInChildren<ProductInteractable>(true);
            UpgradeInteractable upgrade = heldObject.GetComponentInChildren<UpgradeInteractable>(true);
            if (interactable != null && interactable.ProductData != null && !boxProduct.IsEmpty)
            {
                productName = interactable.ProductData.Name;

                nameText.text = $"{productName}";
                hintText.text =
                    $"E  para colocar\n" +
                    $"R  para rotar\n" +
                    $"G  para soltar\n";
            }
            else if (heldObject.TryGetComponent<FurnitureBox>(out FurnitureBox furnitureBox) && !furnitureBox.IsEmpty)
            {
                productName = furnitureBox.name;

                nameText.text = $"{productName}";
                hintText.text =
                    $"E  para colocar\n" +
                    $"R  para rotar\n" +
                    $"G  para soltar\n";
            }
            else if (upgrade != null && upgrade.UpgradeData != null && !boxProduct.IsEmpty)
            {
                productName = upgrade.UpgradeData.Name;

                nameText.text = $"{productName}";
                hintText.text =
                    $"E  para colocar\n" +
                    $"R  para rotar\n" +
                    $"G  para soltar\n";
            }
            else if (boxProduct != null && boxProduct.IsEmpty || furnitureBox != null && this.furnitureBox.IsEmpty)
            {
                nameText.text = $"Caja Vacía\n";
                hintText.text = $"Acercate al tacho/pallete para depositarla\n";
            }
        }
    }

    // devuelve todas las propiedades al objeto y aplica fuerza de lanzamiento
    private void DropObject()
    {
        if (heldObject != null)
        {
            heldObjectCollider.enabled = false;
            bool hitForward = Physics.Raycast(cameraTransform.position, cameraTransform.forward, 2.5f, layersLock);

            heldObject.transform.position += Vector3.up * 0.2f;

            if (heldObjectRb != null)
            {
                heldObjectRb.isKinematic = false;
                heldObjectRb.collisionDetectionMode = CollisionDetectionMode.Continuous;

                if (heldBroom != null)
                {
                    bool broomBlockedForward = Physics.Raycast(cameraTransform.position, cameraTransform.forward, 2.5f, layersLock);
                    bool broomBlockedRight = Physics.Raycast(cameraTransform.position, cameraTransform.right, 2.5f, layersLock);

                    if (broomBlockedForward || broomBlockedRight)
                    {
                        heldObjectRb.transform.position = transform.position;
                    }
                    else
                        heldObjectRb.AddForce(cameraTransform.forward * (throwForce * 0.5f), ForceMode.Impulse);

                    heldBroom.SetHeld(false);
                    heldBroom = null;
                }
                else
                {
                    if (!hitForward)
                        heldObjectRb.AddForce(cameraTransform.forward * throwForce, ForceMode.Impulse);
                    else
                        heldObjectRb.transform.position = transform.position;
                }
            }

            if (heldObjectCollider != null)
            {
                heldObjectCollider.enabled = true;
                heldObjectCollider.isTrigger = false;
            }

            if (dropHintUI != null)
            {
                dropHintUI.SetActive(false);
                hintText.text = "";
            }

            heldObject.transform.SetParent(null);
            heldObject.layer = LayerMask.NameToLayer("Interact");
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
    public bool HasBoxInHand()
    {
        return heldObject != null && (boxProduct != null || furnitureBox != null);
    }
}

