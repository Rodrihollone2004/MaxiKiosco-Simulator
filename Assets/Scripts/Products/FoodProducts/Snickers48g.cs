public class Snickers48g : CandyProduct
{
    protected override void Awake()
    {
        base.Awake();
        ProductName = "Snickers 48g";
        Price = 1800f;
        weightInGrams = 48f;
        type = CandyType.Chocolate;
        brand = "Mars";
    }
}
