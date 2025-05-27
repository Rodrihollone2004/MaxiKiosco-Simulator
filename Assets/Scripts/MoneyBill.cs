using System;
using UnityEngine;

public class MoneyBill : MonoBehaviour, IInteractable
{
    public static event Action<int, bool> onPickBill;
    [SerializeField] private int billValue = 1;
    [SerializeField] private Color highlightColor = Color.green;
    [SerializeField] private float highlightWidth = 1.03f;

    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;

    public bool CanBePickedUp => false;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _propBlock = new MaterialPropertyBlock();
    }

    public void Interact()
    {
        onPickBill?.Invoke(billValue, true);
        Debug.Log($"Interactuando con billete de: {billValue}");
    }

    public void InteractSubtract()
    {
        onPickBill?.Invoke(billValue, false);
        Debug.Log($"Restando billete de: {billValue}");
    }

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
}
