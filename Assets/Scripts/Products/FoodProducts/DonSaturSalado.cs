public class DonSaturSalado : CookieProduct
{
    protected override void Awake()
    {
        base.Awake();
        ProductName = "Don Satur salado";
        Price = 2000f;
        weightInGrams = 200f;
        brand = "Don Satur";
        isSweet = false;
    }
}
