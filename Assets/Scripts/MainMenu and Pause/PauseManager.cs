using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] GameObject pauseMenuObject;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject graphicsMenu;
    [SerializeField] GameObject soundMenu;
    [SerializeField] GameObject gameplayMenu;
    [Header("Settings")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
    [SerializeField] PlayerCam playerCam;
    private bool isPaused = false;

    private void Awake()
    {
        // Suscribirse al evento de carga de escena
        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Desuscribirse para evitar memory leaks
        //SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    if (scene.name != "MainMenu")
    //    {
    //        LoadGame();
    //    }
    //}

    void Update()
    {
        if (!playerCam.IsInCashRegister)
        {
            if (Input.GetKeyDown(pauseKey))
            {
                if (isPaused == false)
                {
                    pauseMenuObject.SetActive(true);
                    isPaused = true;

                    Time.timeScale = 0f;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;

                }
                else
                {
                    Resume();
                }

            }
        }
    }

    public void Resume()
    {
        pauseMenuObject.SetActive(false);
        optionsMenu.SetActive(false);
        graphicsMenu.SetActive(false);
        soundMenu.SetActive(false);
        gameplayMenu.SetActive(false);
        isPaused = false;

        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void BackToMenu(string name)
    {
        SceneManager.LoadScene(name);
        Time.timeScale = 1f;
    }

    public void SaveGame()
    {
        DataPersistanceManager dataPersistanceManager = FindObjectOfType<DataPersistanceManager>();
        dataPersistanceManager.SaveGame();
        Time.timeScale = 1f;
        TransitionManager.Instance.BackMenu();
        Debug.Log("Partida guardad");
    }


    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey("PlayerPosX"))
        {
            return;
        }

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            return;
        }

        Vector3 savedPosition = new Vector3(
            PlayerPrefs.GetFloat("PlayerPosX"),
            PlayerPrefs.GetFloat("PlayerPosY"),
            PlayerPrefs.GetFloat("PlayerPosZ")
        );

        player.transform.position = savedPosition;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
