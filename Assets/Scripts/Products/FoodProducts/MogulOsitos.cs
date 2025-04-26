public class MogulOsitos : CandyProduct
{
    protected override void Awake()
    {
        base.Awake();
        ProductName = "Mogul Ositos";
        Price = 1200f;
        weightInGrams = 50f;
        brand = "Arcor";
        type = CandyType.Gummy;
    }
}
