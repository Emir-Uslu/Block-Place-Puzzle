using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    public static PauseMenuUI Instance;

    [Header("Panel")]
    public GameObject pausePanel;

    [Header("Optional")]
    public GameObject topBar;

    [Header("Music UI")]
    public TextMeshProUGUI musicButtonText;

    private bool isPaused = false;
    private bool musicOn = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        musicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;

        ApplyMusicState();
        UpdateMusicButtonText();
    }

    public void TogglePause()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButton();

        if (isPaused)
            ResumeGameInternal();
        else
            PauseGameInternal();
    }

    public void PauseGame()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButton();

        PauseGameInternal();
    }

    public void ResumeGame()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButton();

        ResumeGameInternal();
    }

    public void ToggleMusic()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButton();

        musicOn = !musicOn;

        PlayerPrefs.SetInt("MusicOn", musicOn ? 1 : 0);
        PlayerPrefs.Save();

        ApplyMusicState();
        UpdateMusicButtonText();
    }

    public void RestartLevel()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButton();

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitToMenu()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButton();

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void PauseGameInternal()
    {
        isPaused = true;

        if (pausePanel != null)
            pausePanel.SetActive(true);

        if (topBar != null)
            topBar.SetActive(false);

        Time.timeScale = 0f;
    }

    private void ResumeGameInternal()
    {
        isPaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (topBar != null)
            topBar.SetActive(true);

        Time.timeScale = 1f;
    }

    private void ApplyMusicState()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.musicSource != null)
        {
            AudioManager.Instance.musicSource.mute = !musicOn;
        }
    }

    private void UpdateMusicButtonText()
    {
        if (musicButtonText != null)
        {
            musicButtonText.text = musicOn ? "On" : "Off";
        }
    }
}