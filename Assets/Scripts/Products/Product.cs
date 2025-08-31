using UnityEngine;

[CreateAssetMenu(fileName = "NewProduct", menuName = "Inventory/Product")]
public class Product : Item
{
    [SerializeField] private int _price;
    [SerializeField] private string _placeZone;
    [SerializeField] private int _packSize;
    [SerializeField] private int _currentAmount;
    [SerializeField] private int _packPrice;
    [SerializeField] private GameObject prefab;
    [SerializeField] private productType type;


    public int Price { get => _price; set => _price = value; }
    public string PlaceZone { get => _placeZone; set => _placeZone = value; }
    public int PackSize => _packSize;
    public int PackPrice => _packPrice;

    public GameObject Prefab => prefab;

    public int CurrentAmount { get => _currentAmount; set => _currentAmount = value; }
    public productType Type { get => type; set => type = value; }
}

public enum productType
{
    Chocolates,
    Bebidas,
    Alfajores,
    Chicles,
    Galletitas,
    Encendedores,
    Golosinas
}