using System.Collections;
using Cinemachine;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [Header("Sensibility")]
    [SerializeField] private float sensitivity;

    [Header("References")]
    [SerializeField] private Transform orientation;

    [Header("Cash Register limits")]
    [SerializeField] private float minAngle = 0;
    [SerializeField] private float maxAngle = 180;
    private bool isInCashRegister = false;
    private bool invertY = false;
    private bool isLocked = false;

    private float xRotation;
    private float yRotation;

    [Header("Zoom")]
    [SerializeField] private float zoomedFieldOfView = 50f;
    private CinemachineVirtualCamera virtualCamera;
    private Camera mainCamera;
    private float normalFieldOfView = 60f;
    private bool isZoomed = false;
    private Coroutine zoomCoroutine;
    public bool IsInCashRegister { get => isInCashRegister; set => isInCashRegister = value; }
    public bool IsLocked { get => isLocked; set => isLocked = value; }

    private bool initializedCashRegisterView;


    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        initializedCashRegisterView = false;
    }

    private void Update()
    {
        if (isLocked)
        {
            if (isZoomed)
                ToggleZoom(false);
            return;
        }
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivity;

        mouseY = invertY ? -mouseY : mouseY;

        yRotation += mouseX;
        xRotation -= mouseY;

        // limita el movimiento de camara si esta en la caja registradora
        if (IsInCashRegister)
        {
            xRotation = Mathf.Clamp(xRotation, -25f, 50f);
            yRotation = Mathf.Clamp(yRotation, minAngle, maxAngle);

            if (!initializedCashRegisterView)
            {
                yRotation = 180;
                initializedCashRegisterView = true;
            }
        }
        else
        {
            // movimiento de camara normal
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            if (Input.GetMouseButton(1))
            {
                if (!isZoomed)
                {
                    ToggleZoom(true);
                }
            }
            else
            {
                if (isZoomed)
                {
                    ToggleZoom(false);
                }
            }


        }

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);

        if (PlayerPrefs.HasKey("masterSen"))
        {
            sensitivity = PlayerPrefs.GetFloat("masterSen");
        }
        if (PlayerPrefs.HasKey("masterInvertY"))
        {
            invertY = PlayerPrefs.GetInt("masterInvertY") == 1;
        }

    }

    public void ToggleZoom(bool zoomIn)
    {
        isZoomed = zoomIn;
        if (zoomCoroutine != null)
        {
            StopCoroutine(zoomCoroutine);
        }
        zoomCoroutine = StartCoroutine(ZoomCoroutine(zoomIn));
    }

    private IEnumerator ZoomCoroutine(bool zoomIn)
    {
        float targetFOV = zoomIn ? zoomedFieldOfView : normalFieldOfView;
        float startFOV = virtualCamera.m_Lens.FieldOfView;
        float elapsed = 0f;

        while (elapsed < 0.2f)
        {
            virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, elapsed / 0.2f);
            mainCamera.fieldOfView = virtualCamera.m_Lens.FieldOfView;
            elapsed += Time.deltaTime;
            yield return null;
        }

        virtualCamera.m_Lens.FieldOfView = targetFOV;
    }
}
