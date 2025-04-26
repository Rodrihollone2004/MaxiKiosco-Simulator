using UnityEngine;
public abstract class Product : MonoBehaviour, IInteractable
{
    [Header("Product Base Settings")]
    [field: SerializeField] public string ProductName { get; protected set; }
    [field: SerializeField] public float Price { get; protected set; }
    [field: SerializeField] public float Pack { get; protected set; }
    [field: SerializeField] public float PackPrice { get; protected set; }
    [SerializeField] protected private Color outlineColor = Color.red;
    [SerializeField] protected private float outlineWidth = 1.03f;

    protected Renderer myRenderer;
    protected MaterialPropertyBlock propBlock;

    protected virtual void Awake()
    {
        myRenderer = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
    }

    public virtual void Interact()
    {
        Debug.Log($"Interactuando con {ProductName} (Precio: ${Price})");
    }

    public virtual void Highlight()
    {
        if (myRenderer == null) return;

        myRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor("_Color", outlineColor);
        propBlock.SetFloat("_Scale", outlineWidth);
        myRenderer.SetPropertyBlock(propBlock);
    }

    public virtual void Unhighlight()
    {
        if (myRenderer == null) return;

        myRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_Scale", 0f);
        myRenderer.SetPropertyBlock(propBlock);
    }
}
