using UnityEngine;
using TMPro;

public class DailySummary : MonoBehaviour
{
    [SerializeField] private GameObject summaryPanel;
    [SerializeField] private TMP_Text summaryText;
    [SerializeField] private PlayerCam playerCam;

    public void ShowSummary(int clientsServed, int moneyEarned)
    {
        summaryText.text = $"Resumen del Día:\n\n" +
                           $"Clientes atendidos: {clientsServed}\n" +
                           $"Total ganado: ${moneyEarned}";
        summaryPanel.SetActive(true);

        playerCam.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void HideSummary()
    {
        summaryPanel.SetActive(false);

        playerCam.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
