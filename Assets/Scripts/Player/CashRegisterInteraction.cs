using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
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

        cashRegisterCanvas.SetActive(false);

        PlayRegisterSound(registerCloseSound);
    }

    // obtiene el cliente actual de la cola y calcula total a pagar
    void ProcessPayment()
    {
        if (queueManager.ClientQueue.Count > 0)
        {
            Client client = queueManager.ClientQueue.Peek();
            float totalToPay = client.CalculateCartTotal();

            // Simulamos que el cliente paga con un monto aleatorio (mayor o igual al total)
            float clientPayment = GetRandomPaymentAmount(totalToPay);

            // Calcular vuelto
            float change = clientPayment - totalToPay;

            if (change > 0)
            {
                Debug.Log($"Cliente pagó ${clientPayment} (Vuelto: ${change})");
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
                Debug.Log($"Cliente pagó ${clientPayment}");
            }

            playerEconomy.ReceivePayment(totalToPay);
            queueManager.PayText.text = $"Total: ${totalToPay}\nPago: ${clientPayment}\nVuelto: ${change}";
            PlayRegisterSound(paymentSound);
            queueManager.RemoveClient();
            StartCoroutine(HideText());
        }
    }

    // Genera un pago aleatorio (siempre mayor o igual al total)
    private float GetRandomPaymentAmount(float total)
    {
        float minPayment = total;
        float maxPayment = total * 1.5f; // Paga hasta 50% más
        return Mathf.Round(Random.Range(minPayment, maxPayment) / 10) * 10; // Redondea a múltiplos de 10
    }

    private IEnumerator HideText()
    {
        yield return new WaitForSeconds(2f);
        queueManager.PayText.text = "";
    }

    private void PlayRegisterSound(AudioClip clip)
    {
        if (registerAudioSource != null && clip != null)
        {
            registerAudioSource.PlayOneShot(clip);
        }
    }
}