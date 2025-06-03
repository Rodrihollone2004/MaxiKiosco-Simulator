using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour, IInteractable
{
    [SerializeField] private List<Light> lightsToControl;
    [SerializeField] private Color highlightColor = Color.green;
    [SerializeField] private float highlightWidth = 1.03f;
    private bool lightsOn = false;

    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;

    public bool CanBePickedUp => false;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _propBlock = new MaterialPropertyBlock();
    }
    private void Start()
    {
        foreach (Light light in lightsToControl)
        {
            if (light != null)
                light.enabled = false;
        }
    }

    public void Interact()
    {
        lightsOn = !lightsOn;

        foreach (Light light in lightsToControl)
        {
            if (light != null)
                light.enabled = lightsOn;
        }
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
