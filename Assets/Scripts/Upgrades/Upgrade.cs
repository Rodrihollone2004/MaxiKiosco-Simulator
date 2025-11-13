using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Inventory/Upgrade")]
public class Upgrade : ScriptableObject
{
    [SerializeField] private string nameUpgrade;
    [SerializeField] private int levelUpdate;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int price;
    [SerializeField] private string placeZone;
    [SerializeField] private Sprite icon;
    [SerializeField] private string infoUpgrade;
    [SerializeField] private bool isUpgradeChange;
    [SerializeField] private int valueForUpgrade;

    [SerializeField] private int amountMin1000 = 0;
    [SerializeField] private int amountMax1000 = 0;

    [SerializeField] private int amountMinOthers = 0;
    [SerializeField] private int amountMaxOthers = 0;

    [SerializeField] private int maxProductsToBuy = 0;
    [SerializeField] private int maxAmountOfProductToBuy = 0;

    public int LevelUpdate { get => levelUpdate; set => levelUpdate = value; }
    public GameObject Prefab { get => prefab; set => prefab = value; }
    public int Price { get => price; set => price = value; }
    public string Name { get => nameUpgrade; set => nameUpgrade = value; }
    public string PlaceZone { get => placeZone; set => placeZone = value; }
    public Sprite Icon => icon;

    public int AmountMin1000 { get => amountMin1000; set => amountMin1000 = value; }
    public int AmountMax1000 { get => amountMax1000; set => amountMax1000 = value; }
    public int AmountMinOthers { get => amountMinOthers; set => amountMinOthers = value; }
    public int AmountMaxOthers { get => amountMaxOthers; set => amountMaxOthers = value; }
    public int MaxProductsToBuy { get => maxProductsToBuy; set => maxProductsToBuy = value; }
    public int MaxAmountOfProductToBuy { get => maxAmountOfProductToBuy; set => maxAmountOfProductToBuy = value; }
    public string InfoUpgrade { get => infoUpgrade; set => infoUpgrade = value; }
    public bool IsUpgradeChange { get => isUpgradeChange; set => isUpgradeChange = value; }
    public int ValueForUpgrade { get => valueForUpgrade; set => valueForUpgrade = value; }
}
