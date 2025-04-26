using System.Collections.Generic;

[System.Serializable]
public class ProductCategory 
{
    public string Name = "No category";
    public int Id = 0;

    public List<Product> products;

#if UNITY_EDITOR
    public void SetId_Editor(int idCategory)
    {
        Id = idCategory;
        for (int i = 0; i < products.Count; i++)
        {
            int idProduct = idCategory * 1000 + i + 1;
            products[i].SetId_Editor(idProduct);
        }
    }
#endif
}
