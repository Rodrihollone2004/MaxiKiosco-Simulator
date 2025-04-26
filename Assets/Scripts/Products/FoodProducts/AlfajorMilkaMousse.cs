public class AlfajorMilkaMousse : AlfajorProduct
{
    protected override void Awake()
    {
        base.Awake();
        ProductName = "Alfajor Milka Mousse";
        Price = 1800f;
        weightInGrams = 55f;
        brand = "Milka";
        layers = 3;
        hasChocolateCover = true;
    }
}
