public class Sprite1_5L : SodaProduct
{
    protected override void Awake()
    {
        base.Awake();
        ProductName = "Sprite 1.5L";
        Price = 2400f;
        volumeInMl = 1500f;
        carbonation = CarbonationType.Regular;
        hasSugar = true;
    }
}
