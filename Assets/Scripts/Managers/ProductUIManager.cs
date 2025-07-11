using System.Collections;
using TMPro;
using UnityEngine;

public class ProductUIManager : MonoBehaviour
{
    public static ProductUIManager Instance { get; private set; }

    [SerializeField] private GameObject hintUI;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text nameText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowInfo(string name, int price, int amount)
    {
        HideInfo();

        nameText.text = $"{name}";
        amountText.text = $"Restantes: {amount}";
        priceText.text = $"Precio: {price}";
        hintUI.SetActive(true);
        Debug.Log($"Interactuando con {name} (${price})");

        StartCoroutine(HideInfoAfterDelay(3));
    }

    public void HideInfo()
    {
        hintUI.SetActive(false);
        StopAllCoroutines();
    }

    private IEnumerator HideInfoAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        hintUI.SetActive(false);
    }
}
