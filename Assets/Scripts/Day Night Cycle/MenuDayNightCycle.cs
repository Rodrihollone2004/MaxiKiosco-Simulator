using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuDayNightCycle : MonoBehaviour
{
    [Header("Time")]
    [Tooltip("Day Length in Minutes")]
    [SerializeField] private float _targetDayLength = 13f; // largo del dia en minutos
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
    private List<DNModuleBase> moduleList = new List<DNModuleBase>();

    private void Awake()
    {
        NormalTimeCurve();
        _timeOfDay = 0f / 24f;
        elapsedTime = (_targetDayLength * 60) * _timeOfDay;
    }

    private void Update()
    {
        UpdateTimeScale();
        UpdateTime();
        UpdateClock();
        AdjustSunRotation();
        AdjustSunColor();
        UpdateModules();
    }

    private void UpdateTimeScale()
    {
        _timeScale = 24 / (_targetDayLength / 60);
        _timeScale *= timeCurve.Evaluate(elapsedTime / (_targetDayLength * 60));
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
        float previousTimeOfDay = _timeOfDay;

        _timeOfDay += Time.deltaTime * _timeScale / 86400f;

        if (previousTimeOfDay < (24f / 24f) && _timeOfDay >= (22f / 24f))
        {
            _timeOfDay = 24f / 24f;
        }

        elapsedTime = _timeOfDay * (_targetDayLength * 60);
    }

    private void UpdateClock()
    {
        float time = elapsedTime / (_targetDayLength * 60);
        int hour = Mathf.FloorToInt(time * 24);
        int minute = Mathf.FloorToInt(((time * 24) - hour) * 60);

        minute = Mathf.Clamp(minute, 0, 59);
    }

    // rotar el sol del dia
    private void AdjustSunRotation()
    {
        float sunAngle = _timeOfDay * 360f;
        dailyRotation.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, sunAngle));

        float seasonalAngle = -maxSeasonalTilt * Mathf.Cos(_dayNumber * 2f * Mathf.PI);
        sunSeasonalRotation.localRotation = Quaternion.Euler(new Vector3(seasonalAngle, 0f, 0f));
    }
   

    private void AdjustSunColor()
    {
        sun.color = sunColor.Evaluate(intensity);
    }

    public void AddModule(DNModuleBase module)
    {
        moduleList.Add(module);
    }

    public void RemoveModule(DNModuleBase module)
    {
        moduleList.Remove(module);
    }

    private void UpdateModules()
    {
        foreach (DNModuleBase module in moduleList)
        {
            module.UpdateModule(intensity);
        }
    }
}
