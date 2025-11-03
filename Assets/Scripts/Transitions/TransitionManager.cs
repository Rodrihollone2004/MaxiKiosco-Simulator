using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;
    [SerializeField] Animator transitionAnim;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(Instance);
    }

    public void NextLevel()
    {
        StartCoroutine(LoadLevel());
    }

    private IEnumerator LoadLevel()
    {
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(1);

        SceneManager.LoadScene("Game");

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
