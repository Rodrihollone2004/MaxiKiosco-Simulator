using UnityEngine;
using TMPro;

public class DailySummary : MonoBehaviour
{
    [SerializeField] private GameObject summaryPanel;
    [SerializeField] private TMP_Text summaryText;

    public void ShowSummary(int clientsServed, int moneyEarned)
    {
        summaryText.text = $"Resumen del D�a:\n\n" +
                           $"Clientes atendidos: {clientsServed}\n" +
                           $"Total ganado: ${moneyEarned}";
        summaryPanel.SetActive(true);
    }

    public void HideSummary()
    {
        summaryPanel.SetActive(false);
    }
}
