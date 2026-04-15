using TMPro;
using UnityEngine;

public class BestTimeUI : MonoBehaviour
{
    public TextMeshProUGUI bestTimeText;

    private void Start()
    {
        if (GameSession.SelectedMode != GameMode.Hard)
        {
            if (bestTimeText != null)
                bestTimeText.gameObject.SetActive(false);
            return;
        }

        float best = PlayerPrefs.GetFloat("BestTime_Hard", -1f);

        if (best < 0f || best == float.MaxValue)
        {
            bestTimeText.text = "Best: --:--";
            return;
        }

        int minutes = Mathf.FloorToInt(best / 60f);
        int seconds = Mathf.FloorToInt(best % 60f);

        bestTimeText.text = $"Best: {minutes:00}:{seconds:00}";
    }
}