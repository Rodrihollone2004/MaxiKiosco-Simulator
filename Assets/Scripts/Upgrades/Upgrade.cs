using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Inventory/Upgrade")]
public class Upgrade : ScriptableObject
{
    [SerializeField] private string nameUpgrade;
    [SerializeField] private int levelUpdate;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int price;
    [SerializeField] private string placeZone;
    
    public int LevelUpdate { get => levelUpdate; set => levelUpdate = value; }
    public GameObject Prefab { get => prefab; set => prefab = value; }
    public int Price { get => price; set => price = value; }
    public string Name { get => nameUpgrade; set => nameUpgrade = value; }
    public string PlaceZone { get => placeZone; set => placeZone = value; }
}
