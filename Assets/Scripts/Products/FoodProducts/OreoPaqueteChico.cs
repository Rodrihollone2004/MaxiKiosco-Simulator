public class OreoPaqueteChico : CookieProduct
{
    protected override void Awake()
    {
        base.Awake();
        ProductName = "Paquete Oreo chico";
        Price = 2000f;
        weightInGrams = 120f;
        brand = "Oreo";
        isSweet = true;
        containsChocolate = true;
    }
}
