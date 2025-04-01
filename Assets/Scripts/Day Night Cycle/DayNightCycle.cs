using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time")]
    [Tooltip("Day Length in Minutes")]
    [SerializeField] private float _targetDayLength = 0.5f; // largo del dia en minutos
    [SerializeField]
    [Range(0f, 1f)]
    private float _timeOfDay;
    [SerializeField] private int _dayNumber = 0;// trackea los dias pasados
    [SerializeField] private int _yearNumber = 0;
    private float _timeScale = 100f;
    [SerializeField] private int _yearLength = 100;
    public bool pause = false;

    public float TargetDayLength { get => _targetDayLength; set => _targetDayLength = value; }
    public float TimeOfDay { get => _timeOfDay; set => _timeOfDay = value; }
    public int DayNumber { get => _dayNumber; set => _dayNumber = value; }
    public int YearNumber { get => _yearNumber; set => _yearNumber = value; }
    public int YearLength { get => _yearLength; set => _yearLength = value; }

    [Header("Sun Light")]
    [SerializeField] private Transform dailyRotation;
    [SerializeField] private Light sun;
    private float intensity;
    [SerializeField] private float sunBaseIntensity = 1f;
    [SerializeField] private float sunVariation = 1.5f;
    [SerializeField] private Gradient sunColor;

    private void Update()
    {
        if (!pause)
        {
            UpdateTimeScale();
            UpdateTime();
        }
        AdjustSunRotation();
        SunIntensity();
    }
    private void UpdateTimeScale()
    {
        _timeScale = 24 / (_targetDayLength / 60);
    }

    private void UpdateTime()
    {
        _timeOfDay += Time.deltaTime * _timeScale / 86400; // segundos en un dia
        if(_timeOfDay > 1)// dia nuevo
        {
            _dayNumber++;
            _timeOfDay -= 1;

            if(_dayNumber > _yearLength)// año nuevo
            {
                _yearNumber++;
                _dayNumber = 0;
            }
        }
    }

    // rotar el sol del dia
    private void AdjustSunRotation()
    {
        float sunAngle = TimeOfDay * 360f;
        dailyRotation.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, sunAngle));
    }

    private void SunIntensity()
    {
        intensity = Vector3.Dot(sun.transform.forward, Vector3.down);
        intensity = Mathf.Clamp01(intensity);

        sun.intensity = intensity * sunVariation + sunBaseIntensity;
    }

    private void AdjustSunColor()
    {
        sun.color = sunColor.Evaluate(intensity);
    }
}
