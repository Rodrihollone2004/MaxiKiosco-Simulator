using UnityEngine;

public class ComputerUIScreenManager : MonoBehaviour
{
    [Header("Pantallas")]
    [SerializeField] private GameObject homeScreen;
    [SerializeField] private GameObject storeScreen;
    [SerializeField] private GameObject cashRegisterScreen;
    [SerializeField] private GameObject priceProducts;
    [SerializeField] private GameObject upgradesScreen;
    [SerializeField] private GameObject configScreen;
    [SerializeField] private GameObject backgrounds;
    [SerializeField] private GameObject bocaBackground;
    [SerializeField] private GameObject riverBackground;
    [SerializeField] private GameObject atlantaBackground;
    [SerializeField] private GameObject velezBackground;
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
        backgrounds.SetActive(false);
        upgradesScreen.SetActive(false);
        configScreen.SetActive(false);
        cashRegisterInteraction.InCashRegister = false;
    }

    public void ShowPriceProducts()
    {
        priceProducts.SetActive(true);
        homeScreen.SetActive(false);
        storeScreen.SetActive(false);
        cashRegisterScreen.SetActive(false);
        backgrounds.SetActive(false);
        cashRegisterInteraction.InCashRegister = false;
        upgradesScreen.SetActive(false);
        configScreen.SetActive(false);
        CustomizeProducts customizable = priceProducts.GetComponent<CustomizeProducts>();
        customizable.PopulateStore();
    }

    public void ShowStoreScreen()
    {
        homeScreen.SetActive(false);
        storeScreen.SetActive(true);
        cashRegisterScreen.SetActive(false);
        priceProducts.SetActive(false);
        backgrounds.SetActive(false);
        upgradesScreen.SetActive(false);
        configScreen.SetActive(false);
        cashRegisterInteraction.InCashRegister = false;
        StoreUI storeUI = storeScreen.GetComponent<StoreUI>();
        storeUI.ButtonType(storeUI.CurrentType);
    }

    public void ShowCashRegisterScreen()
    {
        homeScreen.SetActive(false);
        storeScreen.SetActive(false);
        priceProducts.SetActive(false);
        cashRegisterScreen.SetActive(true);
        backgrounds.SetActive(false);
        upgradesScreen.SetActive(false);
        configScreen.SetActive(false);
        cashRegisterInteraction.InCashRegister = true;
    }

    public void ShowBackgrounds()
    {
        homeScreen.SetActive(false);
        storeScreen.SetActive(false);
        cashRegisterScreen.SetActive(false);
        priceProducts.SetActive(false);
        backgrounds.SetActive(true);
        upgradesScreen.SetActive(false);
        configScreen.SetActive(false);
    }

    public void ShowUpgradeScreen()
    {
        homeScreen.SetActive(false);
        storeScreen.SetActive(false);
        cashRegisterScreen.SetActive(false);
        priceProducts.SetActive(false);
        backgrounds.SetActive(false);
        upgradesScreen.SetActive(true);
        configScreen.SetActive(false);
        UpgradeManager upgradeManager = upgradesScreen.GetComponent<UpgradeManager>();
        upgradeManager.PopulateStore();
    }

    public void ShowConfigScreen()
    {
        homeScreen.SetActive(false);
        storeScreen.SetActive(false);
        cashRegisterScreen.SetActive(false);
        priceProducts.SetActive(false);
        backgrounds.SetActive(false);
        upgradesScreen.SetActive(false);
        configScreen.SetActive(true);
    }

    public void BocaBackground()
    {
        bocaBackground.SetActive(true);
        riverBackground.SetActive(false);
        atlantaBackground.SetActive(false);
        velezBackground.SetActive(false);
    }

    public void RiverBackground()
    {
        bocaBackground.SetActive(false);
        riverBackground.SetActive(true);
        atlantaBackground.SetActive(false);
        velezBackground.SetActive(false);
    }

    public void AtlantaBackground()
    {
        bocaBackground.SetActive(false);
        riverBackground.SetActive(false);
        atlantaBackground.SetActive(true);
        velezBackground.SetActive(false);
    }

    public void VelezBackground()
    {
        bocaBackground.SetActive(false);
        riverBackground.SetActive(false);
        atlantaBackground.SetActive(false);
        velezBackground.SetActive(true);
    }
}
