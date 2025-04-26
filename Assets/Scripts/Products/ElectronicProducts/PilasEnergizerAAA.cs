public class PilasEnergizerAAA : BatteryProduct
{
    protected override void Awake()
    {
        base.Awake();
        ProductName = "Pilas Energizer AAA";
        Price = 600f;
        type = BatteryType.AAA;
        voltage = 1.5f;
    }
}
