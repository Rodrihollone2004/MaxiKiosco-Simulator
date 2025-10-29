using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private KeyCode interactKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode subtractKey = KeyCode.Mouse1;
    [SerializeField] private KeyCode dropKey = KeyCode.G;
    [SerializeField] private KeyCode sellKey = KeyCode.V;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private LayerMask clientLayer;
    [SerializeField] private LayerMask tutorialLayer;
    [SerializeField] private LayerMask fridgeLayer;
    [SerializeField] private LayerMask layersLock;
    [SerializeField] private Transform holdPosition;

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
    public TMP_Text HintText { get => hintText; set => hintText = value; }

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

    [Header("UI Progress")]
    [SerializeField] private Image progressCircle;

    private int timerRepick;
    private int timerSell;
    [SerializeField] private int limitTimerDelay;
    private int ignorePlayer;

    private bool isShowingAdvice;

    public LightSwitch LightSwitch { get; private set; }

    private void Awake()
    {
        LightSwitch = FindObjectOfType<LightSwitch>();
        playerEconomy = GetComponent<PlayerEconomy>();
        cashRegisterInteraction = GetComponent<CashRegisterInteraction>();
        audioSource = GetComponent<AudioSource>();

        ignorePlayer = ~LayerMask.GetMask("Player");

        hintText.text = "LMB para interactuar\n" +
                    "F para repickear\n";
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

        if (Input.GetKeyDown(KeyCode.E) && boxProduct != null && boxProduct.CurrentPreview != null && boxProduct.CurrentPreview.activeSelf)
        {
            boxProduct.PlaceProduct();
        }

        if (Input.GetKeyDown(KeyCode.E) && productPlaced != null)
            PlaceRepick();

        if (boxProduct != null && boxProduct.IsEmpty && heldObject == boxProduct.gameObject)
            CheckTrashBoxes();

        if (Input.GetKeyDown(interactKey) && TutorialContent.Instance.CurrentIndexGuide == 1
            || Input.GetKeyDown(interactKey) && TutorialContent.Instance.CurrentIndexGuide == 14)
            CheckTutorialStart();

        if (Input.GetKey(KeyCode.V) && productPlaced != null)
        {
            timerSell += 1;
            ShowProgressCircle((float)timerSell / limitTimerDelay);

            if (timerSell >= limitTimerDelay)
            {
                UpgradeInteractable upgradeSell = productPlaced.GetComponent<UpgradeInteractable>();

                timerSell = 0;
                HideProgressCircle();

                if (upgradeSell != null && !upgradeSell.IsPlaced)
                    CheckSell(upgradeSell);
            }
        }
        else if (Input.GetKeyUp(KeyCode.V))
        {
            timerSell = 0;
            HideProgressCircle();
        }
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
                hintText.text = "LMB para interactuar\n" +
                    "F para repickear\n";
                playerEconomy.ReceivePayment(20);

                if (dailySummary != null)
                    dailySummary.IncrementBoxesThrownAway();
            }
        }
    }

    private void TrySubtractBill()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactRange, ignorePlayer))
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
            if (hit.collider.gameObject.layer != 3 && hit.collider.gameObject.layer != 14)
            {
                if (currentInteractable != null)
                {
                    currentInteractable.Unhighlight();
                    currentInteractable = null;
                }

                if (highlightPanel != null && highlightPanel.activeInHierarchy)
                    highlightPanel.SetActive(false);

                HideProgressCircle();
                return;
            }

            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                if (Input.GetKey(KeyCode.F) && !cashRegisterInteraction.InCashRegister && heldObject == null)
                {
                    timerRepick += 1;

                    ShowProgressCircle((float)timerRepick / limitTimerDelay);

                    if (timerRepick >= limitTimerDelay)
                    {
                        ProductInteractable productPlaced = hit.collider.GetComponent<ProductInteractable>();
                        UpgradeInteractable upgradeInteractable = hit.collider.GetComponent<UpgradeInteractable>();
                        if (hit.transform.parent != null && hit.transform.parent.parent != null)
                        {
                            Transform fridge = hit.transform.parent.parent;
                            upgradeInteractable = fridge.GetComponent<UpgradeInteractable>();
                        }

                        timerRepick = 0;
                        HideProgressCircle();

                        if (productPlaced != null && productPlaced.IsPlaced)
                            CheckRepick(productPlaced.gameObject);
                        else if (upgradeInteractable != null && upgradeInteractable.IsPlaced)
                            CheckRepick(upgradeInteractable.gameObject);
                    }
                }
                else if (Input.GetKeyUp(KeyCode.F))
                {
                    timerRepick = 0;
                    HideProgressCircle();
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

                        if (upgrade != null && upgrade.transform.parent != null && upgrade.transform.parent.parent != null)
                        {
                            upgrade = null;
                        }

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
            HideProgressCircle();
        }
    }

    private void ShowProgressCircle(float progress)
    {
        if (progressCircle == null) return;
        if (!progressCircle.gameObject.activeSelf)
            progressCircle.gameObject.SetActive(true);

        progressCircle.fillAmount = Mathf.Clamp01(progress);
    }

    private void HideProgressCircle()
    {
        if (progressCircle == null) return;
        progressCircle.fillAmount = 0;
        progressCircle.gameObject.SetActive(false);
    }


    PlacementZoneProducts[] AllZones;

    public void CheckRepick(GameObject productRef)
    {
        this.productPlaced = productRef;

        dropHintUI.SetActive(true);

        if (productPlaced.TryGetComponent<PreviewObject>(out PreviewObject preview))
            preview.enabled = true;

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
                    $"R  para rotar\n" +
                    "V para vender";

            foreach (PlacementZoneProducts zone in AllZones)
                zone.ShowVisual(upgradePlaceZone);

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
            {
                upgrade.IsPlaced = true;
                Collider[] colliders = productPlaced.GetComponentsInChildren<Collider>();
                foreach (Collider collider in colliders) collider.enabled = true;
            }

            heldObject = null;
            productPlaced = null;

            dropHintUI.SetActive(false);
            hintText.text = "LMB para interactuar\n" +
                    "F para repickear\n";

            if (AllZones != null)
                foreach (PlacementZoneProducts zone in AllZones)
                {
                    zone.HideVisual();
                    AllZones = null;
                }

            audioSource.PlayOneShot(placeProduct);
        }
        else
            audioSource.PlayOneShot(errorSound);
    }

    private void CheckSell(UpgradeInteractable upgrade)
    {
        if (isShowingAdvice)
            return;

        PlacementZoneProducts tempPlacement = upgrade.GetComponentInChildren<PlacementZoneProducts>();
        ProductInteractable[] tempProducts = null;

        if (tempPlacement != null)
            tempProducts = tempPlacement.GetComponentsInChildren<ProductInteractable>();

        if (tempProducts != null && tempProducts.Length > 0)
            StartCoroutine(Advice());
        else
            DeleteUpgrade(upgrade);
    }

    private void DeleteUpgrade(UpgradeInteractable upgrade)
    {
        if (AllZones != null)
            foreach (PlacementZoneProducts zone in AllZones)
            {
                zone.HideVisual();
                AllZones = null;
            }

        Light light = upgrade.GetComponentInChildren<Light>();
        if (light != null && LightSwitch.totalLights.Contains(light))
            LightSwitch.totalLights.Remove(light);

        dropHintUI.SetActive(false);
        hintText.text = "LMB para interactuar\n" +
            "F para repickear\n";
        playerEconomy.ReceivePayment(upgrade.UpgradeData.Price);
        Destroy(upgrade.gameObject);
    }

    private IEnumerator Advice()
    {
        isShowingAdvice = true;
        dropHintUI.SetActive(true);
        nameText.text = "Saca los productos de la heladera";
        yield return new WaitForSeconds(3f);
        isShowingAdvice = false;
        dropHintUI.SetActive(false);
        nameText.text = "";
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

                    AnalyticsManager.Instance.TheftPrevented();

                    if (dailySummary != null)
                        dailySummary.IncrementThievesCaught();
                }
                else
                {
                    currentClient.GetHit();

                    AnalyticsManager.Instance.TheftPrevented();

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
            else if (upgrade != null && upgrade.UpgradeData != null && !boxProduct.IsEmpty)
            {
                productName = upgrade.UpgradeData.Name;

                nameText.text = $"{productName}";
                hintText.text =
                    $"E  para colocar\n" +
                    $"R  para rotar\n" +
                    $"G  para soltar\n";
            }
            else if (boxProduct != null && boxProduct.IsEmpty)
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
                hintText.text = "LMB para interactuar\n" +
                    "F para repickear\n";
            }

            heldObject.transform.SetParent(null);
            heldObject.layer = LayerMask.NameToLayer("Interact");
            heldObject = null;
            heldObjectRb = null;
            heldObjectCollider = null;
            currentInteractable = null;

            if (boxProduct != null && boxProduct.CurrentPreview != null && boxProduct.AllZones.Length > 0)
            {
                boxProduct.CurrentPreview.SetActive(false);
                foreach (PlacementZoneProducts zone in boxProduct.AllZones)
                    zone.HideVisual();
            }

            if (audioSource != null && dropSound != null)
                audioSource.PlayOneShot(dropSound);
        }
    }
    public bool HasBoxInHand()
    {
        return heldObject != null && (boxProduct != null);
    }
}

