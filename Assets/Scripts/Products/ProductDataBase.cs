using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProductDatabase", menuName = "Inventory/Product Database")]
public class ProductDataBase : ScriptableObject
{
    public List<ProductCategory> categories = new List<ProductCategory>();
    public Product GetProductByName(string name)
    {
        foreach(ProductCategory category in categories)
            foreach(Product product in category.products)
                if(product.name == name) 
                    return product;

        Debug.LogWarning("Producto no encontrado: " + name);
        return null;
    }
    public Product GetProductById(int id)
    {
        foreach (ProductCategory category in categories)
            foreach (Product product in category.products)
                if (product.Id == id)
                    return product;

        Debug.LogWarning("Producto no encontrado: " + id);
        return null;
    }

    private void OnValidate()
    {
        for (int i = 0; i < categories.Count; i++)
        {
            int id = i * 1000;
            categories[i].SetId_Editor(id);
        }
    }
}
