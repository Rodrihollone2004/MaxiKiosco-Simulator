using UnityEngine;
public abstract class AlfajorProduct : FoodProduct
{
    [Header("Alfajor Settings")]
    [SerializeField] protected int layers;
    [SerializeField] protected bool hasChocolateCover;
}
