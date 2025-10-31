using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class CashRegisterInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ClientQueueManager queueManager;
    [SerializeField] Transform limitedCameraTarget;
    [SerializeField] Transform lockedCameraTarget;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerCam virtualPlayerCam;
    [SerializeField] PlayerCam playerCam;
    [SerializeField] Rigidbody playerRb;
    [SerializeField] ComputerUIScreenManager computerUIScreenManager;
    [SerializeField] ExperienceManager experienceManager;
    [SerializeField] GameObject crosshair;
    PlayerInteraction playerInteraction;
    public PlayerEconomy playerEconomy;
    public CashRegisterUI cashRegisterUI;

    [Header("QR Payment")]
    [SerializeField] private QRPaymentHandler qrPaymentHandler;
    private bool isQRPayment = false;

    [Header("Configurations")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private string cashRegisterTag = "CashRegister";

    [Header("Sounds")]
    [SerializeField] private AudioSource registerAudioSource;
    //[SerializeField] private AudioClip registerOpenSound;
    //[SerializeField] private AudioClip registerCloseSound;
    [SerializeField] private AudioClip paymentSound;

    public static event Action onFinishPath;

    public bool InCashRegister { get; set; }
    private bool canClickTheCashRegister = true;

    public List<int> clientPayment = new();
    private Vector3 originalCameraPos;
    private Quaternion originalCameraRot;

    private Vector3 trueOriginalCameraPos;
    private Quaternion trueOriginalCameraRot;
    private bool hasStoredTrueOriginal = false;

    private Camera playerCamera;

    public Client currentClient;
    int change = 0;

    public NPC_Controller nPC_Controller;

    private Coroutine cameraTransitionCoroutine;
    private DayNightCycle dayNightCycle;
    public Transform LockedCameraTarget { get => lockedCameraTarget; set => lockedCameraTarget = value; }
    public Transform LimitedCameraTarget { get => limitedCameraTarget; set => limitedCameraTarget = value; }
    public PlayerCam PlayerCam { get => playerCam; set => playerCam = value; }
    public Camera PlayerCamera { get => playerCamera; set => playerCamera = value; }

    private Quaternion orientationRot;

    private Vector3 startCameraPos;
    private Quaternion startCameraRot;

    private bool isTransitioning = false;

    //private void Awake()
    //{
    //    if (registerAudioSource == null)
    //    {
    //        registerAudioSource = gameObject.AddComponent<AudioSource>();
    //        registerAudioSource.spatialBlend = 0.8f;
    //    }
    //}

    private void Start()
    {
        playerCamera = Camera.main;
        startCameraPos = playerCamera.transform.position;
        startCameraRot = playerCamera.transform.rotation;
        dayNightCycle = FindObjectOfType<DayNightCycle>();
        playerInteraction = GetComponent<PlayerInteraction>();
        NPC_Controller.onShowScreen += () => PeekClient();
    }

    private void Update()
    {
        if (isTransitioning)
            return;

        // metodo para entrar a la caja registradora
        if (Input.GetMouseButtonDown(0) && canClickTheCashRegister)
        {
            TryInteractWithRegister();
        }

        if (TutorialContent.Instance.IsComplete && Input.GetKeyDown(KeyCode.Escape))
        {
            if (InCashRegister)
            {
                InCashRegister = false;
                StartCoroutine(SafeEnterCashRegisterMode(true, lockedCameraTarget));
                computerUIScreenManager.ShowHomeScreen();
            }
            else if (playerCam.IsLocked)
            {
                StartCoroutine(SafeExitCashRegisterMode());
            }
        }

        // enter procesas el pago
        if (InCashRegister && currentClient != null)
        {
            if (nPC_Controller.isInCashRegister && !nPC_Controller.isPaying)
            {
                ProcessPayment(currentClient);
                PeekClient();

                if (queueManager.ClientQueue.Count > 0 && currentClient == queueManager.ClientQueue.Peek())
                {
                    cashRegisterUI.UpdatePaymentText(currentClient, clientPayment, playerEconomy.GetCurrentChange(), nPC_Controller);
                    nPC_Controller.isPaying = true;
                }
            }
            else if (nPC_Controller.isInCashRegister && nPC_Controller.isPaying)
            {
                cashRegisterUI.UpdatePaymentText(currentClient, clientPayment, playerEconomy.GetCurrentChange(), nPC_Controller);
            }
        }

        if (InCashRegister && currentClient != null && Input.GetKeyDown(KeyCode.Return) && nPC_Controller.isInCashRegister)
        {
            if (!isQRPayment)
            {
                if (playerEconomy.GetCurrentChange() == change)
                {
                    ConfirmPayment();
                    PeekClient();
                }
            }
        }
    }

    private void TryInteractWithRegister()
    {
        if (TutorialContent.Instance.CurrentIndexGuide == 3 || !Termica.IsTermicaOn)
            return;

        if (playerInteraction != null && playerInteraction.HasBoxInHand())
        {
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactionDistance))
        {
            if (hit.collider.CompareTag(cashRegisterTag) && playerMovement.State == PlayerMovement.MovementState.idle)
            {
                EnterCashRegisterMode(true, lockedCameraTarget);
                PeekClient(false);
                TutorialContent.Instance.CompleteStep(4);
            }
        }
    }

    // configuracion al entrar a la caja registradora
    public void EnterCashRegisterMode(bool lockCamera, Transform targetPosition)
    {
        if (playerMovement.State == PlayerMovement.MovementState.sprinting || playerMovement.State == PlayerMovement.MovementState.walking || playerMovement.State == PlayerMovement.MovementState.air || playerMovement.State == PlayerMovement.MovementState.crouching) return;
        // desactiva movimiento de jugador, de la camara y mueve la camara a la posicion de la caja, activa la ui y notifica al sistema de camara
        playerMovement.enabled = false;
        canClickTheCashRegister = false;

        playerRb.velocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;

        if (!hasStoredTrueOriginal)
        {
            trueOriginalCameraPos = playerCamera.transform.position;
            trueOriginalCameraRot = playerCamera.transform.rotation;
            hasStoredTrueOriginal = true;
        }

        StartCoroutine(EnterCashRegisterSequence(targetPosition, lockCamera));
    }

    private IEnumerator EnterCashRegisterSequence(Transform targetPosition, bool lockCamera)
    {
        yield return MoveCameraCoroutine(targetPosition, 0.5f, true);

        playerCamera.GetComponent<CinemachineBrain>().enabled = false;

        if (targetPosition == limitedCameraTarget)
            crosshair.SetActive(true);
        else
        {
            playerCamera.transform.rotation = targetPosition.rotation;
            crosshair.SetActive(false);
        }

        virtualPlayerCam.IsInCashRegister = true;
        virtualPlayerCam.IsLocked = true;

        playerCam.IsInCashRegister = true;
        playerCam.IsLocked = lockCamera;

        Cursor.lockState = lockCamera ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = lockCamera;
    }

    // configuracion al salir de la caja registradora
    public void ExitCashRegisterMode()
    {
        InCashRegister = false;
        playerMovement.enabled = true;
        canClickTheCashRegister = true;

        if (playerCamera != null)
        {
            Transform tempTarget = new GameObject("TempCameraTarget").transform;
            tempTarget.position = trueOriginalCameraPos;
            tempTarget.rotation = trueOriginalCameraRot;

            if (!dayNightCycle.sleepPressed)
            {
                MoveCameraSmooth(tempTarget, 0.5f);
            }
            else
            {
                Transform startTarget = new GameObject("StartCameraTarget").transform;
                startTarget.position = startCameraPos;
                startTarget.rotation = startCameraRot;

                dayNightCycle.sleepPressed = false;

                MoveCameraSmooth(startTarget, 0.5f);
                Destroy(startTarget.gameObject, 1f);
            }
            Destroy(tempTarget.gameObject, 1f);
        }

        virtualPlayerCam.IsInCashRegister = false;
        virtualPlayerCam.IsLocked = false;

        playerCam.IsInCashRegister = false;
        playerCam.IsLocked = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        crosshair.SetActive(true);

        if (currentClient != null)
            ProcessPayment(currentClient);

        hasStoredTrueOriginal = false;
    }

    private void ProcessPayment(Client client)
    {
        if (queueManager.ClientQueue.Count == 0) return;

        clientPayment = client.ClientPayment;
        change = Mathf.Max(0, clientPayment.Sum() - client.totalCart);
    }

    public void ProcessQRPayment(Client client, int amount)
    {
        playerEconomy.ReceivePayment(amount);
        PlayRegisterSound(paymentSound);
        onFinishPath?.Invoke();
        cashRegisterUI.ClearText();
        change = 0;
        StartCoroutine(queueManager.RemoveClient());
        queueManager.UpdateQueuePositions();

        if (currentClient.totalCart > 0)
            experienceManager.AddExperience(10);
    }

    private void ConfirmPayment()
    {
        playerEconomy.ReceivePayment(clientPayment.Sum());
        PlayRegisterSound(paymentSound);
        onFinishPath?.Invoke();
        cashRegisterUI.ClearText();
        change = 0;
        StartCoroutine(queueManager.RemoveClient());
        queueManager.UpdateQueuePositions();

        if (currentClient.totalCart > 0)
            experienceManager.AddExperience(10);
    }

    private void PlayRegisterSound(AudioClip clip)
    {
        if (registerAudioSource != null && clip != null)
        {
            registerAudioSource.PlayOneShot(clip);
        }
    }

    public void PeekClient(bool moveCamera = true)
    {
        if (queueManager.ClientQueue.Count > 0)
        {
            currentClient = queueManager.ClientQueue.Peek();
            nPC_Controller = currentClient.GetComponent<NPC_Controller>();
            CashRegisterContext.SetCurrentClient(nPC_Controller);

            isQRPayment = currentClient.paymentMethod == Client.PaymentMethod.QR;

            if (isQRPayment)
            {
                if (moveCamera && InCashRegister)
                {
                    EnterCashRegisterMode(true, lockedCameraTarget);
                    crosshair.SetActive(false);
                }

                qrPaymentHandler.SetupQRPayment(currentClient);
            }
            else
            {
                if (moveCamera && InCashRegister)
                {
                    EnterCashRegisterMode(false, limitedCameraTarget);
                    crosshair.SetActive(true);
                    playerCam.enabled = true;
                }

                qrPaymentHandler.CancelQRPayment();

                nPC_Controller = currentClient.GetComponent<NPC_Controller>();
                CashRegisterContext.SetCurrentClient(nPC_Controller);
                ProcessPayment(currentClient);
            }

            if (queueManager.ClientQueue.Peek() == currentClient)
            {
                CashRegisterInteraction.onFinishPath -= nPC_Controller.BackToStart;
                CashRegisterInteraction.onFinishPath += nPC_Controller.BackToStart;
            }
        }
    }

    private void MoveCameraSmooth(Transform target, float duration, bool forceRotation = true)
    {
        if (cameraTransitionCoroutine != null)
            StopCoroutine(cameraTransitionCoroutine);

        cameraTransitionCoroutine = StartCoroutine(MoveCameraCoroutine(target, duration, forceRotation));
    }

    private IEnumerator MoveCameraCoroutine(Transform target, float duration, bool forceRotation)
    {
        Vector3 startPos = playerCamera.transform.position;
        Quaternion startRot = playerCamera.transform.rotation;

        Vector3 endPos = target.position;
        Quaternion endRot;

        if (forceRotation && target == limitedCameraTarget)
            endRot = Quaternion.Euler(0, 180, 0);
        else
            endRot = target.rotation;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            playerCamera.transform.position = Vector3.Lerp(startPos, endPos, t);
            playerCamera.transform.rotation = Quaternion.Slerp(startRot, endRot, t);

            yield return null;
        }

        playerCamera.transform.position = endPos;
        playerCamera.transform.rotation = endRot;
    }

    private IEnumerator SafeEnterCashRegisterMode(bool lockCamera, Transform target)
    {
        isTransitioning = true;
        EnterCashRegisterMode(lockCamera, target);
        yield return new WaitForSeconds(0.6f); // Espera a que termine la corrutina interna de cámara
        isTransitioning = false;
    }

    private IEnumerator SafeExitCashRegisterMode()
    {
        isTransitioning = true;
        ExitCashRegisterMode();
        yield return new WaitForSeconds(0.6f);
        playerCam.enabled = false;
        playerCamera.GetComponent<CinemachineBrain>().enabled = true;
        isTransitioning = false;
    }
}