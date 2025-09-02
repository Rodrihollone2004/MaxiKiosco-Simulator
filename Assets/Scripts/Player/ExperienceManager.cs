using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExperienceManager : MonoBehaviour
{
    [Header("Experience")]
    [SerializeField] AnimationCurve experienceCurve;

    int currentLevel, totalExperience, levelToPoints;
    int previousLevelsExperience, nextLevelsExperience;

    [Header("Interface")]
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI experienceText;
    [SerializeField] TextMeshProUGUI levelPoints;
    [SerializeField] Image experienceFill;

    public int CurrentLevel { get => currentLevel; set => currentLevel = value; }

    private void Start()
    {
        UpdateLevel();
    }

    //SUBIR DE NIVEL MAS RAPIDO
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            AddExperience(20);
    }

    public void AddExperience(int amount)
    {
        totalExperience += amount;
        CheckForLevelUp();
        UpdateInterface();
    }

    void CheckForLevelUp()
    {
        if (totalExperience >= nextLevelsExperience)
        {
            currentLevel++;
            levelToPoints++;
            UpdateLevel();
            levelPoints.text = $"Nivel: {levelToPoints}";
        }
    }

    public bool PurchaseUpgrade(int upgrade)
    {
        if (levelToPoints >= upgrade)
        {
            levelToPoints -= upgrade;
            levelPoints.text = $"Nivel: {levelToPoints}";
            return true;
        }
        else
        {
            Debug.LogWarning("No tenes suficiente nivel para realizar la mejora.");
            return false;
        }
    }

    void UpdateLevel()
    {
        previousLevelsExperience = (int)experienceCurve.Evaluate(currentLevel);
        nextLevelsExperience = (int)experienceCurve.Evaluate(currentLevel + 1);
        UpdateInterface();
    }

    void UpdateInterface()
    {
        int start = totalExperience - previousLevelsExperience;
        int end = nextLevelsExperience - previousLevelsExperience;

        levelText.text = currentLevel.ToString();
        experienceText.text = start + " exp / " + end + " exp";
        experienceFill.fillAmount = (float)start / (float)end;
    }
}
