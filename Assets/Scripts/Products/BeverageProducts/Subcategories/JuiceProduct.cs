using UnityEngine;

public class JuiceProduct : BeverageProduct
{
    [Header("Juice Settings")]
    [SerializeField] private FruitType mainFruit;
    public enum FruitType { Orange, Apple, Grape, Multi }

    [SerializeField] private bool isFromConcentrate;
}
