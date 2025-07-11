using UnityEngine;

public class Radio : MonoBehaviour, IInteractable
{
    private AudioSource _audioSource;

    public bool CanBePickedUp => false;
    public bool IsHeld { get; private set; } = false;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        _audioSource.Stop();
    }

    public void Interact()
    {
        if (!_audioSource.isPlaying)
            _audioSource.Play();
        else
            _audioSource.Pause();
    }

    public void Highlight(){}

    public void Unhighlight(){}
}
