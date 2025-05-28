using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CashRegisterInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ClientQueueManager queueManager;
    [SerializeField] Transform cameraTarget;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerCam playerCam;
    [SerializeField] MoveCamera moveCamera;
    [SerializeField] Rigidbody playerRb;
    [SerializeField] private PlayerEconomy playerEconomy;
    [SerializeField] private CashRegisterUI cashRegisterUI;

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

    List<int> clientPayment = new();
    private Vector3 originalCameraPos;
    private Quaternion originalCameraRot;

    private Camera playerCamera;

    Client currentClient;
    int change = 0;

    NPC_Controller nPC_Controller;

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
    }

    private void Update()
    {
        // metodo para entrar a la caja registradora
        if (Input.GetMouseButtonDown(0) && canClickTheCashRegister)
        {
            TryInteractWithRegister();
        }

        // escape salis de la caja registradora
        if (inCashRegister && Input.GetKeyDown(KeyCode.Escape))
        {
            ExitCashRegisterMode();
        }

        // enter procesas el pago
        if (inCashRegister && currentClient != null)
        {
            if (nPC_Controller.isInCashRegister)
            {
                cashRegisterUI.UpdatePaymentText(currentClient, clientPayment, playerEconomy.GetCurrentChange(), nPC_Controller);
                ProcessPayment(currentClient);
            }
        }

        if (inCashRegister && currentClient != null && Input.GetKeyDown(KeyCode.Return) && nPC_Controller.isInCashRegister)
        {
            if (playerEconomy.GetCurrentChange() == change)
            {
                ConfirmPayment();
            }
            else
            {
                Debug.Log("El vuelto aún no es correcto.");
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
                EnterCashRegisterMode();
            }
        }
    }

    // configuracion al entrar a la caja registradora
    void EnterCashRegisterMode()
    {
        // desactiva movimiento de jugador, de la camara y mueve la camara a la posicion de la caja, activa la ui y notifica al sistema de camara
        inCashRegister = true;
        playerMovement.enabled = false;
        moveCamera.enabled = false;
        canClickTheCashRegister = false;

        playerRb.velocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;

        originalCameraPos = playerCamera.transform.position;
        originalCameraRot = playerCamera.transform.rotation;

        playerCamera.transform.position = cameraTarget.position;
        playerCamera.transform.rotation = cameraTarget.rotation;

        playerCam.IsInCashRegister = true;

        currentClient = queueManager.ClientQueue.Peek();
        nPC_Controller = currentClient.GetComponent<NPC_Controller>();
        CashRegisterContext.SetCurrentClient(nPC_Controller);
        ProcessPayment(currentClient); // Mostrar total del cliente al entrar
        cashRegisterUI.UpdatePaymentText(currentClient, clientPayment, playerEconomy.GetCurrentChange(), nPC_Controller);

        PlayRegisterSound(registerOpenSound);
    }

    // configuracion al salir de la caja registradora
    void ExitCashRegisterMode()
    {
        // revierte todos los cambios anteriores, restaura la posicion de la camara y reactiva los controles
        inCashRegister = false;
        playerMovement.enabled = true;
        moveCamera.enabled = true;
        canClickTheCashRegister = true;

        playerCamera.transform.position = originalCameraPos;
        playerCamera.transform.rotation = originalCameraRot;

        playerCam.IsInCashRegister = false;

        ProcessPayment(currentClient);
        cashRegisterUI.UpdatePaymentText(currentClient, clientPayment, playerEconomy.GetCurrentChange(), nPC_Controller);

        PlayRegisterSound(registerCloseSound);
    }

    private void ProcessPayment(Client client)
    {
        if (queueManager.ClientQueue.Count == 0) return;

        currentClient = client;
        int totalToPay = client.CalculateCartTotal();

        clientPayment = client.TryMakePayment(totalToPay);
        change = clientPayment.Sum() - totalToPay;
    }

    private void ConfirmPayment()
    {
        playerEconomy.ReceivePayment(clientPayment.Sum());
        PlayRegisterSound(paymentSound);
        onFinishPath?.Invoke();
        cashRegisterUI.ClearText();
        change = 0;
        Debug.Log("Pago confirmado manualmente con ENTER.");
    }

    private void PlayRegisterSound(AudioClip clip)
    {
        if (registerAudioSource != null && clip != null)
        {
            registerAudioSource.PlayOneShot(clip);
        }
    }
}