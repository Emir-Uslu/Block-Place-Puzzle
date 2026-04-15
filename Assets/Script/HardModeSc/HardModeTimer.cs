using TMPro;
using UnityEngine;

public class HardModeTimer : MonoBehaviour
{
    public static HardModeTimer Instance;

    [Header("UI")]
    public TextMeshProUGUI timerText;

    [Header("Settings")]
    public float hardModeDuration = 120f;

    private float timeLeft;
    private bool timerRunning;
    private bool isHardMode;

    public float ElapsedTime => hardModeDuration - timeLeft;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        isHardMode = GameSession.SelectedMode == GameMode.Hard;

        if (!isHardMode)
        {
            if (timerText != null)
                timerText.gameObject.SetActive(false);
            return;
        }

        timeLeft = hardModeDuration;
        timerRunning = true;

        if (timerText != null)
            timerText.gameObject.SetActive(true);

        UpdateTimerUI();
    }

    private void Update()
    {
        if (!isHardMode || !timerRunning)
            return;

        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            timerRunning = false;
            UpdateTimerUI();
            OnTimeUp();
            return;
        }

        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        if (timerText == null)
            return;

        int minutes = Mathf.FloorToInt(timeLeft / 60f);
        int seconds = Mathf.FloorToInt(timeLeft % 60f);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void OnTimeUp()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayFail();
        }

        Debug.Log("TIME UP");

        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.ShowFailPanel();
        }
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    public bool IsHardMode()
    {
        return isHardMode;
    }
}