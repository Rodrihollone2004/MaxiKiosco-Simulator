using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;
    Animator transitionAnim;

    private void Awake()
    {
        if (transitionAnim == null)
            transitionAnim = GetComponent<Animator>();

        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void NextLevel()
    {
        StartCoroutine(LoadLevel());
    }

    public void BackMenu()
    {
        StartCoroutine(LoadMenu());
    }

    private IEnumerator LoadLevel()
    {
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(1);

        SceneManager.LoadScene("Game");

        yield return new WaitForEndOfFrame();
        transitionAnim.SetTrigger("Start");
    }

    private IEnumerator LoadMenu()
    {
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(1);

        SceneManager.LoadScene("MainMenu");

        yield return new WaitForEndOfFrame();
        transitionAnim.SetTrigger("Start");
    }

    public IEnumerator EndDay()
    {
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(2f);
        transitionAnim.SetTrigger("Start");
    }
}
