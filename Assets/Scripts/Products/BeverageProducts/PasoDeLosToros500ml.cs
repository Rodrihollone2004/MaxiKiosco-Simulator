using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasoDeLosToros500ml : SodaProduct
{
    private void Awake()
    {
        ProductName = "Paso de los Toros 500ml";
        Price = 1350f;
        volumeInMl = 500f;
        carbonation = CarbonationType.Regular;
        hasSugar = true;
    }
}
