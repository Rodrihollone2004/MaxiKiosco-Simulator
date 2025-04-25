using UnityEngine;
public abstract class FoodProduct : Product
{
    [Header("Food Settings")]
    [SerializeField] protected float weightInGrams;
    [SerializeField] protected string brand;
}
