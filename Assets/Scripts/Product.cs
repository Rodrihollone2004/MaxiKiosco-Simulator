using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Product : MonoBehaviour, IInteractable
{
    [SerializeField] private string productName;
    [SerializeField] private float price;

    public float Price { get => price; set => price = value; }

    public void Interact()
    {
        Debug.Log($"Interactuando con {productName} (Precio: ${price})");
    }
}
