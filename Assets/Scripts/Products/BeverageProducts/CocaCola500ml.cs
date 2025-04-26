public class CocaCola500ml : ColaProduct
{
    protected override void Awake()
    {
        base.Awake();
        ProductName = "Coca-Cola 500ml";
        Price = 1500f;
        volumeInMl = 500f;
        carbonation = CarbonationType.Regular;
        hasSugar = true;
        isDiet = false;
        variant = "Original";
    }
}
