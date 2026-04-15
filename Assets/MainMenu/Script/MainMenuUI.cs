using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    private bool isLoading = false;

    void Start()
    {
        AudioManager.Instance.PlayMusic();
    }

    public void PlayEasy()
    {
        if (isLoading) return;
        StartCoroutine(LoadGameWithSound(GameMode.Easy));
    }

    public void PlayMedium()
    {
        if (isLoading) return;
        StartCoroutine(LoadGameWithSound(GameMode.Medium));
    }

    public void PlayHard()
    {
        if (isLoading) return;
        StartCoroutine(LoadGameWithSound(GameMode.Hard));
    }

    private IEnumerator LoadGameWithSound(GameMode mode)
    {
        isLoading = true;

        GameSession.SelectedMode = mode;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButton();
            yield return new WaitForSecondsRealtime(0.15f);
        }

        SceneManager.LoadScene("GameScene");
    }
}