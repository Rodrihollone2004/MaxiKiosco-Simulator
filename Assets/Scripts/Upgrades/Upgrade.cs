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

    public int AmountMin1000 { get => AmountMin1000; set => AmountMin1000 = value; }
    public int AmountMax1000 { get => AmountMax1000; set => AmountMax1000 = value; }
    public int AmountMinOthers { get => AmountMinOthers; set => AmountMinOthers = value; }
    public int AmountMaxOthers { get => AmountMaxOthers; set => AmountMaxOthers = value; }
    public int MaxProductsToBuy { get => maxProductsToBuy; set => maxProductsToBuy = value; }
    public int MaxAmountOfProductToBuy { get => maxAmountOfProductToBuy; set => maxAmountOfProductToBuy = value; }
}
