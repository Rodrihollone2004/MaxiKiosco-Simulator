public class SurtidoBagley : CookieProduct
{
    protected override void Awake()
    {
        base.Awake();
        ProductName = "Surtido Bagley";
        Price = 1500f;
        weightInGrams = 250f;
        brand = "Bagley";
        isSweet = true;
    }
}
