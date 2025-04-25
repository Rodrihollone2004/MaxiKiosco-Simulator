using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColaProduct : SodaProduct
{
    [Header("Cola Specific")]
    [SerializeField] protected private bool isDiet;
    [SerializeField] protected private string variant; // "Original", "Pomelo", etc.
}
