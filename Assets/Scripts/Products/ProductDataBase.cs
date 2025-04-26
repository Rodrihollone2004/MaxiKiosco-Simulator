using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProductDatabase", menuName = "Inventory/Product Database")]
public class ProductDataBase : ScriptableObject
{
    public List<Product> AllProducts;
    public Product GetProductByName(string name)
    {
        return AllProducts.Find(p => p.Name == name);
    }
}
