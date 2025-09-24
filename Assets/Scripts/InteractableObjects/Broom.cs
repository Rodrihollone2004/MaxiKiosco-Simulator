using UnityEngine;

public class Broom : MonoBehaviour, IInteractable
{
    [SerializeField] private Color highlightColor = Color.green;
    [SerializeField] private float highlightWidth = 1.03f;

    [Header("Pickup Offset")]
    public Vector3 holdOffset = new Vector3(0.2f, -0.3f, 0.5f);
    public Vector3 holdRotation = new Vector3(0f, 90f, 0f);

    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;

    public bool CanBePickedUp => true;
    public bool ShowNameOnHighlight => false;

    public bool IsHeld { get; private set; } = false;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _propBlock = new MaterialPropertyBlock();
    }

    public void Interact() { }

    public void Highlight()
    {
        if (!_renderer) return;
        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor("_Color", highlightColor);
        _propBlock.SetFloat("_Scale", highlightWidth);
        _renderer.SetPropertyBlock(_propBlock);
    }

    public void Unhighlight()
    {
        if (!_renderer) return;
        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetFloat("_Scale", 0f);
        _renderer.SetPropertyBlock(_propBlock);
    }

    public void SetHeld(bool held)
    {
        IsHeld = held;
    }
}
