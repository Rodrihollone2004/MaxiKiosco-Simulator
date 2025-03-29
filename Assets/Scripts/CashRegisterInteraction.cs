using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class CashRegisterInteraction : MonoBehaviour
{
    [SerializeField] ClientQueueManager queueManager;
    [SerializeField] Transform cameraTarget;
    [SerializeField] GameObject cashRegisterCanvas;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerCam playerCam;
    [SerializeField] MoveCamera moveCamera;
    [SerializeField] Rigidbody playerRb;

    private bool inCashRegister = false;
    private bool canClickTheCashRegister = true;

    private Vector3 originalCameraPos;
    private Quaternion originalCameraRot;

    private Camera playerCamera;

    private void Start()
    {
        cashRegisterCanvas.SetActive(false);
        playerCamera = Camera.main;
    }

    private void Update()
    { 
            if (Input.GetMouseButtonDown(0))
            {
                if (canClickTheCashRegister) 
                { 
                    RaycastHit hit;
                    if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 3f))
                    {
                            if (hit.collider.CompareTag("CashRegister"))
                            {
                                EnterCashRegisterMode();
                            }
                    }
                }
            }

        if (inCashRegister && Input.GetKeyDown(KeyCode.Escape))
        {
            ExitCashRegisterMode();
        }

        if (inCashRegister && Input.GetKeyDown(KeyCode.Space))
        {
            ProcessPayment();
        }
    }

    void EnterCashRegisterMode()
    {
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
    }

    void ExitCashRegisterMode()
    {
        inCashRegister = false;
        playerMovement.enabled = true;
        moveCamera.enabled = true;
        canClickTheCashRegister = true;

        playerCamera.transform.position = originalCameraPos;
        playerCamera.transform.rotation = originalCameraRot;

        playerCam.IsInCashRegister = false;

        cashRegisterCanvas.SetActive(false);
    }

    void ProcessPayment()
    {
        if (queueManager.ClientQueue.Count > 0)
        {
            Client client = queueManager.ClientQueue.Peek().GetComponent<Client>();
            float pay = client.CalculateCartTotal();
            Debug.Log("Cliente pago $" + pay);

            queueManager.PayText.text = "Pago: $" + pay;

            queueManager.RemoveClient();

            StartCoroutine(HideText());
        }
    }

    private IEnumerator HideText()
    {
        yield return new WaitForSeconds(2f);
        queueManager.PayText.text = "";
    }
}