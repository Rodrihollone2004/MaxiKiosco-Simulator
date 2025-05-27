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

    List<int> clientPayment = new();
    private Vector3 originalCameraPos;
    private Quaternion originalCameraRot;

    private Camera playerCamera;

    Client currentClient;
    int change = 0;

    private void Awake()
    {
        if (registerAudioSource == null)
        {
            registerAudioSource = gameObject.AddComponent<AudioSource>();
            registerAudioSource.spatialBlend = 0.8f;
        }
        playerEconomy.onFinishPay += HandlePaymentFinished;
        //Client.onClientWantPay += ProcessPayment;
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
        if (inCashRegister && Input.GetKeyDown(KeyCode.Return) && currentClient != null)
        {
            ProcessPayment(currentClient);
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
        ProcessPayment(currentClient); // Mostrar total del cliente al entrar

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

        ProcessPayment(currentClient);

        PlayRegisterSound(registerCloseSound);
    }

    void ProcessPayment(Client client)
    {
        currentClient = client;
        if (queueManager.ClientQueue.Count > 0)
        {
            client = queueManager.ClientQueue.Peek();
            int totalToPay = client.CalculateCartTotal();

            clientPayment = client.TryMakePayment(totalToPay);
            int simulatedTotal = clientPayment.Sum();

            //calcular vuelto
            change = clientPayment.Sum() - totalToPay;

            List<ProductInteractable> cart = client.GetCart();
            string cartInfo = "Carrito:\n";

            foreach (var product in cart)
            {
                cartInfo += $"- {product.ProductData.Name} (${product.ProductData.Price})\n";
            }

            queueManager.PayText.text = $"Total: ${totalToPay}\nPago: ${simulatedTotal}\nVuelto: ${change}\n{cartInfo}";

            HandlePaymentFinished(0);
        }
        else
        {
            queueManager.PayText.text = "";
        }
    }

    public void HandlePaymentFinished(int vuelto)
    {
        if (vuelto == change)
        {
            playerEconomy.ReceivePayment(clientPayment.Sum());
            NPC_Controller.instance.currentNode = AStarManager.instance.startNode;
            PlayRegisterSound(paymentSound);
            queueManager.RemoveClient();
            change = 0;
            Debug.Log($"vuelto correcto {vuelto}");
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