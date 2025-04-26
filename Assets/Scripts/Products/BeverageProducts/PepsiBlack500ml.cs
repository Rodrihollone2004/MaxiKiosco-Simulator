public class PepsiBlack500ml : ColaProduct
{
    protected override void Awake()
    {
        base.Awake();
        ProductName = "Pepsi Black 500ml";
        Price = 1400f;
        volumeInMl = 500f;
        carbonation = CarbonationType.Regular;
        hasSugar = false;
        variant = "Zero";
    }
}
