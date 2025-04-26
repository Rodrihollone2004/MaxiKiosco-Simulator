public class Marroc : CandyProduct
{
    protected override void Awake()
    {
        base.Awake();
        ProductName = "Marroc";
        Price = 685f;
        weightInGrams = 14f;
        brand = "Arcor";
        type = CandyType.Chocolate;
    }
}
