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

    [SerializeField] private List<Upgrade> upgradesUnlocked = new List<Upgrade>();

    private Dictionary<Upgrade, GameObject> upgradesButtons = new Dictionary<Upgrade, GameObject>();

    private void Start()
    {
        ConfirmUpgrade.SetActive(false);
    }

    public void PopulateStore()
    {
        if (upgradesUnlocked.Count > 0)
            foreach (Upgrade upgrade in upgradesUnlocked)
            {
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

                    upgradesButtons.Add(upgrade, inputGO);

                    inputGO.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        if (texts[0].text == "Heladera" && TutorialContent.Instance.CurrentIndexGuide == 7)
                        {
                            bool purchasedFridge = playerEconomy.HasEnoughMoney(upgrade.Price);
                            if (purchasedFridge && upgrade.Prefab != null)
                            {
                                ConfirmUpgrade.SetActive(true);

                                yesButton.onClick.RemoveAllListeners();

                                yesButton.onClick.AddListener(() => SpawnUpgrade(upgrade));
                                yesButton.onClick.AddListener(() => ConfirmUpgrade.SetActive(false));
                                yesButton.onClick.AddListener(() => playerEconomy.SubtractMoneyUpgrade(upgrade));
                                yesButton.onClick.AddListener(() => TutorialContent.Instance.CompleteStep(7));

                                noButton.onClick.AddListener(() => ConfirmUpgrade.SetActive(false));
                            }
                        }

                        if (TutorialContent.Instance.CurrentIndexGuide < 12)
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

        //if (!spawned.TryGetComponent<Rigidbody>(out _))
        //    spawned.AddComponent<Rigidbody>();

        if (!spawned.TryGetComponent<Collider>(out _))
            spawned.AddComponent<BoxCollider>();
    }

    private void SetLayerRecursive(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursive(child.gameObject, layer);
        }
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
