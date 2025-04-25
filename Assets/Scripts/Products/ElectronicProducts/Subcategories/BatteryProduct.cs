using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryProduct : ElectronicProducts
{
    public enum BatteryType { AA, AAA, C, D, _9V, Button }
    [Header("Battery Specific")]
    [SerializeField] protected private BatteryType type;

    public override void Interact()
    {
        base.Interact();
        Debug.Log($"Tipo de pila: {type}");
    }
}
