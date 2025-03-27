using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashRegisterInteraction : MonoBehaviour
{
    [SerializeField] Transform cameraTarget;
    [SerializeField] GameObject cashRegisterCanvas;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerCam playerCam;
    [SerializeField] MoveCamera moveCamera;
    [SerializeField] Rigidbody playerRb;

    private bool inCashRegister = false;

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
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 3f))
            {
                if (hit.collider.CompareTag("CashRegister"))
                {
                    EnterCashRegisterMode();
                }
            }
        }

        if (inCashRegister && Input.GetKeyDown(KeyCode.Escape))
        {
            ExitCashRegisterMode();
        }
    }

    void EnterCashRegisterMode()
    {
        inCashRegister = true;
        playerMovement.enabled = false;
        playerCam.enabled = false;
        moveCamera.enabled = false;

        playerRb.velocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;

        originalCameraPos = playerCamera.transform.position;
        originalCameraRot = playerCamera.transform.rotation;

        playerCamera.transform.position = cameraTarget.position;
        playerCamera.transform.rotation = cameraTarget.rotation;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        cashRegisterCanvas.SetActive(true);
    }

    void ExitCashRegisterMode()
    {
        inCashRegister = false;
        playerMovement.enabled = true;
        playerCam.enabled = true;
        moveCamera.enabled = true;

        playerCamera.transform.position = originalCameraPos;
        playerCamera.transform.rotation = originalCameraRot;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cashRegisterCanvas.SetActive(false);
    }
}