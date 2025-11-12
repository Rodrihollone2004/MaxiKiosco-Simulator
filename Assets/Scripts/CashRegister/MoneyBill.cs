using System;
using UnityEngine;

public class MoneyBill : MonoBehaviour, IInteractable
{
    public static event Action<int, bool> onPickBill;
    [SerializeField] private int billValue = 1;
    [SerializeField] private Color highlightColor = Color.green;
    [SerializeField] private float highlightWidth = 1.03f;
    [SerializeField] private PlayerCam playerCam;

    [SerializeField] private AudioClip pickSound;
    private AudioSource audioSource;

    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;

    public bool CanBePickedUp => false;
    public bool ShowNameOnHighlight => false;


    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _propBlock = new MaterialPropertyBlock();
        audioSource = GetComponent<AudioSource>();
    }

    public void Interact()
    {
        if (playerCam.IsInCashRegister && CashRegisterContext.IsClientInCashRegister())
        {
            if (audioSource != null && pickSound != null)
                audioSource.PlayOneShot(pickSound);

            onPickBill?.Invoke(billValue, true);
        }
    }

    public void InteractSubtract()
    {
        if (playerCam.IsInCashRegister && CashRegisterContext.IsClientInCashRegister())
            onPickBill?.Invoke(billValue, false);
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
