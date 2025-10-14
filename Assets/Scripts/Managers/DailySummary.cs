using TMPro;
using UnityEngine;

public class DailySummary : MonoBehaviour
{
    [SerializeField] private GameObject summaryPanel;
    [SerializeField] private TMP_Text summaryText;
    [SerializeField] private PlayerCam playerCam;
    [SerializeField] private PlayerMovement playerMov;

    private int trashCleanedToday = 0;
    private int thievesCaughtToday = 0;
    private int boxesThrownAwayToday = 0;
    private int productsSoldToday = 0;

    public void ShowSummary(int clientsServed, int moneyEarned, int productsSold = 0)
    {
        productsSoldToday = productsSold;

        summaryText.text = $"Resumen del Día:\n\n" +
                           $"Clientes atendidos: {clientsServed}\n" +
                           $"Total ganado: ${moneyEarned}\n" +
                           $"Productos vendidos: {productsSoldToday}\n" +
                           $"Basura limpiada: {trashCleanedToday}\n" +
                           $"Ladrones atrapados: {thievesCaughtToday}\n" +
                           $"Cajas tiradas: {boxesThrownAwayToday}\n";
                          
        summaryPanel.SetActive(true);

        playerCam.enabled = false;
    }
    private void LateUpdate()
    {
        if (summaryPanel.activeSelf)
        {
            playerMov.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void IncrementTrashCleaned()
    {
        trashCleanedToday++;
    }

    public void IncrementThievesCaught()
    {
        thievesCaughtToday++;
    }

    public void IncrementBoxesThrownAway()
    {
        boxesThrownAwayToday++;
    }

    public void IncrementProductsSold(int amount = 1)
    {
        productsSoldToday += amount;
    }

    public void HideSummary()
    {
        summaryPanel.SetActive(false);

        playerCam.enabled = true;
        playerMov.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ResetDailyStats();
    }
    private void ResetDailyStats()
    {
        trashCleanedToday = 0;
        thievesCaughtToday = 0;
        boxesThrownAwayToday = 0;
        productsSoldToday = 0;
    }
}
