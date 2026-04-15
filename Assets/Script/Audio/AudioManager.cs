using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSource;

    [Header("Clips")]
    public AudioClip buttonClick;
    public AudioClip placeSound;
    public AudioClip winSound;
    public AudioClip failSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        
        AudioSource[] sources = GetComponents<AudioSource>();

        if (sfxSource == null && sources.Length > 0)
            sfxSource = sources[0];

        if (musicSource == null && sources.Length > 1)
            musicSource = sources[1];

        
        if (sfxSource != null)
        {
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
            sfxSource.spatialBlend = 0f; // 2D
            sfxSource.volume = 0.5f;
        }

        if (musicSource != null)
        {
            musicSource.playOnAwake = false;
            musicSource.loop = true;
            musicSource.spatialBlend = 0f; // 2D
            musicSource.volume = 0.2f;
        }
    }

   /* private void Start()
    {
        PlayButton();
    }
    */
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("PlaySFX called but clip is NULL.");
            return;
        }

        if (sfxSource == null)
        {
            Debug.LogWarning("PlaySFX called but sfxSource is NULL.");
            return;
        }

        sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic()
    {
        if (musicSource.clip == null && musicSource.resource == null)
            return;

        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayButton()
    {
        PlaySFX(buttonClick);
    }

    public void PlayPlace()
    {
        PlaySFX(placeSound);
    }

    public void PlayWin()
    {
        PlaySFX(winSound);
    }

    public void PlayFail()
    {
        PlaySFX(failSound);
    }

    public void PlayMusic(AudioClip clip = null)
    {
        if (musicSource == null)
        {
            Debug.LogWarning("PlayMusic called but musicSource is NULL.");
            return;
        }

        if (clip != null)
            musicSource.clip = clip;

        if (musicSource.clip == null)
        {
            Debug.LogWarning("PlayMusic called but music clip is NULL.");
            return;
        }

        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    public void ToggleMusicMute()
    {
        if (musicSource != null)
            musicSource.mute = !musicSource.mute;
    }
}