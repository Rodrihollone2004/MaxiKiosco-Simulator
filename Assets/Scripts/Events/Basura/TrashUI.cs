using TMPro;
using UnityEngine;
using System.Collections;

public class TrashUI : MonoBehaviour
{
    [SerializeField] private ClientTrashSpawner trashSpawner;
    [SerializeField] private TMP_Text trashText;
    [SerializeField] private GameObject panel;

    private Coroutine hidePanelCoroutine;
    private bool isAbove50 = false;

    private void OnEnable()
    {
        trashSpawner.OnTrashChanged.AddListener(UpdateTrashText);
    }

    private void OnDisable()
    {
        trashSpawner.OnTrashChanged.RemoveListener(UpdateTrashText);
    }

    private void UpdateTrashText(float percentage)
    {
        trashText.text = $"Basura: {percentage:0}%";

        if (percentage >= 70f)
        {
            trashText.color = Color.red;
            AnalyticsManager.Instance.Trash70();
            trashText.text = $"Basura: {percentage:0}% <sprite name=Advertencia>\n";
        }
        else if (percentage >= 50f)
            trashText.color = Color.yellow;
        else
            trashText.color = Color.white;

        if (percentage >= 50f)
        {
            if (!isAbove50)
            {
                isAbove50 = true;

                if (hidePanelCoroutine != null)
                    StopCoroutine(hidePanelCoroutine);

                panel.SetActive(true);
            }
        }
        else
        {
            if (isAbove50)
            {
                isAbove50 = false;
            }

            if (hidePanelCoroutine != null)
                StopCoroutine(hidePanelCoroutine);

            hidePanelCoroutine = StartCoroutine(ShowPanelTemporarily());
        }
    }


    private IEnumerator ShowPanelTemporarily()
    {
        panel.SetActive(true);
        yield return new WaitForSeconds(3f);
        panel.SetActive(false);
    }
}
