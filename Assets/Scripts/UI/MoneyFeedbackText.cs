using System.Collections;
using TMPro;
using UnityEngine;

public class MoneyFeedbackText : MonoBehaviour
{
    [SerializeField] private float floatSpeed = 40f;
    [SerializeField] private float fadeDuration = 1f;

    private TMP_Text text;
    private CanvasGroup canvasGroup;
    private Vector3 floatDirection;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Setup(string message, Color color)
    {
        text.text = message;
        text.color = color;

        if (message.StartsWith("+"))
            floatDirection = Vector3.up;
        else
            floatDirection = Vector3.down;

        StartCoroutine(FloatAndFade());
    }

    private IEnumerator FloatAndFade()
    {
        Vector3 startPos = transform.localPosition;
        Vector3 endPos = startPos + floatDirection * 50f;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            canvasGroup.alpha = 1 - t;

            yield return null;
        }

        Destroy(gameObject);
    }
}
