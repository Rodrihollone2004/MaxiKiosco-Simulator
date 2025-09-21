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
    [SerializeField] private LayerMask productLayer;

    [SerializeField] private List<Upgrade> upgradesUnlocked = new List<Upgrade>();

    private Dictionary<Upgrade, GameObject> upgradesButtons = new Dictionary<Upgrade, GameObject>();

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

                    inputGO.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
                    {
                        bool purchased = playerEconomy.TryPurchaseUpgrade(upgrade);
                        if (purchased && upgrade.Prefab != null)
                        {
                            SpawnUpgrade(upgrade);
                        }
                    });
                }
            }
    }

    private void SpawnUpgrade(Upgrade upgrade)
    {
        GameObject spawned = Instantiate(upgrade.Prefab, spawnPoint.transform.position, Quaternion.identity);

        SetLayerRecursive(spawned, LayerMaskToLayer(productLayer));

        if (!spawned.TryGetComponent<Rigidbody>(out _))
            spawned.AddComponent<Rigidbody>();

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
