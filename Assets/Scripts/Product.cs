using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Product : MonoBehaviour, IInteractable
{
    [SerializeField] private string productName;
    [SerializeField] private float price;
    [SerializeField] private Color outlineColor = Color.magenta;
    [SerializeField] private float outlineWidth = 1.03f;

    private Renderer myRenderer;
    private MaterialPropertyBlock propBlock;

    public float Price { get => price; set => price = value; }

    private void Awake()
    {
        myRenderer = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
    }

    public void Interact()
    {
        Debug.Log($"Interactuando con {productName} (Precio: ${price})");
    }

    public void Highlight()
    {
        if (myRenderer == null) return;

        myRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor("_Color", outlineColor);
        propBlock.SetFloat("_Scale", outlineWidth);
        myRenderer.SetPropertyBlock(propBlock);
    }

    public void Unhighlight()
    {
        if (myRenderer == null) return;

        myRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_Scale", 0f);
        myRenderer.SetPropertyBlock(propBlock);
    }
}
