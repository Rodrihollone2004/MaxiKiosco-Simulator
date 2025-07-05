using UnityEngine;

public class ComputerUIScreenManager : MonoBehaviour
{
    [Header("Pantallas")]
    [SerializeField] private GameObject homeScreen;
    [SerializeField] private GameObject storeScreen;
    [SerializeField] private GameObject cashRegisterScreen;
    [SerializeField] private GameObject priceProducts;
    [SerializeField] private CashRegisterInteraction cashRegisterInteraction;

    void Start()
    {
        ShowHomeScreen();
    }

    public void ShowHomeScreen()
    {
        homeScreen.SetActive(true);
        storeScreen.SetActive(false);
        cashRegisterScreen.SetActive(false);
        priceProducts.SetActive(false);
    }

    public void ShowPriceProducts()
    {
        CustomizeProducts customizable = priceProducts.GetComponent<CustomizeProducts>();
        customizable.PopulateStore();
        priceProducts.SetActive(true);
        homeScreen.SetActive(false);
        storeScreen.SetActive(false);
        cashRegisterScreen.SetActive(false);
    }

    public void ShowStoreScreen()
    {
        homeScreen.SetActive(false);
        storeScreen.SetActive(true);
        cashRegisterScreen.SetActive(false);
        priceProducts.SetActive(false);
    }

    public void ShowCashRegisterScreen()
    {
        homeScreen.SetActive(false);
        storeScreen.SetActive(false);
        priceProducts.SetActive(false);
        cashRegisterScreen.SetActive(true);
        cashRegisterInteraction.EnterCashRegisterMode(false, cashRegisterInteraction.LimitedCameraTarget);
    }
}
