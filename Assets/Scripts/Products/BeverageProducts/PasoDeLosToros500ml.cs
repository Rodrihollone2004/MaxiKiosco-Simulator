public class PasoDeLosToros500ml : SodaProduct
{
    protected override void Awake()
    {
        base.Awake();
        ProductName = "Paso de los Toros 500ml";
        Price = 1350f;
        volumeInMl = 500f;
        carbonation = CarbonationType.Regular;
        hasSugar = true;
    }
}
