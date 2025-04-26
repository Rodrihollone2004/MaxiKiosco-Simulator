public class KinderBueno : CandyProduct
{
    protected override void Awake()
    {
        base.Awake();
        ProductName = "Kinder Bueno";
        Price = 2500f;
        weightInGrams = 43f;
        brand = "Ferrero";
        type = CandyType.Chocolate;
    }
}
