public class PilasEnergizerAAA : BatteryProduct
{
    private void Awake()
    {
        ProductName = "Pilas Energizer AAA";
        Price = 600f;
        type = BatteryType.AAA;
        voltage = 1.5f;
    }
}
