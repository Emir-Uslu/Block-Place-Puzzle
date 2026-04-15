using UnityEngine;

public class BoardVisualGenerator : MonoBehaviour
{
    public GameObject cellPrefab;
    public Transform visualParent;

    private void Start()
    {
        GenerateBoard();
    }

    public void GenerateBoard()
    {
        BoardManager board = BoardManager.Instance;

        if (board == null)
        {
            Debug.LogError("BoardManager bulunamadý.");
            return;
        }

        if (cellPrefab == null)
        {
            Debug.LogError("Cell Prefab atanmadý.");
            return;
        }

        if (visualParent == null)
        {
            visualParent = transform;
        }

        ClearOldCells();

        for (int y = 0; y < board.height; y++)
        {
            for (int x = 0; x < board.width; x++)
            {
                Vector3 pos = board.GetCellWorldPosition(x, y);
                GameObject cell = Instantiate(cellPrefab, pos, Quaternion.identity, visualParent);
                cell.name = $"Cell ({x},{y})";
            }
        }
    }

    private void ClearOldCells()
    {
        if (visualParent == null) return;

        for (int i = visualParent.childCount - 1; i >= 0; i--)
        {
            Destroy(visualParent.GetChild(i).gameObject);
        }
    }
}