using UnityEngine;

public class Radio : MonoBehaviour
{
    private AudioSource _audioSource;
    private bool wasPlaying = false;
    private bool manuallyPaused = false;

    [SerializeField] private AudioClip[] songs;
    private int currentSongIndex = 0;

    public bool CanBePickedUp => false;
    public bool ShowNameOnHighlight => false;


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
        if (_audioSource.isPlaying)
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

        // Verificar si la canción terminó y pasar a la siguiente
        if (!_audioSource.isPlaying && wasPlaying && !manuallyPaused)
        {
            wasPlaying = false;
            NextSong();
        }

        // Actualizar estado si está reproduciendo
        if (_audioSource.isPlaying)
        {
            wasPlaying = true;
        }
    }


    public void StartRadio()
    {
        if (_audioSource.clip == null) return;

        AudioSource backgroundMusic = GameObject.Find("MusicBackground").GetComponent<AudioSource>();

        if (_audioSource.isPlaying)
        {
            _audioSource.Pause();
            manuallyPaused = true;
            wasPlaying = false; // evitamos trigger de siguiente canción
            backgroundMusic.Play();
        }
        else
        {
            _audioSource.Play();
            manuallyPaused = false;
            wasPlaying = true;
            backgroundMusic.Pause();
        }
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