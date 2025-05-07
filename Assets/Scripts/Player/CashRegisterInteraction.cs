using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CashRegisterInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ClientQueueManager queueManager;
    [SerializeField] Transform cameraTarget;
    [SerializeField] GameObject cashRegisterCanvas;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerCam playerCam;
    [SerializeField] MoveCamera moveCamera;
    [SerializeField] Rigidbody playerRb;

    [Header("Configurations")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private string cashRegisterTag = "CashRegister";
    [SerializeField] private PlayerEconomy playerEconomy;

    [Header("Sounds")]
    [SerializeField] private AudioSource registerAudioSource;
    [SerializeField] private AudioClip registerOpenSound;
    [SerializeField] private AudioClip registerCloseSound;
    [SerializeField] private AudioClip paymentSound;

    private bool inCashRegister = false;
    private bool canClickTheCashRegister = true;

    private Vector3 originalCameraPos;
    private Quaternion originalCameraRot;

    private Camera playerCamera;

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
        cashRegisterCanvas.SetActive(false);
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

        // espacio procesas el pago
        if (inCashRegister && Input.GetKeyDown(KeyCode.Space))
        {
            ProcessPayment();
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

        cashRegisterCanvas.SetActive(true);
        DisplayClientPaymentInfo(); // Mostrar total del cliente al entrar

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

        queueManager.PayText.text = ""; // Limpiar texto al salir
        cashRegisterCanvas.SetActive(false);

        PlayRegisterSound(registerCloseSound);
    }

    void DisplayClientPaymentInfo()
    {
        if (queueManager.ClientQueue.Count > 0)
        {
            Client client = queueManager.ClientQueue.Peek();
            int totalToPay = client.CalculateCartTotal();

            List<int> simulatedPayment = client.TryMakePayment(totalToPay);
            int simulatedTotal = simulatedPayment.Sum();
            int change = simulatedTotal - totalToPay;


            List<ProductInteractable> cart = client.GetCart();
            string cartInfo = "Carrito:\n";

            foreach (var product in cart)
            {
                cartInfo += $"- {product.ProductData.Name} (${product.ProductData.Price})\n";
            }

            queueManager.PayText.text = $"Total: ${totalToPay}\nPago: ${simulatedTotal}\nVuelto: ${change}\n{cartInfo}";
        }
        else
        {
            queueManager.PayText.text = "";
        }
    }


    // obtiene el cliente actual de la cola y calcula total a pagar
    void ProcessPayment()
    {
        if (queueManager.ClientQueue.Count > 0)
        {
            Client client = queueManager.ClientQueue.Peek();
            int totalToPay = client.CalculateCartTotal();

            List<int> clientPayment = client.TryMakePayment(totalToPay);

            // Calcular vuelto
            int change = clientPayment.Sum() - totalToPay;

            if (change > 0)
            {
                Debug.Log($"Cliente pagó ${clientPayment.Sum()} (Vuelto: ${change})");
                if (playerEconomy.TryGiveChange(change))
                {
                    Debug.Log($"Vuelto dado al cliente: ${change}");
                }
                else
                {
                    Debug.LogWarning("No hay suficiente dinero en la caja para dar vuelto.");
                }
            }
            else
            {
                Debug.Log($"Cliente pagó ${clientPayment.Sum()}");
            }

            playerEconomy.ReceivePayment(totalToPay);
            PlayRegisterSound(paymentSound);
            queueManager.RemoveClient();
            DisplayClientPaymentInfo();
        }
    }

    private void PlayRegisterSound(AudioClip clip)
    {
        if (registerAudioSource != null && clip != null)
        {
            registerAudioSource.PlayOneShot(clip);
        }
    }
}