using System.Collections;
using System.Collections.Generic;
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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Desuscribirse para evitar memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "MainMenu")
        {
            LoadGame();
        }
    }

    void Update()
    {
        if(!playerCam.IsInCashRegister)
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
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found for saving position.");
            return;
        }

        PlayerPrefs.SetFloat("PlayerPosX", player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerPosY", player.transform.position.y);
        PlayerPrefs.SetFloat("PlayerPosZ", player.transform.position.z);
        PlayerPrefs.SetString("SavedLevel", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
    }


    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey("PlayerPosX"))
        {
            Debug.Log("No hay datos de posición guardados.");
            return;
        }

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found for loading position.");
            return;
        }

        Vector3 savedPosition = new Vector3(
            PlayerPrefs.GetFloat("PlayerPosX"),
            PlayerPrefs.GetFloat("PlayerPosY"),
            PlayerPrefs.GetFloat("PlayerPosZ")
        );

        player.transform.position = savedPosition;
        Debug.Log($"Posición cargada: {savedPosition}");
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Se salio del juego");
    }
}
