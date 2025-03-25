using UnityEngine;

public class OutlineHighlighter : MonoBehaviour
{
    [SerializeField] private Material outlineMaterial;

    private Renderer objectRenderer;
    private Material[] originalMaterials;
    private Material[] highlightedMaterials;
    private bool isHighlighted = false;

    private void Awake()
    {
        objectRenderer = GetComponent<Renderer>();

        if (objectRenderer != null)
        {
            // Guardamos los materiales originales
            originalMaterials = objectRenderer.sharedMaterials;

            // Creamos el array de materiales con el outline
            highlightedMaterials = new Material[originalMaterials.Length + 1];
            for (int i = 0; i < originalMaterials.Length; i++)
            {
                highlightedMaterials[i] = originalMaterials[i];
            }
            highlightedMaterials[highlightedMaterials.Length - 1] = outlineMaterial;
        }
    }

    public void Highlight()
    {
        if (!isHighlighted && objectRenderer != null)
        {
            objectRenderer.materials = highlightedMaterials;
            isHighlighted = true;
        }
    }

    public void RemoveHighlight()
    {
        if (isHighlighted && objectRenderer != null)
        {
            objectRenderer.materials = originalMaterials;
            isHighlighted = false;
        }
    }
}
