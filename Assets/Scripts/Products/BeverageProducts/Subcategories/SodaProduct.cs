using UnityEngine;

public abstract class SodaProduct : BeverageProduct
{
    public enum CarbonationType { Regular, Low, Zero }

    [Header("Soda Settings")]
    [SerializeField] protected CarbonationType carbonation;
    [SerializeField] protected bool hasSugar;
}
