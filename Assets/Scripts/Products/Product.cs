using UnityEngine;

[CreateAssetMenu(fileName = "NewProduct", menuName = "Inventory/Product")]
public class Product : Item
{
    [SerializeField] private int _price;
    [SerializeField] private int _packSize;
    [SerializeField] private int _packPrice;

    public int Price => _price;
    public int PackSize => _packSize;
    public int PackPrice => _packPrice;

  
}
