using System.Collections.Generic;

[System.Serializable]
public class ProductCategory 
{
    public productType Type;
    public int Id = 0;

    public List<Product> products;


    public void SetId_Editor(int idCategory)
    {
        Id = idCategory;
        for (int i = 0; i < products.Count; i++)
        {
            int idProduct = idCategory * 1000 + i + 1;
            products[i].SetId_Editor(idProduct);
        }
    }

}
