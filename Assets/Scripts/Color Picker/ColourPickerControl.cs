using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class ColourPickerControl : MonoBehaviour
{
    public float currentHue, currentSat, currentVal;

    [SerializeField]
    private RawImage hueImage, satValImage, outputImage;

    [SerializeField] private Slider hueSlider;

    private Texture2D hueTexture, svTexture, outputTexture;

    [SerializeField] List<MeshRenderer> changeThisColor;

    private void Start()
    {
        CreateHueImage();
        CreateSVImage();
        CreateOutputImage();

        hueSlider.value = currentHue;

        UpdateSVImage();
        UpdateOutputImage();
    }

    private void CreateHueImage()
    {
        hueTexture = new Texture2D(1, 16);
        hueTexture.wrapMode = TextureWrapMode.Clamp;
        hueTexture.name = "HueTexture";

        for (int i = 0; i < hueTexture.height; i++)
        {
            hueTexture.SetPixel(0, i, Color.HSVToRGB((float)i / hueTexture.height, 1, 1f));
        }

        hueTexture.Apply();
        currentHue = 0.7f;

        hueImage.texture = hueTexture;
    }

    private void CreateSVImage()
    {
        int size = 256;
        svTexture = new Texture2D(size, size);
        svTexture.wrapMode = TextureWrapMode.Clamp;
        svTexture.name = "SatValTexture";

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                svTexture.SetPixel(x, y, Color.HSVToRGB(
                    currentHue,
                    (float)x / (size - 1),
                    (float)y / (size - 1)));
            }
        }

        svTexture.Apply();
        currentSat = 0.13f;
        currentVal = 0.88f;

        satValImage.texture = svTexture;
    }

    private void CreateOutputImage()
    {
        outputTexture = new Texture2D(1, 16);
        outputTexture.wrapMode = TextureWrapMode.Clamp;
        outputTexture.name = "OutputTexture";

        Color currentColor = Color.HSVToRGB(currentHue, currentSat, currentVal);

        for (int i = 0; i < outputTexture.height; i++)
        {
            outputTexture.SetPixel(0, i, currentColor);
        }

        outputTexture.Apply();

        outputImage.texture = outputTexture;
    }

    private void UpdateOutputImage()
    {
        Color currentColor = Color.HSVToRGB(currentHue, currentSat, currentVal);

        for (int i = 0; i < outputTexture.height; i++)
        {
            outputTexture.SetPixel(0, i, currentColor);
        }

        outputTexture.Apply();

        for (int i = 0; i < changeThisColor.Count; i++)
            changeThisColor[i].GetComponent<MeshRenderer>().material.color = currentColor;
    }


    public void SetSV(float S, float V)
    {
        currentSat = S;
        currentVal = V;

        UpdateOutputImage();
    }

    public void UpdateSVImage()
    {
        currentHue = hueSlider.value;

        for (int y = 0; y < svTexture.height; y++)
        {
            for (int x = 0; x < svTexture.width; x++)
            {
                svTexture.SetPixel(x, y, Color.HSVToRGB(
                    currentHue,
                    (float)x / svTexture.width,
                    (float)y / svTexture.height));
            }
        }

        svTexture.Apply();

        UpdateOutputImage();
    }
}
