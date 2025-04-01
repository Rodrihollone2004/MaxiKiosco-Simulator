using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuObject;
    //[SerializeField] GameObject exitMenu;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
    [SerializeField] PlayerCam playerCam;
    private bool pause = false;


    void Update()
    {
        if(!playerCam.IsInCashRegister)
        {
            if (Input.GetKeyDown(pauseKey))
            {
                if (pause == false)
                {
                    pauseMenuObject.SetActive(true);
                    pause = true;

                    Time.timeScale = 0f;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;

                    //AudioSource[] sounds = FindObjectsOfType<AudioSource>();

                    //for (int i = 0; i < sounds.Length; i++)
                    //{
                    //    sounds[i].Pause();
                    //}
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
        //exitMenu.SetActive(false);
        optionsMenu.SetActive(false);
        pause = false;

        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //AudioSource[] sounds = FindObjectsOfType<AudioSource>();

        //for (int i = 0; i < sounds.Length; i++)
        //{
        //    sounds[i].Play();
        //}
    }
    public void BackToMenu(string name)
    {
        SaveGame();
        SceneManager.LoadScene(name);
        Time.timeScale = 1f;
    }

    public void SaveGame()
    {
        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        PlayerPrefs.SetFloat("PlayerPosX", playerPos.x);
        PlayerPrefs.SetFloat("PlayerPosY", playerPos.y);
        PlayerPrefs.SetFloat("PlayerPosZ", playerPos.z);

        int playerMoney = PlayerPrefs.GetInt("PlayerMoney", 0);
        PlayerPrefs.SetInt("PlayerMoney", playerMoney);

        PlayerPrefs.SetString("SavedLevel", SceneManager.GetActiveScene().name);

        PlayerPrefs.Save();
        Debug.Log("Partida guardada correctamente.");
    }

    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("PlayerPosX"))
        {
            float x = PlayerPrefs.GetFloat("PlayerPosX");
            float y = PlayerPrefs.GetFloat("PlayerPosY");
            float z = PlayerPrefs.GetFloat("PlayerPosZ");

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = new Vector3(x, y, z);
        }

        if (PlayerPrefs.HasKey("PlayerMoney"))
        {
            int playerMoney = PlayerPrefs.GetInt("PlayerMoney");
        }

        Debug.Log("Partida cargada correctamente.");
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Se salio del juego");
    }
}
