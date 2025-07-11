using UnityEngine;

public class Radio : MonoBehaviour, IInteractable
{
    private AudioSource _audioSource;

    [SerializeField] private AudioClip[] songs;
    private int currentSongIndex = 0;

    public bool CanBePickedUp => false;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (songs.Length > 0)
        {
            _audioSource.clip = songs[currentSongIndex];
        }
        _audioSource.Stop();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            NextSong();
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            PreviousSong();
        }
    }

    public void Interact()
    {
        if (_audioSource.clip == null) return;

        if (_audioSource.isPlaying)
            _audioSource.Pause();
        else
            _audioSource.Play();
    }

    private void NextSong()
    {
        if (songs.Length == 0) return;

        currentSongIndex = (currentSongIndex + 1) % songs.Length;
        _audioSource.clip = songs[currentSongIndex];
        _audioSource.Play();
    }

    private void PreviousSong()
    {
        if (songs.Length == 0) return;

        currentSongIndex--;
        if (currentSongIndex < 0)
            currentSongIndex = songs.Length - 1;

        _audioSource.clip = songs[currentSongIndex];
        _audioSource.Play();
    }

    public void Highlight() { }
    public void Unhighlight() { }
}