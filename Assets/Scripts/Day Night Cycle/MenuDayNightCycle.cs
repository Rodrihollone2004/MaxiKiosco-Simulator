using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MenuDayNightCycle : MonoBehaviour
{
    [Header("Time")]
    [Tooltip("Day Length in Minutes")]
    [SerializeField] private float _targetDayLength = .2f; // largo del dia en minutos
    [SerializeField] private float elapsedTime;
    [SerializeField]
    [Range(0f, 1f)]
    private float _timeOfDay;
    [SerializeField] private int _dayNumber = 1;// trackea los dias pasados
    private float _timeScale = 100f;
    [SerializeField] private AnimationCurve timeCurve;
    private float timeCurveNormalization;

    [Header("Sun Light")]
    [SerializeField] private Transform dailyRotation;
    [SerializeField] private Light sun;
    private float intensity;
    [SerializeField] private float sunBaseIntensity = 1f;
    [SerializeField] private float sunVariation = 1.5f;
    [SerializeField] private Gradient sunColor;

    [Header("Seasonal Variables")]
    [SerializeField] private Transform sunSeasonalRotation;
    [SerializeField]
    [Range(-45f, 45f)]
    private float maxSeasonalTilt;

    [Header("Modules")]
    private List<DNModuleBaseMenu> moduleList = new List<DNModuleBaseMenu>();


    private void Awake()
    {
        NormalTimeCurve();
    }

    private void Update()
    {
        UpdateTimeScale();
        UpdateTime();
        AdjustSunRotation();
        SunIntensity();
        AdjustSunColor();
        UpdateModules();
    }

    private void UpdateTimeScale()
    {
        _timeScale = 24 / (_targetDayLength / 60);
        _timeScale *= timeCurve.Evaluate(_timeOfDay);
        _timeScale /= timeCurveNormalization;
    }

    private void NormalTimeCurve()
    {
        float stepSize = 0.01f;
        int numberSteps = Mathf.FloorToInt(1f / stepSize);
        float curveTotal = 0;

        for (int i = 0; i < numberSteps; i++)
        {
            curveTotal += timeCurve.Evaluate(i * stepSize);
        }

        timeCurveNormalization = curveTotal / numberSteps;
    }

    private void UpdateTime()
    {
        _timeOfDay += Time.deltaTime * _timeScale / 86400f;
    }

    // rotar el sol del dia
    private void AdjustSunRotation()
    {
        float sunAngle = _timeOfDay * 360f;
        dailyRotation.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, sunAngle));

        float seasonalAngle = -maxSeasonalTilt * Mathf.Cos(_dayNumber * 2f * Mathf.PI);
        sunSeasonalRotation.localRotation = Quaternion.Euler(new Vector3(seasonalAngle, 0f, 0f));
    }

    private void SunIntensity()
    {
        intensity = Vector3.Dot(sun.transform.forward, Vector3.down);
        intensity = Mathf.Clamp01(intensity);

        sun.intensity = intensity;
    }

    private void AdjustSunColor()
    {
        sun.color = sunColor.Evaluate(intensity);
    }

    public void AddModuleMenu(DNModuleBaseMenu module)
    {
        moduleList.Add(module);
    }

    public void RemoveModuleMenu(DNModuleBaseMenu module)
    {
        moduleList.Remove(module);
    }

    private void UpdateModules()
    {
        foreach (DNModuleBaseMenu module in moduleList)
        {
            module.UpdateModuleMenu(intensity);
        }
    }
}
