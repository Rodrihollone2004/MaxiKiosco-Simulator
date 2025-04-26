public class AlfajorCachafazNegro : AlfajorProduct
{
    protected override void Awake()
    {
        base.Awake();
        ProductName = "Alfajor Cachafaz Negro";
        Price = 1200f;
        weightInGrams = 60f;
        brand = "Cachafaz";
        layers = 2;
        hasChocolateCover = true;
    }
}
