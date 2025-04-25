public class BateriaDuracell9V : BatteryProduct
{
    private void Awake()
    {
        ProductName = "Bateria Duracell 9V";
        Price = 800f;
        type = BatteryType._9V;
        voltage = 9f;
    }
}
