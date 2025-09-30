using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Termica : MonoBehaviour, IInteractable
{
    [SerializeField] private Color highlightColor = Color.green;
    [SerializeField] private float highlightWidth = 1.03f;

    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;
    private Animator animator;

    [SerializeField] private string doorAnimationParam = "OpenDoor";
    [SerializeField] private string knobAnimationParam = "LiftKnob";

    public static Action<bool> OnTermicaStateChanged;

    public bool CanBePickedUp => false;
    public bool ShowNameOnHighlight => false;

    public static bool IsTermicaOn { get; private set; }


    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        animator = GetComponent<Animator>();
        _propBlock = new MaterialPropertyBlock();
    }
    private void Start()
    {
        IsTermicaOn = false;
    }

    public void Interact()
    {
        if (IsTermicaOn == false)
        {
            IsTermicaOn = true;

            if (animator != null)
            {
                animator.SetBool(doorAnimationParam, IsTermicaOn);
                animator.SetBool(knobAnimationParam, IsTermicaOn);
            }

            OnTermicaStateChanged?.Invoke(IsTermicaOn);

            TutorialContent.Instance.CompleteStep(3);
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
