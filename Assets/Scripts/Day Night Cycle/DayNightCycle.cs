using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;
using Cinemachine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time")]
    [Tooltip("Day Length in Minutes")]
    [SerializeField] private float _targetDayLength = 13f; // largo del dia en minutos
    [SerializeField] private float elapsedTime;
    [SerializeField] private TMP_Text clockText;
    [SerializeField]
    [Range(0f, 1f)]
    private float _timeOfDay;
    [SerializeField] private int _dayNumber = 1;// trackea los dias pasados
    [SerializeField] private TMP_Text daysText;
    private float _timeScale = 100f;
    public bool pause = false;
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

    [Header("Configs")]
    [SerializeField] private DailySummary summaryUI;
    [SerializeField] private ClientQueueManager queueManager;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject sleepButton;
    [SerializeField] private GameObject newProductsUI;

    public bool sleepPressed;

    [SerializeField] CashRegisterInteraction cashRegisterInteraction;
    [SerializeField] GameObject storeGO;
    StoreUI storeUI;

    public bool IsPaused => pause;

    public int DayNumber { get => _dayNumber; set => _dayNumber = value; }
    public GameObject NewProductsUI { get => newProductsUI; set => newProductsUI = value; }

    public bool IsComplete;

    private void Awake()
    {
        NormalTimeCurve();
        _timeOfDay = 8f / 24f;
        elapsedTime = (_targetDayLength * 60) * _timeOfDay;
        pause = true;
        daysText.text = $"{_dayNumber}";

        storeUI = storeGO.GetComponent<StoreUI>();
        foreach (ProductCategory category in storeUI.Database.categories)
            category.products.Clear();

        UpateProducts();
    }

    private void Update()
    {
        if (!pause)
        {
            UpdateTimeScale();
            UpdateTime();
            UpdateClock();
        }
        AdjustSunRotation();
        SunIntensity();
        AdjustSunColor();
        UpdateModules();


        if (pause)
        {
            if (_timeOfDay >= (22f / 24f))
            {
                startButton.SetActive(false);
                sleepButton.SetActive(true);
            }
            else
            {
                startButton.SetActive(true);
                sleepButton.SetActive(false);
            }
        }
        else
        {
            startButton.SetActive(false);
            sleepButton.SetActive(false);
        }

    }
    public void OnStartButtonPressed()
    {
        if (TutorialContent.Instance.CurrentIndexGuide < 15)
            return;
        pause = false;
    }

    public void OnCloseDay()
    {
        StartCoroutine(OnSleepButtonPressed());
    }

    public IEnumerator OnSleepButtonPressed()
    {
        StartCoroutine(TransitionManager.Instance.EndDay());
        if (_timeOfDay >= (22f / 24f))
        {
            yield return new WaitForSeconds(1);
            CashRegisterInteraction cash = FindObjectOfType<CashRegisterInteraction>();
            StartCoroutine(cash.SafeExitCashRegisterMode());
            StartNewDay();
            sleepPressed = true;
        }
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

        if (previousTimeOfDay < (22f / 24f) && _timeOfDay >= (22f / 24f))
        {
            IsComplete = true;
            pause = true;
            _timeOfDay = 22f / 24f;
        }

        elapsedTime = _timeOfDay * (_targetDayLength * 60);
    }

    private void UpdateClock()
    {
        float time = elapsedTime / (_targetDayLength * 60);
        int hour = Mathf.FloorToInt(time * 24);
        int minute = Mathf.FloorToInt(((time * 24) - hour) * 60);

        minute = Mathf.Clamp(minute, 0, 59);

        string hourString = hour.ToString("00");
        string minuteString = minute.ToString("00");

        clockText.text = hourString + ":" + minuteString;
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
        // Convertir _timeOfDay (0-1) a hora del día (0-24)
        float currentHour = _timeOfDay * 24f;

        // Intensidad según hora (lineal o suave)
        if (currentHour < 6f || currentHour >= 22f)
        {
            intensity = 0f; // Noche
        }
        else if (currentHour >= 6f && currentHour < 12f)
        {
            // Amanecer: de 0 a 1
            intensity = Mathf.InverseLerp(6f, 12f, currentHour);
        }
        else if (currentHour >= 12f && currentHour < 19f)
        {
            // Atardecer: de 1 a 0.5
            intensity = Mathf.Lerp(1f, 0.5f, Mathf.InverseLerp(12f, 19f, currentHour));
        }
        else if (currentHour >= 19f && currentHour < 22f)
        {
            // Anochecer: de 0.5 a 0
            intensity = Mathf.Lerp(0.5f, 0f, Mathf.InverseLerp(19f, 22f, currentHour));
        }

        // Aplicar la intensidad al sol
        sun.intensity = intensity * sunVariation + sunBaseIntensity;
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

    public void StartNewDay()
    {
        int productsSold = queueManager.GetProductsSoldToday();
        summaryUI.ShowSummary(queueManager.GetClientsServedToday(), queueManager.GetMoneyEarnedToday(), productsSold);
        _dayNumber++;
        _timeOfDay = 8f / 24f;

        UpateProducts();
        storeUI.CheckUpdate();

        elapsedTime = (_targetDayLength * 60) * _timeOfDay;

        pause = true;
        UpdateClock();

        NPC_Controller[] currentNPCs = FindObjectsOfType<NPC_Controller>();
        foreach (NPC_Controller npc in currentNPCs)
        {
            npc.currentNode = AStarManager.instance.StartNode;
            queueManager.ReturnClientToPool(npc.client);
            npc.isInCashRegister = false;
            queueManager.ClientQueue.Clear();
        }

        GateInteractable[] gate = FindObjectsOfType<GateInteractable>();
        for (int i = 0; i < gate.Length; i++)
        {
            gate[i].BackToClose();
        }

        queueManager.ResetDailyStats();

        cashRegisterInteraction.ExitCashRegisterMode();
        cashRegisterInteraction.RestartCameraPos();
        cashRegisterInteraction.PlayerCam.enabled = false;
        cashRegisterInteraction.PlayerCamera.GetComponent<CinemachineBrain>().enabled = true;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = new Vector3(8, 1.166f, 0);

        IsComplete = false;

        daysText.text = $"{_dayNumber}";

        if (_dayNumber == 5)
            AnalyticsManager.Instance.DayFive();
    }

    private void UpateProducts()
    {
        storeUI.UpdateProducts();
    }

    public IEnumerator NewProducts()
    {
        newProductsUI.SetActive(true);

        yield return new WaitForSeconds(10f);

        newProductsUI.SetActive(false);
    }
}
