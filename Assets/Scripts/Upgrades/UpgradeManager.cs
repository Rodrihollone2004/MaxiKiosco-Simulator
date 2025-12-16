using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private ExperienceManager experienceManager;
    [SerializeField] private PlayerEconomy playerEconomy;
    //[SerializeField] private GameObject categoriesButtonPrefab;
    //[SerializeField] private Transform categoriesButtonsContainer;
    [SerializeField] private Transform productButtonContainer;
    [SerializeField] private GameObject productButtonPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject ConfirmUpgrade;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    [SerializeField] private LayerMask productLayer;

    [SerializeField] private List<UpgradeUpdate> upgradesUnlocked = new List<UpgradeUpdate>();

    [Header("Categories Variables")]
    [SerializeField] private GameObject categoriesButtonPrefab;
    [SerializeField] private Transform categoriesButtonsContainer;
    [SerializeField] private Sprite pressButton;

    private Sprite normalButton;
    private Dictionary<UpgradesType, Button> categoriesButtons = new Dictionary<UpgradesType, Button>();

    private Dictionary<Upgrade, GameObject> upgradesButtons = new Dictionary<Upgrade, GameObject>();

    public UpgradesType CurrentType { get; private set; }

    private void Awake()
    {
        CategoriesButtons();
    }

    private void Start()
    {
        ButtonType(UpgradesType.Upgrades);
        CheckButtonPressed(UpgradesType.Upgrades);
        ConfirmUpgrade.SetActive(false);
    }

    private void CategoriesButtons()
    {
        foreach (UpgradeUpdate upgrades in upgradesUnlocked)
            foreach (Upgrade category in upgrades.upgrades)
            {
                if (categoriesButtons.ContainsKey(category.Type))
                    continue;

                GameObject buttonGO = Instantiate(categoriesButtonPrefab, categoriesButtonsContainer);
                normalButton = buttonGO.GetComponent<Image>().sprite;
                TMP_Text text = buttonGO.GetComponentInChildren<TMP_Text>();
                text.text = $"{category.Type}";

                Button button = buttonGO.GetComponent<UnityEngine.UI.Button>();
                button.onClick.AddListener(() => ButtonType(category.Type));
                button.onClick.AddListener(() => CheckButtonPressed(category.Type));

                categoriesButtons.Add(category.Type, button);
            }
    }

    public void ButtonType(UpgradesType buttonType)
    {
        CurrentType = buttonType;
        foreach (KeyValuePair<Upgrade, GameObject> buttons in upgradesButtons)
        {
            if (buttonType == buttons.Key.Type)
            {
                buttons.Value.SetActive(true);
            }
            else
                buttons.Value.SetActive(false);
        }
    }

    private void CheckButtonPressed(UpgradesType button)
    {
        foreach (KeyValuePair<UpgradesType, Button> buttons in categoriesButtons)
        {
            if (button == buttons.Key)
            {
                buttons.Value.image.sprite = pressButton;
            }
            else
            {
                buttons.Value.image.sprite = normalButton;
            }
        }
    }

    public void PopulateStore()
    {
        if (upgradesUnlocked.Count > 0)
            foreach (UpgradeUpdate upgradeUpdate in upgradesUnlocked)
            {
                foreach (Upgrade upgrade in upgradeUpdate.upgrades)
                    if (experienceManager.CurrentLevel >= upgrade.LevelUpdate && !upgradesButtons.ContainsKey(upgrade))
                    {
                        GameObject inputGO = Instantiate(productButtonPrefab, productButtonContainer);
                        TMP_Text[] texts = inputGO.GetComponentsInChildren<TMP_Text>();
                        texts[0].text = $"{upgrade.Name}";
                        texts[1].text = $"${upgrade.Price}";

                        Image[] images = inputGO.GetComponentsInChildren<Image>(true);
                        foreach (var img in images)
                        {
                            if (img.gameObject.name == "Icon")
                            {
                                img.sprite = upgrade.Icon;
                                break;
                            }
                        }
                        if (!upgradesButtons.ContainsKey(upgrade))
                            upgradesButtons.Add(upgrade, inputGO);

                        inputGO.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            if (texts[0].text == "Heladera" && TutorialContent.Instance.CurrentIndexGuide == 12)
                            {
                                bool purchasedFridge = playerEconomy.HasEnoughMoney(upgrade.Price);
                                if (purchasedFridge && upgrade.Prefab != null)
                                {
                                    ConfirmUpgrade.SetActive(true);

                                    yesButton.onClick.RemoveAllListeners();

                                    yesButton.onClick.AddListener(() => SpawnUpgrade(upgrade));
                                    yesButton.onClick.AddListener(() => ConfirmUpgrade.SetActive(false));
                                    yesButton.onClick.AddListener(() => playerEconomy.SubtractMoneyUpgrade(upgrade));
                                    yesButton.onClick.AddListener(() => TutorialContent.Instance.CompleteStep(12));

                                    noButton.onClick.AddListener(() => ConfirmUpgrade.SetActive(false));
                                }
                            }

                            if (TutorialContent.Instance.CurrentIndexGuide < 14)
                                return;

                            bool purchased = playerEconomy.HasEnoughMoney(upgrade.Price);
                            if (purchased && upgrade.Prefab != null)
                            {
                                ConfirmUpgrade.SetActive(true);
                                yesButton.onClick.RemoveAllListeners();

                                yesButton.onClick.AddListener(() => SpawnUpgrade(upgrade));
                                yesButton.onClick.AddListener(() => ConfirmUpgrade.SetActive(false));
                                yesButton.onClick.AddListener(() => playerEconomy.SubtractMoneyUpgrade(upgrade));

                                noButton.onClick.AddListener(() => ConfirmUpgrade.SetActive(false));
                            }
                        });
                    }
            }
    }

    private void SpawnUpgrade(Upgrade upgrade)
    {
        GameObject spawned = Instantiate(upgrade.Prefab, spawnPoint.transform.position, Quaternion.identity);

        if (upgrade.Name != "Heladera")
            SetLayerRecursive(spawned, LayerMaskToLayer(productLayer));
        else
            spawned.name = upgrade.Name;

        UpgradeInteractable upgradeInteract = spawned.GetComponentInChildren<UpgradeInteractable>(true);
        if (upgradeInteract != null)
        {
            Light light = upgradeInteract.GetComponentInChildren<Light>();
            if (light != null)
                light.enabled = false;
        }
    }

    private void SetLayerRecursive(GameObject obj, int layer)
    {
        obj.layer = layer;
    }

    private int LayerMaskToLayer(LayerMask layerMask)
    {
        int layer = 0;
        int value = layerMask.value;
        while (value > 1)
        {
            value >>= 1;
            layer++;
        }
        return layer;
    }
}

[System.Serializable]
public class UpgradeUpdate
{
    public string nameUpdate;
    public List<Upgrade> upgrades;
}

public enum UpgradesType
{
    Upgrades,
    Decoración,
    Muebles,
    Iluminación,
}
