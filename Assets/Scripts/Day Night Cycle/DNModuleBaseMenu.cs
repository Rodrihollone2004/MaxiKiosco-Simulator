using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DNModuleBaseMenu : MonoBehaviour
{
    protected MenuDayNightCycle dayNightControl;

    private void OnEnable()
    {
        dayNightControl = this.GetComponent<MenuDayNightCycle>();
        if (dayNightControl != null)
            dayNightControl.AddModuleMenu(this);
    }
    private void OnDisable()
    {
        if (dayNightControl != null)
            dayNightControl.RemoveModuleMenu(this);
    }
    public abstract void UpdateModuleMenu(float intensity);
}
