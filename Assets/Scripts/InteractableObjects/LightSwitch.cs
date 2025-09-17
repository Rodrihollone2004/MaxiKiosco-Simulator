using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour, IInteractable
{
    [SerializeField] private List<Light> lightsToControl;
    [SerializeField] private Color highlightColor = Color.green;
    [SerializeField] private float highlightWidth = 1.03f;
    private bool lightsOn = false;
    private bool isEnabled = false;

    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;

    public bool CanBePickedUp => false;
    public bool ShowNameOnHighlight => false;


    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _propBlock = new MaterialPropertyBlock();

        Termica.OnTermicaStateChanged += OnTermicaStateChanged;
    }

    private void OnDestroy()
    {
        Termica.OnTermicaStateChanged -= OnTermicaStateChanged;
    }

    private void Start()
    {
        isEnabled = Termica.IsTermicaOn;
        UpdateLights();

        foreach (Light light in lightsToControl)
        {
            if (light != null)
                light.enabled = false;
        }
    }

    private void OnTermicaStateChanged(bool termicaOn)
    {
        isEnabled = termicaOn;

        if (!termicaOn && lightsOn)
        {
            lightsOn = false;
            UpdateLights();
        }

    }

    public void Interact()
    {
        if (!isEnabled) return;

        lightsOn = !lightsOn;

        UpdateLights();
    }

    private void UpdateLights()
    {
        foreach (Light light in lightsToControl)
        {
            if (light != null)
                light.enabled = lightsOn && isEnabled;
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
