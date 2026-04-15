using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private bool levelCompleted = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic();
        }
    }

    public void OnPiecePlaced()
    {
        Debug.Log("Piece placed");
        CheckLevelComplete();
    }

    public void OnPieceRemoved()
    {
        Debug.Log("Piece removed");
    }

    private void CheckLevelComplete()
    {
        if (levelCompleted)
            return;

        DraggablePiece[] pieces = FindObjectsOfType<DraggablePiece>();

        if (pieces.Length == 0)
            return;

        foreach (DraggablePiece piece in pieces)
        {
            if (!piece.IsPlaced())
                return;
        }

        levelCompleted = true;
        LevelComplete();
    }

    private void LevelComplete()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayWin();
        }

        Debug.Log("LEVEL COMPLETE");

        if (HardModeTimer.Instance != null && HardModeTimer.Instance.IsHardMode())
        {
            HardModeTimer.Instance.StopTimer();

            float finalTime = HardModeTimer.Instance.ElapsedTime;
            float bestTime = SaveBestTime(finalTime);

            if (GameUIManager.Instance != null)
            {
                GameUIManager.Instance.ShowWinPanel(finalTime, bestTime);
            }
        }
        else
        {
            if (GameUIManager.Instance != null)
            {
                GameUIManager.Instance.ShowWinPanel(0f, 0f);
            }
        }
    }

    private float SaveBestTime(float newTime)
    {
        float oldBest = PlayerPrefs.GetFloat("BestTime_Hard", float.MaxValue);

        if (newTime < oldBest)
        {
            PlayerPrefs.SetFloat("BestTime_Hard", newTime);
            PlayerPrefs.Save();

            Debug.Log("NEW BEST TIME: " + newTime);
            return newTime;
        }

        Debug.Log("Finished Time: " + newTime + " | Best: " + oldBest);
        return oldBest;
    }
}