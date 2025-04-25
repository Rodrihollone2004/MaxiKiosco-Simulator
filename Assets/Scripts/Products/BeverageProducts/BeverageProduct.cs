using UnityEngine;
public abstract class BeverageProduct : Product
{
    [Header("Beverage Settings")]
    [SerializeField] protected float volumeInMl;
    [SerializeField] protected bool isAlcoholic;
}
