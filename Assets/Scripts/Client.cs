using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour
{
    private float amountToPay;

    public float AmountToPay { get => amountToPay; set => amountToPay = value; }

    private void Start()
    {
        amountToPay = Random.Range(10, 100);
    }
}
