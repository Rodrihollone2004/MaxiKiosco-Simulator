using UnityEngine;

public class SkyboxModule : DNModuleBase
{
    [SerializeField] private Gradient skyColor;
    [SerializeField] private Gradient horizonColor;

    public override void UpdateModule(float intensity)
    {
        RenderSettings.skybox.SetFloat("_Exposure", Mathf.Lerp(0.01f, 1.5f, intensity));
        RenderSettings.skybox.SetFloat("_Rotation", intensity * 360);
    }
}
