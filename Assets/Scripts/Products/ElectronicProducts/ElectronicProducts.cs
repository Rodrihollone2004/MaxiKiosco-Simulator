using UnityEngine;
public abstract class ElectronicProducts : Product
{
    [Header("Electronic Settings")]
    [SerializeField] protected float voltage;
    [SerializeField] protected bool isRechargeable;
}
