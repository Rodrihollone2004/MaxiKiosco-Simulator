using UnityEngine;
using UnityEngine.UI;
using SFB;
using System.IO;
using System.Collections;

public class ImageLoader : MonoBehaviour
{
    public RawImage targetImage;

    public void OpenFileExplorer()
    {
        var extensions = new[] {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg"),
        };

        var paths = StandaloneFileBrowser.OpenFilePanel("Selecciona una imagen", "", extensions, false);

        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            StartCoroutine(LoadImage(paths[0]));
        }
    }

    private IEnumerator LoadImage(string path)
    {
        byte[] imageData = File.ReadAllBytes(path);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(imageData);

        targetImage.texture = tex;
        targetImage.uvRect = new Rect(0, 0, 1, 1);

        targetImage.rectTransform.localScale = Vector3.one;
        targetImage.rectTransform.anchorMin = Vector2.zero;
        targetImage.rectTransform.anchorMax = Vector2.one;
        targetImage.rectTransform.offsetMin = Vector2.zero;
        targetImage.rectTransform.offsetMax = Vector2.zero;

        yield return null;
    }
}