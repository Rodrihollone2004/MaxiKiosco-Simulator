using UnityEngine;

[CreateAssetMenu(fileName = "NewProduct", menuName = "Inventory/Product")]
public class Product : Item
{
    [SerializeField] private int _price;
    [SerializeField] private int _packSize;
    [SerializeField] private int _currentAmount;
    [SerializeField] private int _packPrice;
    [SerializeField] private GameObject prefab;

    public int Price { get; set; }
    public int PackSize => _packSize;
    public int PackPrice => _packPrice;

    public GameObject Prefab => prefab;

    public int CurrentAmount { get => _currentAmount; set => _currentAmount = value; }
}
