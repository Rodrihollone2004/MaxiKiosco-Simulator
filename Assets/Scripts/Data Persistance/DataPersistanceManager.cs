using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistanceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    
    private GameData gameData; 
    private List<IDataPersistance> dataPersistancesObjects;
    private FileDataHandler dataHandler;

    public static DataPersistanceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        Debug.Log("Ruta del guardado: " + Application.persistentDataPath);
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        //Cargar toda la data guardad mediante un Data Handler
        gameData = dataHandler.Load();

        if (gameData == null)
        {
            Debug.Log("No hay partidas guardadas");
            return;
        }

        TransitionManager.Instance.NextLevel();

        StartCoroutine(DelayLoadGame());
    }

    private IEnumerator DelayLoadGame()
    {
        while (SceneManager.GetActiveScene().name != "Game")
            yield return null;

        yield return null;

        dataPersistancesObjects = FindAllDataPersistanceObjects();

        foreach (IDataPersistance dataPersistanceObject in dataPersistancesObjects)
        {
            dataPersistanceObject.LoadData(gameData);
        }
    }
    
    public void SaveGame()
    {
        dataPersistancesObjects = FindAllDataPersistanceObjects();

        //Pasar la data a los otros scripts así se updatean
        foreach (IDataPersistance dataPersistanceObject in dataPersistancesObjects)
        {
            dataPersistanceObject.SaveData(ref gameData);
        }

        //guardar la data del archivo usando el Data Handler
        dataHandler.Save(gameData);
    }

    private List<IDataPersistance> FindAllDataPersistanceObjects()
    {
        IEnumerable<IDataPersistance> dataPersistancesObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistance>();

        return new List<IDataPersistance>(dataPersistancesObjects);
    }
}
