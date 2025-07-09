using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CashRegisterInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ClientQueueManager queueManager;
    [SerializeField] Transform limitedCameraTarget;
    [SerializeField] Transform lockedCameraTarget;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerCam playerCam;
    [SerializeField] MoveCamera moveCamera;
    [SerializeField] Rigidbody playerRb;
    [SerializeField] ComputerUIScreenManager computerUIScreenManager;
    [SerializeField] ExperienceManager experienceManager;
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
    [SerializeField] private AudioClip registerOpenSound;
    [SerializeField] private AudioClip registerCloseSound;
    [SerializeField] private AudioClip paymentSound;

    public static event Action onFinishPath;

    private bool inCashRegister = false;
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

    public Transform LockedCameraTarget { get => lockedCameraTarget; set => lockedCameraTarget = value; }
    public Transform LimitedCameraTarget { get => limitedCameraTarget; set => limitedCameraTarget = value; }

    private void Awake()
    {
        if (registerAudioSource == null)
        {
            registerAudioSource = gameObject.AddComponent<AudioSource>();
            registerAudioSource.spatialBlend = 0.8f;
        }
    }

    private void Start()
    {
        playerCamera = Camera.main;
        NPC_Controller.onShowScreen += () => PeekClient();
    }

    private void Update()
    {
        // metodo para entrar a la caja registradora
        if (Input.GetMouseButtonDown(0) && canClickTheCashRegister)
        {
            TryInteractWithRegister();
        }

        // escape salis de la caja registradora
        if ((playerCam.IsLocked) && Input.GetKeyDown(KeyCode.Escape))
        {
            ExitCashRegisterMode();
            computerUIScreenManager.ShowHomeScreen();
        }
        if ((inCashRegister) && Input.GetKeyDown(KeyCode.Escape))
        {
            EnterCashRegisterMode(true, lockedCameraTarget);
            computerUIScreenManager.ShowHomeScreen();
        }

        // enter procesas el pago
        if (inCashRegister && currentClient != null)
        {
            if (nPC_Controller.isInCashRegister)
            {
                cashRegisterUI.UpdatePaymentText(currentClient, clientPayment, playerEconomy.GetCurrentChange(), nPC_Controller);
            }
        }

        if (inCashRegister && currentClient != null && Input.GetKeyDown(KeyCode.Return) && nPC_Controller.isInCashRegister)
        {
            if (isQRPayment)
            {
                qrPaymentHandler.ConfirmQRPayment();
                PeekClient();
            }
            else
            {
                if (playerEconomy.GetCurrentChange() == change)
                {
                    ConfirmPayment();
                    PeekClient();
                }
                else
                {
                    Debug.Log("El vuelto aún no es correcto.");
                }
            }
        }
    }

    private void TryInteractWithRegister()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactionDistance))
        {
            if (hit.collider.CompareTag(cashRegisterTag))
            {
                EnterCashRegisterMode(true, lockedCameraTarget);
                PeekClient(false);
            }
        }
    }

    // configuracion al entrar a la caja registradora
    public void EnterCashRegisterMode(bool lockCamera, Transform targetPosition)
    {
        // desactiva movimiento de jugador, de la camara y mueve la camara a la posicion de la caja, activa la ui y notifica al sistema de camara
        inCashRegister = true;
        playerMovement.enabled = false;
        moveCamera.enabled = false;
        canClickTheCashRegister = false;

        playerRb.velocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;

        if (!hasStoredTrueOriginal)
        {
            trueOriginalCameraPos = playerCamera.transform.position;
            trueOriginalCameraRot = playerCamera.transform.rotation;
            hasStoredTrueOriginal = true;
        }

        originalCameraPos = playerCamera.transform.position;
        originalCameraRot = playerCamera.transform.rotation;

        playerCamera.transform.position = targetPosition.position;
        playerCamera.transform.rotation = targetPosition.rotation;

        playerCam.IsInCashRegister = true;
        playerCam.IsLocked = lockCamera;

        Cursor.lockState = lockCamera ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = lockCamera;

        PlayRegisterSound(registerOpenSound);
    }

    // configuracion al salir de la caja registradora
    public void ExitCashRegisterMode()
    {
        // revierte todos los cambios anteriores, restaura la posicion de la camara y reactiva los controles
        inCashRegister = false;
        playerMovement.enabled = true;
        moveCamera.enabled = true;
        canClickTheCashRegister = true;

        if (playerCamera != null)
        {
            playerCamera.transform.position = trueOriginalCameraPos;
            playerCamera.transform.rotation = trueOriginalCameraRot;
        }

        playerCam.IsInCashRegister = false;
        playerCam.IsLocked = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (currentClient != null)
            ProcessPayment(currentClient);

        cashRegisterUI.UpdatePaymentText(currentClient, clientPayment, playerEconomy.GetCurrentChange(), nPC_Controller);

        PlayRegisterSound(registerCloseSound);

        hasStoredTrueOriginal = false;
    }

    private void ProcessPayment(Client client)
    {
        if (queueManager.ClientQueue.Count == 0) return;

        clientPayment = client.ClientPayment;
        change = clientPayment.Sum() - client.totalCart;
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
        Debug.Log("Pago QR confirmado");
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
        Debug.Log("Pago confirmado manualmente con ENTER.");
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
                if (moveCamera && inCashRegister)
                    EnterCashRegisterMode(true, lockedCameraTarget);

                qrPaymentHandler.SetupQRPayment(currentClient);
                cashRegisterUI.UpdatePaymentText(currentClient, new List<int>(), 0, nPC_Controller);
            }
            else
            {
                if(moveCamera && inCashRegister)
                    EnterCashRegisterMode(false, limitedCameraTarget);

                nPC_Controller = currentClient.GetComponent<NPC_Controller>();
                CashRegisterContext.SetCurrentClient(nPC_Controller);
                ProcessPayment(currentClient);
                cashRegisterUI.UpdatePaymentText(currentClient, clientPayment, playerEconomy.GetCurrentChange(), nPC_Controller);
            }

            if (queueManager.ClientQueue.Peek() == currentClient)
            {
                CashRegisterInteraction.onFinishPath -= nPC_Controller.BackToStart;
                CashRegisterInteraction.onFinishPath += nPC_Controller.BackToStart;
            }
        }
        else
            Debug.LogWarning("No hay clientes en la cola");
    }
}