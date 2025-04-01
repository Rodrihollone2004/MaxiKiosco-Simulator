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

    private float xRotation;
    private float yRotation;

    public bool IsInCashRegister { get => isInCashRegister; set => isInCashRegister = value; }
    public float Sensitivity { get => sensitivity; set => sensitivity = value; }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


    }

    private void Update()
    {
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
        }
        else
        {
            // movimiento de camara normal
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
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
}
