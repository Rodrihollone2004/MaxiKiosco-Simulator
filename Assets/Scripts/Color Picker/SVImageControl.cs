using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SVImageControl : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    [SerializeField] private Image pickerImage;

    private RawImage SVimage;

    private ColourPickerControl CC;

    private RectTransform rectTransform, pickerTransform;

    private bool isUsed;

    private void Awake()
    {
        SVimage = GetComponent<RawImage>();
        CC = FindObjectOfType<ColourPickerControl>();
        rectTransform = GetComponent<RectTransform>();
        isUsed = false;

        pickerTransform = pickerImage.GetComponent<RectTransform>();
        pickerTransform.localPosition = new Vector2(-(rectTransform.sizeDelta.x * 0.5f), -(rectTransform.sizeDelta.y * 0.5f));
    }

    private void UpdateColor(PointerEventData eventData)
    {
        Vector2 localPoint;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            Rect rect = rectTransform.rect;

            localPoint.x = Mathf.Clamp(localPoint.x, rect.xMin, rect.xMax);
            localPoint.y = Mathf.Clamp(localPoint.y, rect.yMin, rect.yMax);

            float xNorm = Mathf.InverseLerp(rect.xMin, rect.xMax, localPoint.x);
            float yNorm = Mathf.InverseLerp(rect.yMin, rect.yMax, localPoint.y);

            pickerTransform.localPosition = localPoint;

            pickerImage.color = Color.HSVToRGB(0, 0, 1 - yNorm);

            CC.SetSV(xNorm, yNorm);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateColor(eventData);
        if (!isUsed)
        {
            AnalyticsManager.Instance.ChangeColor();
            isUsed = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UpdateColor(eventData);
        if (!isUsed)
        {
            AnalyticsManager.Instance.ChangeColor();
            isUsed = true;
        }
    }

}
