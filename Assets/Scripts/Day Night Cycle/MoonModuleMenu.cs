using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonModuleMenu : DNModuleBaseMenu
{
    [SerializeField] private Light moon;
    [SerializeField] private Gradient moonColor;
    [SerializeField] private float baseIntensity;

    public override void UpdateModuleMenu(float intensity)
    {
        moon.color = moonColor.Evaluate(1 - intensity);
        moon.intensity = (1 - intensity) * baseIntensity + 0.05f;
    }
}
