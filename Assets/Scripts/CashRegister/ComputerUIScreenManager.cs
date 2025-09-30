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
        if (TutorialContent.Instance.CurrentIndexGuide < 8)
            return;
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
        TutorialContent.Instance.CompleteStep(8);
    }

    public void ShowStoreScreen()
    {
        if (TutorialContent.Instance.CurrentIndexGuide < 6)
            return;
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
        TutorialContent.Instance.CompleteStep(6);
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
        cashRegisterInteraction.PeekClient();
        TutorialContent.Instance.CompleteStep(5);
    }

    public void ShowBackgrounds()
    {
        if (TutorialContent.Instance.CurrentIndexGuide < 9)
            return;
        homeScreen.SetActive(false);
        storeScreen.SetActive(false);
        cashRegisterScreen.SetActive(false);
        priceProducts.SetActive(false);
        backgrounds.SetActive(true);
        upgradesScreen.SetActive(false);
        configScreen.SetActive(false);
        TutorialContent.Instance.CompleteStep(9);
    }

    public void ShowUpgradeScreen()
    {
        if (TutorialContent.Instance.CurrentIndexGuide < 7)
            return;
        homeScreen.SetActive(false);
        storeScreen.SetActive(false);
        cashRegisterScreen.SetActive(false);
        priceProducts.SetActive(false);
        backgrounds.SetActive(false);
        upgradesScreen.SetActive(true);
        configScreen.SetActive(false);
        UpgradeManager upgradeManager = upgradesScreen.GetComponent<UpgradeManager>();
        upgradeManager.PopulateStore();
        TutorialContent.Instance.CompleteStep(7);
    }

    public void ShowConfigScreen()
    {
        if (TutorialContent.Instance.CurrentIndexGuide < 10)
            return;
        homeScreen.SetActive(false);
        storeScreen.SetActive(false);
        cashRegisterScreen.SetActive(false);
        priceProducts.SetActive(false);
        backgrounds.SetActive(false);
        upgradesScreen.SetActive(false);
        configScreen.SetActive(true);
        TutorialContent.Instance.CompleteStep(10);
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
