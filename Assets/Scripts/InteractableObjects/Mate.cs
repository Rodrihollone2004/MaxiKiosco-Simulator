using UnityEngine;

public class Mate : MonoBehaviour, IInteractable
{
    private AudioSource _audioSource;

    public bool CanBePickedUp => false;

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
        if (_audioSource.clip == null) return;

        if (_audioSource.isPlaying)
            _audioSource.Stop();
        else
            _audioSource.Play();
    }

    public void Highlight(){}

    public void Unhighlight(){}
}
