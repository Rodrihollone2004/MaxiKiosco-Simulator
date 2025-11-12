using UnityEngine;

public class SkyboxModuleMenu : DNModuleBaseMenu
{
    [SerializeField] private Gradient skyColor;
    [SerializeField] private Gradient horizonColor;

    public override void UpdateModuleMenu(float intensity)
    {
        RenderSettings.skybox.SetFloat("_Exposure", Mathf.Lerp(0.01f, 1.5f, intensity));
        RenderSettings.skybox.SetFloat("_Rotation", intensity * 360);
    }
}
