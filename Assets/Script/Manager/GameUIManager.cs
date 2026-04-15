using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    [Header("Panels")]
    public GameObject winPanel;
    public GameObject failPanel;

    [Header("Win Texts")]
    public TextMeshProUGUI winTimeText;
    public TextMeshProUGUI bestTimeText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (winPanel != null) winPanel.SetActive(false);
        if (failPanel != null) failPanel.SetActive(false);
    }

    public void ShowWinPanel(float finalTime, float bestTime)
    {
        if (winPanel != null)
            winPanel.SetActive(true);

        if (winTimeText != null)
            winTimeText.text = "Time: " + FormatTime(finalTime);

        if (bestTimeText != null)
            bestTimeText.text = "Best: " + FormatTime(bestTime);

        Time.timeScale = 0f;
    }

    public void ShowFailPanel()
    {
        if (failPanel != null)
            failPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
}