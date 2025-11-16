using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sound Effects")]
    public AudioClip buttonTapSfx;
    public AudioClip matchSfx;
    public AudioClip mismatchSfx;
    public AudioClip gameOverSfx;

    public AudioClip flipSfx;

    private AudioSource audioSource;

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayButtonTap()
    {
        audioSource.PlayOneShot(buttonTapSfx);
    }

    public void PlayMatch()
    {
        audioSource.PlayOneShot(matchSfx);
    }

    public void PlayMismatch()
    {
        audioSource.PlayOneShot(mismatchSfx);
    }

    public void PlayGameOver()
    {
        audioSource.PlayOneShot(gameOverSfx);
    }

    public void PlayFlip()
    {
        audioSource.PlayOneShot(flipSfx);
    }

    // General
    public void PlaySFX(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
