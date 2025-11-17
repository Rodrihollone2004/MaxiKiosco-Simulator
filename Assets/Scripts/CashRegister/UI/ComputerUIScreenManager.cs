using UnityEngine;

public class ComputerUIScreenManager : MonoBehaviour
{
    [Header("Pantallas")]
    [SerializeField] private GameObject homeScreen;
    [SerializeField] private GameObject changeColorScreen;
    [SerializeField] private GameObject storeScreen;
    [SerializeField] private GameObject cashRegisterScreen;
    [SerializeField] private GameObject priceProducts;
    [SerializeField] private GameObject upgradesScreen;
    [SerializeField] private GameObject configScreen;
    [SerializeField] private GameObject backgrounds;
    [SerializeField] private GameObject defaultBackground;
    [SerializeField] private GameObject bocaBackground;
    [SerializeField] private GameObject riverBackground;
    [SerializeField] private GameObject atlantaBackground;
    [SerializeField] private GameObject velezBackground;
    [SerializeField] private GameObject customBackground;
    [SerializeField] private GameObject atardecerNubesRosasBackground;
    [SerializeField] private GameObject bonsaiPaisajeRosaBackground;
    [SerializeField] private GameObject ciudadCyberpunkBackground;
    [SerializeField] private GameObject carreteraAnimeBackground;
    [SerializeField] private GameObject islaTropicalBackground;
    [SerializeField] private GameObject paisajeAnimeBackground;
    [SerializeField] private GameObject manchasBlancasCafeBackground;
    [SerializeField] private GameObject manchasVerdesBackground;
    [SerializeField] private GameObject messiSeleccionBackground;
    [SerializeField] private GameObject montañasNevadasBackground;
    [SerializeField] private GameObject olasBackground;
    [SerializeField] private GameObject pinosEnBosqueBackground;
    [SerializeField] private GameObject pinturaEnOleoBackground;
    [SerializeField] private GameObject pinturaEnVidrioBackground;
    [SerializeField] private GameObject pinturaNeonBackground;
    [SerializeField] private GameObject pulpoBackground;
    [SerializeField] private GameObject montañasNeonBackground;
    [SerializeField] private GameObject argentinaMundialBackground;
    [SerializeField] private CashRegisterInteraction cashRegisterInteraction;

    void Start()
    {
        ShowHomeScreen();
    }

    public void ShowHomeScreen()
    {
        homeScreen.SetActive(true);
        changeColorScreen.SetActive(false);
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
        changeColorScreen.SetActive(false);
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
        changeColorScreen.SetActive(false);
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
        changeColorScreen.SetActive(false);
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
        changeColorScreen.SetActive(false);
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
        changeColorScreen.SetActive(false);
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
        if (TutorialContent.Instance.CurrentIndexGuide < 10)
            return;
        homeScreen.SetActive(false);
        changeColorScreen.SetActive(false);
        storeScreen.SetActive(false);
        cashRegisterScreen.SetActive(false);
        priceProducts.SetActive(false);
        backgrounds.SetActive(false);
        upgradesScreen.SetActive(false);
        configScreen.SetActive(true);
        TutorialContent.Instance.CompleteStep(10);
    }

    public void ShowChangeColorScreen()
    {
        homeScreen.SetActive(false);
        changeColorScreen.SetActive(true);
        storeScreen.SetActive(false);
        cashRegisterScreen.SetActive(false);
        priceProducts.SetActive(false);
        backgrounds.SetActive(false);
        upgradesScreen.SetActive(false);
        configScreen.SetActive(false);
    }

    public void BocaBackground()
    {
        bocaBackground.SetActive(true);
        riverBackground.SetActive(false);
        atlantaBackground.SetActive(false);
        velezBackground.SetActive(false);
        defaultBackground.SetActive(false);
        atardecerNubesRosasBackground.SetActive(false);
        bonsaiPaisajeRosaBackground.SetActive(false);
        ciudadCyberpunkBackground.SetActive(false);
        carreteraAnimeBackground.SetActive(false);
        islaTropicalBackground.SetActive(false);
        paisajeAnimeBackground.SetActive(false);
        manchasBlancasCafeBackground.SetActive(false);
        manchasVerdesBackground.SetActive(false);
        messiSeleccionBackground.SetActive(false);
        montañasNevadasBackground.SetActive(false);
        olasBackground.SetActive(false);
        pinosEnBosqueBackground.SetActive(false);
        pinturaEnOleoBackground.SetActive(false);
        pinturaEnVidrioBackground.SetActive(false);
        pinturaNeonBackground.SetActive(false);
        pulpoBackground.SetActive(false);
        montañasNeonBackground.SetActive(false);
        argentinaMundialBackground.SetActive(false);
    }

    public void RiverBackground()
    {
        bocaBackground.SetActive(false);
        riverBackground.SetActive(true);
        atlantaBackground.SetActive(false);
        velezBackground.SetActive(false);
        defaultBackground.SetActive(false);
        atardecerNubesRosasBackground.SetActive(false);
        bonsaiPaisajeRosaBackground.SetActive(false);
        ciudadCyberpunkBackground.SetActive(false);
        carreteraAnimeBackground.SetActive(false);
        islaTropicalBackground.SetActive(false);
        paisajeAnimeBackground.SetActive(false);
        manchasBlancasCafeBackground.SetActive(false);
        manchasVerdesBackground.SetActive(false);
        messiSeleccionBackground.SetActive(false);
        montañasNevadasBackground.SetActive(false);
        olasBackground.SetActive(false);
        pinosEnBosqueBackground.SetActive(false);
        pinturaEnOleoBackground.SetActive(false);
        pinturaEnVidrioBackground.SetActive(false);
        pinturaNeonBackground.SetActive(false);
        pulpoBackground.SetActive(false);
        montañasNeonBackground.SetActive(false);
        argentinaMundialBackground.SetActive(false);
    }

    public void AtlantaBackground()
    {
        bocaBackground.SetActive(false);
        riverBackground.SetActive(false);
        atlantaBackground.SetActive(true);
        velezBackground.SetActive(false);
        defaultBackground.SetActive(false);
        atardecerNubesRosasBackground.SetActive(false);
        bonsaiPaisajeRosaBackground.SetActive(false);
        ciudadCyberpunkBackground.SetActive(false);
        carreteraAnimeBackground.SetActive(false);
        islaTropicalBackground.SetActive(false);
        paisajeAnimeBackground.SetActive(false);
        manchasBlancasCafeBackground.SetActive(false);
        manchasVerdesBackground.SetActive(false);
        messiSeleccionBackground.SetActive(false);
        montañasNevadasBackground.SetActive(false);
        olasBackground.SetActive(false);
        pinosEnBosqueBackground.SetActive(false);
        pinturaEnOleoBackground.SetActive(false);
        pinturaEnVidrioBackground.SetActive(false);
        pinturaNeonBackground.SetActive(false);
        pulpoBackground.SetActive(false);
        montañasNeonBackground.SetActive(false);
        argentinaMundialBackground.SetActive(false);
    }

    public void VelezBackground()
    {
        bocaBackground.SetActive(false);
        riverBackground.SetActive(false);
        atlantaBackground.SetActive(false);
        velezBackground.SetActive(true);
        defaultBackground.SetActive(false);
        customBackground.SetActive(true);
        atardecerNubesRosasBackground.SetActive(false);
        bonsaiPaisajeRosaBackground.SetActive(false);
        ciudadCyberpunkBackground.SetActive(false);
        carreteraAnimeBackground.SetActive(false);
        islaTropicalBackground.SetActive(false);
        paisajeAnimeBackground.SetActive(false);
        manchasBlancasCafeBackground.SetActive(false);
        manchasVerdesBackground.SetActive(false);
        messiSeleccionBackground.SetActive(false);
        montañasNevadasBackground.SetActive(false);
        olasBackground.SetActive(false);
        pinosEnBosqueBackground.SetActive(false);
        pinturaEnOleoBackground.SetActive(false);
        pinturaEnVidrioBackground.SetActive(false);
        pinturaNeonBackground.SetActive(false);
        pulpoBackground.SetActive(false);
        montañasNeonBackground.SetActive(false);
        argentinaMundialBackground.SetActive(false);
    }

    public void CustomBackground()
    {
        bocaBackground.SetActive(false);
        riverBackground.SetActive(false);
        atlantaBackground.SetActive(false);
        velezBackground.SetActive(false);
        defaultBackground.SetActive(false);
        customBackground.SetActive(true);
        atardecerNubesRosasBackground.SetActive(false);
        bonsaiPaisajeRosaBackground.SetActive(false);
        ciudadCyberpunkBackground.SetActive(false);
        carreteraAnimeBackground.SetActive(false);
        islaTropicalBackground.SetActive(false);
        paisajeAnimeBackground.SetActive(false);
        manchasBlancasCafeBackground.SetActive(false);
        manchasVerdesBackground.SetActive(false);
        messiSeleccionBackground.SetActive(false);
        montañasNevadasBackground.SetActive(false);
        olasBackground.SetActive(false);
        pinosEnBosqueBackground.SetActive(false);
        pinturaEnOleoBackground.SetActive(false);
        pinturaEnVidrioBackground.SetActive(false);
        pinturaNeonBackground.SetActive(false);
        pulpoBackground.SetActive(false);
        montañasNeonBackground.SetActive(false);
        argentinaMundialBackground.SetActive(false);
    }

    public void AtardecerNubesRosasBackground()
    {
        bocaBackground.SetActive(false);
        riverBackground.SetActive(false);
        atlantaBackground.SetActive(false);
        velezBackground.SetActive(false);
        defaultBackground.SetActive(false);
        atardecerNubesRosasBackground.SetActive(true);
        bonsaiPaisajeRosaBackground.SetActive(false);
        ciudadCyberpunkBackground.SetActive(false);
        carreteraAnimeBackground.SetActive(false);
        islaTropicalBackground.SetActive(false);
        paisajeAnimeBackground.SetActive(false);
        manchasBlancasCafeBackground.SetActive(false);
        manchasVerdesBackground.SetActive(false);
        messiSeleccionBackground.SetActive(false);
        montañasNevadasBackground.SetActive(false);
        olasBackground.SetActive(false);
        pinosEnBosqueBackground.SetActive(false);
        pinturaEnOleoBackground.SetActive(false);
        pinturaEnVidrioBackground.SetActive(false);
        pinturaNeonBackground.SetActive(false);
        pulpoBackground.SetActive(false);
        montañasNeonBackground.SetActive(false);
        argentinaMundialBackground.SetActive(false);
    }

    public void BonsaiPaisajeRosaBackground()
    {
        bocaBackground.SetActive(false);
        riverBackground.SetActive(false);
        atlantaBackground.SetActive(false);
        velezBackground.SetActive(false);
        defaultBackground.SetActive(false);
        atardecerNubesRosasBackground.SetActive(false);
        bonsaiPaisajeRosaBackground.SetActive(true);
        ciudadCyberpunkBackground.SetActive(false);
        carreteraAnimeBackground.SetActive(false);
        islaTropicalBackground.SetActive(false);
        paisajeAnimeBackground.SetActive(false);
        manchasBlancasCafeBackground.SetActive(false);
        manchasVerdesBackground.SetActive(false);
        messiSeleccionBackground.SetActive(false);
        montañasNevadasBackground.SetActive(false);
        olasBackground.SetActive(false);
        pinosEnBosqueBackground.SetActive(false);
        pinturaEnOleoBackground.SetActive(false);
        pinturaEnVidrioBackground.SetActive(false);
        pinturaNeonBackground.SetActive(false);
        pulpoBackground.SetActive(false);
        montañasNeonBackground.SetActive(false);
        argentinaMundialBackground.SetActive(false);
    }

    public void CiudadCyberpunkBackground()
    {
        bocaBackground.SetActive(false);
        riverBackground.SetActive(false);
        atlantaBackground.SetActive(false);
        velezBackground.SetActive(false);
        defaultBackground.SetActive(false);
        atardecerNubesRosasBackground.SetActive(false);
        bonsaiPaisajeRosaBackground.SetActive(false);
        ciudadCyberpunkBackground.SetActive(true);
        carreteraAnimeBackground.SetActive(false);
        islaTropicalBackground.SetActive(false);
        paisajeAnimeBackground.SetActive(false);
        manchasBlancasCafeBackground.SetActive(false);
        manchasVerdesBackground.SetActive(false);
        messiSeleccionBackground.SetActive(false);
        montañasNevadasBackground.SetActive(false);
        olasBackground.SetActive(false);
        pinosEnBosqueBackground.SetActive(false);
        pinturaEnOleoBackground.SetActive(false);
        pinturaEnVidrioBackground.SetActive(false);
        pinturaNeonBackground.SetActive(false);
        pulpoBackground.SetActive(false);
        montañasNeonBackground.SetActive(false);
        argentinaMundialBackground.SetActive(false);
    }

    public void CarreteraAnimeBackground()
    {
        bocaBackground.SetActive(false);
        riverBackground.SetActive(false);
        atlantaBackground.SetActive(false);
        velezBackground.SetActive(false);
        defaultBackground.SetActive(false);
        atardecerNubesRosasBackground.SetActive(false);
        bonsaiPaisajeRosaBackground.SetActive(false);
        ciudadCyberpunkBackground.SetActive(false);
        carreteraAnimeBackground.SetActive(true);
        islaTropicalBackground.SetActive(false);
        paisajeAnimeBackground.SetActive(false);
        manchasBlancasCafeBackground.SetActive(false);
        manchasVerdesBackground.SetActive(false);
        messiSeleccionBackground.SetActive(false);
        montañasNevadasBackground.SetActive(false);
        olasBackground.SetActive(false);
        pinosEnBosqueBackground.SetActive(false);
        pinturaEnOleoBackground.SetActive(false);
        pinturaEnVidrioBackground.SetActive(false);
        pinturaNeonBackground.SetActive(false);
        pulpoBackground.SetActive(false);
        montañasNeonBackground.SetActive(false);
        argentinaMundialBackground.SetActive(false);
    }

    public void IslaTropicalBackground()
    {
        bocaBackground.SetActive(false);
        riverBackground.SetActive(false);
        atlantaBackground.SetActive(false);
        velezBackground.SetActive(false);
        defaultBackground.SetActive(false);
        atardecerNubesRosasBackground.SetActive(false);
        bonsaiPaisajeRosaBackground.SetActive(false);
        ciudadCyberpunkBackground.SetActive(false);
        carreteraAnimeBackground.SetActive(false);
        islaTropicalBackground.SetActive(true);
        paisajeAnimeBackground.SetActive(false);
        manchasBlancasCafeBackground.SetActive(false);
        manchasVerdesBackground.SetActive(false);
        messiSeleccionBackground.SetActive(false);
        montañasNevadasBackground.SetActive(false);
        olasBackground.SetActive(false);
        pinosEnBosqueBackground.SetActive(false);
        pinturaEnOleoBackground.SetActive(false);
        pinturaEnVidrioBackground.SetActive(false);
        pinturaNeonBackground.SetActive(false);
        pulpoBackground.SetActive(false);
        montañasNeonBackground.SetActive(false);
        argentinaMundialBackground.SetActive(false);
    }

    public void PaisajeAnimeBackground()
    {
        bocaBackground.SetActive(false);
        riverBackground.SetActive(false);
        atlantaBackground.SetActive(false);
        velezBackground.SetActive(false);
        defaultBackground.SetActive(false);
        atardecerNubesRosasBackground.SetActive(false);
        bonsaiPaisajeRosaBackground.SetActive(false);
        ciudadCyberpunkBackground.SetActive(false);
        carreteraAnimeBackground.SetActive(false);
        islaTropicalBackground.SetActive(false);
        paisajeAnimeBackground.SetActive(true);
        manchasBlancasCafeBackground.SetActive(false);
        manchasVerdesBackground.SetActive(false);
        messiSeleccionBackground.SetActive(false);
        montañasNevadasBackground.SetActive(false);
        olasBackground.SetActive(false);
        pinosEnBosqueBackground.SetActive(false);
        pinturaEnOleoBackground.SetActive(false);
        pinturaEnVidrioBackground.SetActive(false);
        pinturaNeonBackground.SetActive(false);
        pulpoBackground.SetActive(false);
        montañasNeonBackground.SetActive(false);
        argentinaMundialBackground.SetActive(false);
    }

    public void ManchasBlancasCafeBackground()
    {
        bocaBackground.SetActive(false);
        riverBackground.SetActive(false);
        atlantaBackground.SetActive(false);
        velezBackground.SetActive(false);
        defaultBackground.SetActive(false);
        atardecerNubesRosasBackground.SetActive(false);
        bonsaiPaisajeRosaBackground.SetActive(false);
        ciudadCyberpunkBackground.SetActive(false);
        carreteraAnimeBackground.SetActive(false);
        islaTropicalBackground.SetActive(false);
        paisajeAnimeBackground.SetActive(false);
        manchasBlancasCafeBackground.SetActive(true);
        manchasVerdesBackground.SetActive(false);
        messiSeleccionBackground.SetActive(false);
        montañasNevadasBackground.SetActive(false);
        olasBackground.SetActive(false);
        pinosEnBosqueBackground.SetActive(false);
        pinturaEnOleoBackground.SetActive(false);
        pinturaEnVidrioBackground.SetActive(false);
        pinturaNeonBackground.SetActive(false);
        pulpoBackground.SetActive(false);
        montañasNeonBackground.SetActive(false);
        argentinaMundialBackground.SetActive(false);
    }

    public void ManchasVerdesBackground()
    {
        bocaBackground.SetActive(false);
        riverBackground.SetActive(false);
        atlantaBackground.SetActive(false);
        velezBackground.SetActive(false);
        defaultBackground.SetActive(false);
        atardecerNubesRosasBackground.SetActive(false);
        bonsaiPaisajeRosaBackground.SetActive(false);
        ciudadCyberpunkBackground.SetActive(false);
        carreteraAnimeBackground.SetActive(false);
        islaTropicalBackground.SetActive(false);
        paisajeAnimeBackground.SetActive(false);
        manchasBlancasCafeBackground.SetActive(false);
        manchasVerdesBackground.SetActive(true);
        messiSeleccionBackground.SetActive(false);
        montañasNevadasBackground.SetActive(false);
        olasBackground.SetActive(false);
        pinosEnBosqueBackground.SetActive(false);
        pinturaEnOleoBackground.SetActive(false);
        pinturaEnVidrioBackground.SetActive(false);
        pinturaNeonBackground.SetActive(false);
        pulpoBackground.SetActive(false);
        montañasNeonBackground.SetActive(false);
        argentinaMundialBackground.SetActive(false);
    }

    public void MessiSeleccionBackground()
    {
        bocaBackground.SetActive(false);
        riverBackground.SetActive(false);
        atlantaBackground.SetActive(false);
        velezBackground.SetActive(false);
        defaultBackground.SetActive(false);
        atardecerNubesRosasBackground.SetActive(false);
        bonsaiPaisajeRosaBackground.SetActive(false);
        ciudadCyberpunkBackground.SetActive(false);
        carreteraAnimeBackground.SetActive(false);
        islaTropicalBackground.SetActive(false);
        paisajeAnimeBackground.SetActive(false);
        manchasBlancasCafeBackground.SetActive(false);
        manchasVerdesBackground.SetActive(false);
        messiSeleccionBackground.SetActive(true);
        montañasNevadasBackground.SetActive(false);
        olasBackground.SetActive(false);
        pinosEnBosqueBackground.SetActive(false);
        pinturaEnOleoBackground.SetActive(false);
        pinturaEnVidrioBackground.SetActive(false);
        pinturaNeonBackground.SetActive(false);
        pulpoBackground.SetActive(false);
        montañasNeonBackground.SetActive(false);
        argentinaMundialBackground.SetActive(false);
    }

    public void MontañasNevadasBackground()
    {
        bocaBackground.SetActive(false);
        riverBackground.SetActive(false);
        atlantaBackground.SetActive(false);
        velezBackground.SetActive(false);
        defaultBackground.SetActive(false);
        atardecerNubesRosasBackground.SetActive(false);
        bonsaiPaisajeRosaBackground.SetActive(false);
        ciudadCyberpunkBackground.SetActive(false);
        carreteraAnimeBackground.SetActive(false);
        islaTropicalBackground.SetActive(false);
        paisajeAnimeBackground.SetActive(false);
        manchasBlancasCafeBackground.SetActive(false);
        manchasVerdesBackground.SetActive(false);
        messiSeleccionBackground.SetActive(false);
        montañasNevadasBackground.SetActive(true);
        olasBackground.SetActive(false);
        pinosEnBosqueBackground.SetActive(false);
        pinturaEnOleoBackground.SetActive(false);
        pinturaEnVidrioBackground.SetActive(false);
        pinturaNeonBackground.SetActive(false);
        pulpoBackground.SetActive(false);
        montañasNeonBackground.SetActive(false);
        argentinaMundialBackground.SetActive(false);
    }

    public void OlasBackground()
    {
        bocaBackground.SetActive(false);
        riverBackground.SetActive(false);
        atlantaBackground.SetActive(false);
        velezBackground.SetActive(false);
        defaultBackground.SetActive(false);
        atardecerNubesRosasBackground.SetActive(false);
        bonsaiPaisajeRosaBackground.SetActive(false);
        ciudadCyberpunkBackground.SetActive(false);
        carreteraAnimeBackground.SetActive(false);
        islaTropicalBackground.SetActive(false);
        paisajeAnimeBackground.SetActive(false);
        manchasBlancasCafeBackground.SetActive(false);
        manchasVerdesBackground.SetActive(false);
        messiSeleccionBackground.SetActive(false);
        montañasNevadasBackground.SetActive(false);
        olasBackground.SetActive(true);
        pinosEnBosqueBackground.SetActive(false);
        pinturaEnOleoBackground.SetActive(false);
        pinturaEnVidrioBackground.SetActive(false);
        pinturaNeonBackground.SetActive(false);
        pulpoBackground.SetActive(false);
        montañasNeonBackground.SetActive(false);
        argentinaMundialBackground.SetActive(false);
    }

    public void PinosEnBosqueBackground()
    {
        bocaBackground.SetActive(false);
        riverBackground.SetActive(false);
        atlantaBackground.SetActive(false);
        velezBackground.SetActive(false);
        defaultBackground.SetActive(false);
        atardecerNubesRosasBackground.SetActive(false);
        bonsaiPaisajeRosaBackground.SetActive(false);
        ciudadCyberpunkBackground.SetActive(false);
        carreteraAnimeBackground.SetActive(false);
        islaTropicalBackground.SetActive(false);
        paisajeAnimeBackground.SetActive(false);
        manchasBlancasCafeBackground.SetActive(false);
        manchasVerdesBackground.SetActive(false);
        messiSeleccionBackground.SetActive(false);
        montañasNevadasBackground.SetActive(false);
        olasBackground.SetActive(false);
        pinosEnBosqueBackground.SetActive(true);
        pinturaEnOleoBackground.SetActive(false);
        pinturaEnVidrioBackground.SetActive(false);
        pinturaNeonBackground.SetActive(false);
        pulpoBackground.SetActive(false);
        montañasNeonBackground.SetActive(false);
        argentinaMundialBackground.SetActive(false);
    }

    public void PinturaEnOleoBackground()
    {
        bocaBackground.SetActive(false);
        riverBackground.SetActive(false);
        atlantaBackground.SetActive(false);
        velezBackground.SetActive(false);
        defaultBackground.SetActive(false);
        atardecerNubesRosasBackground.SetActive(false);
        bonsaiPaisajeRosaBackground.SetActive(false);
        ciudadCyberpunkBackground.SetActive(false);
        carreteraAnimeBackground.SetActive(false);
        islaTropicalBackground.SetActive(false);
        paisajeAnimeBackground.SetActive(false);
        manchasBlancasCafeBackground.SetActive(false);
        manchasVerdesBackground.SetActive(false);
        messiSeleccionBackground.SetActive(false);
        montañasNevadasBackground.SetActive(false);
        olasBackground.SetActive(false);
        pinosEnBosqueBackground.SetActive(false);
        pinturaEnOleoBackground.SetActive(true);
        pinturaEnVidrioBackground.SetActive(false);
        pinturaNeonBackground.SetActive(false);
        pulpoBackground.SetActive(false);
        montañasNeonBackground.SetActive(false);
        argentinaMundialBackground.SetActive(false);
    }

    public void PinturaEnVidrioBackground()
    {
        bocaBackground.SetActive(false);
        riverBackground.SetActive(false);
        atlantaBackground.SetActive(false);
        velezBackground.SetActive(false);
        defaultBackground.SetActive(false);
        atardecerNubesRosasBackground.SetActive(false);
        bonsaiPaisajeRosaBackground.SetActive(false);
        ciudadCyberpunkBackground.SetActive(false);
        carreteraAnimeBackground.SetActive(false);
        islaTropicalBackground.SetActive(false);
        paisajeAnimeBackground.SetActive(false);
        manchasBlancasCafeBackground.SetActive(false);
        manchasVerdesBackground.SetActive(false);
        messiSeleccionBackground.SetActive(false);
        montañasNevadasBackground.SetActive(false);
        olasBackground.SetActive(false);
        pinosEnBosqueBackground.SetActive(false);
        pinturaEnOleoBackground.SetActive(false);
        pinturaEnVidrioBackground.SetActive(true);
        pinturaNeonBackground.SetActive(false);
        pulpoBackground.SetActive(false);
        montañasNeonBackground.SetActive(false);
        argentinaMundialBackground.SetActive(false);
    }

    public void PinturaNeonBackground()
    {
        bocaBackground.SetActive(false);
        riverBackground.SetActive(false);
        atlantaBackground.SetActive(false);
        velezBackground.SetActive(false);
        defaultBackground.SetActive(false);
        atardecerNubesRosasBackground.SetActive(false);
        bonsaiPaisajeRosaBackground.SetActive(false);
        ciudadCyberpunkBackground.SetActive(false);
        carreteraAnimeBackground.SetActive(false);
        islaTropicalBackground.SetActive(false);
        paisajeAnimeBackground.SetActive(false);
        manchasBlancasCafeBackground.SetActive(false);
        manchasVerdesBackground.SetActive(false);
        messiSeleccionBackground.SetActive(false);
        montañasNevadasBackground.SetActive(false);
        olasBackground.SetActive(false);
        pinosEnBosqueBackground.SetActive(false);
        pinturaEnOleoBackground.SetActive(false);
        pinturaEnVidrioBackground.SetActive(false);
        pinturaNeonBackground.SetActive(true);
        pulpoBackground.SetActive(false);
        montañasNeonBackground.SetActive(false);
        argentinaMundialBackground.SetActive(false);
    }

    public void PulpoBackground()
    {
        bocaBackground.SetActive(false);
        riverBackground.SetActive(false);
        atlantaBackground.SetActive(false);
        velezBackground.SetActive(false);
        defaultBackground.SetActive(false);
        atardecerNubesRosasBackground.SetActive(false);
        bonsaiPaisajeRosaBackground.SetActive(false);
        ciudadCyberpunkBackground.SetActive(false);
        carreteraAnimeBackground.SetActive(false);
        islaTropicalBackground.SetActive(false);
        paisajeAnimeBackground.SetActive(false);
        manchasBlancasCafeBackground.SetActive(false);
        manchasVerdesBackground.SetActive(false);
        messiSeleccionBackground.SetActive(false);
        montañasNevadasBackground.SetActive(false);
        olasBackground.SetActive(false);
        pinosEnBosqueBackground.SetActive(false);
        pinturaEnOleoBackground.SetActive(false);
        pinturaEnVidrioBackground.SetActive(false);
        pinturaNeonBackground.SetActive(false);
        pulpoBackground.SetActive(true);
        montañasNeonBackground.SetActive(false);
        argentinaMundialBackground.SetActive(false);
    }

    public void MontañasNeonBackground()
    {
        bocaBackground.SetActive(false);
        riverBackground.SetActive(false);
        atlantaBackground.SetActive(false);
        velezBackground.SetActive(false);
        defaultBackground.SetActive(false);
        atardecerNubesRosasBackground.SetActive(false);
        bonsaiPaisajeRosaBackground.SetActive(false);
        ciudadCyberpunkBackground.SetActive(false);
        carreteraAnimeBackground.SetActive(false);
        islaTropicalBackground.SetActive(false);
        paisajeAnimeBackground.SetActive(false);
        manchasBlancasCafeBackground.SetActive(false);
        manchasVerdesBackground.SetActive(false);
        messiSeleccionBackground.SetActive(false);
        montañasNevadasBackground.SetActive(false);
        olasBackground.SetActive(false);
        pinosEnBosqueBackground.SetActive(false);
        pinturaEnOleoBackground.SetActive(false);
        pinturaEnVidrioBackground.SetActive(false);
        pinturaNeonBackground.SetActive(false);
        pulpoBackground.SetActive(false);
        montañasNeonBackground.SetActive(true);
        argentinaMundialBackground.SetActive(false);
    }

    public void ArgentinaMundialBackground()
    {
        bocaBackground.SetActive(false);
        riverBackground.SetActive(false);
        atlantaBackground.SetActive(false);
        velezBackground.SetActive(false);
        defaultBackground.SetActive(false);
        atardecerNubesRosasBackground.SetActive(false);
        bonsaiPaisajeRosaBackground.SetActive(false);
        ciudadCyberpunkBackground.SetActive(false);
        carreteraAnimeBackground.SetActive(false);
        islaTropicalBackground.SetActive(false);
        paisajeAnimeBackground.SetActive(false);
        manchasBlancasCafeBackground.SetActive(false);
        manchasVerdesBackground.SetActive(false);
        messiSeleccionBackground.SetActive(false);
        montañasNevadasBackground.SetActive(false);
        olasBackground.SetActive(false);
        pinosEnBosqueBackground.SetActive(false);
        pinturaEnOleoBackground.SetActive(false);
        pinturaEnVidrioBackground.SetActive(false);
        pinturaNeonBackground.SetActive(false);
        pulpoBackground.SetActive(false);
        montañasNeonBackground.SetActive(false);
        argentinaMundialBackground.SetActive(true);
    }
}
