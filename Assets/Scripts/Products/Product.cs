using UnityEngine;

[CreateAssetMenu(fileName = "NewProduct", menuName = "Inventory/Product")]
public class Product : Item
{
    [SerializeField] private float _price;
    [SerializeField] private float _packSize;
    [SerializeField] private float _packPrice;

    public float Price => _price;
    public float PackSize => _packSize;
    public float PackPrice => _packPrice;
}
