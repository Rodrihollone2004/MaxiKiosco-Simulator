using UnityEngine;
public class CandyProduct : FoodProduct
{
    [Header("Candy Specific")]
    [SerializeField] protected private CandyType type;
    public enum CandyType { Chocolate, Gummy, Hard, ChewingGum }
}
