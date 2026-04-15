using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    [Header("Board Settings")]
    public int width = 11;
    public int height = 5;
    public float cellSize = 0.55f;
    public Vector2 boardOrigin = new Vector2(-2.75f, 4.1f);

    private bool[,] occupied;

    private void Awake()
    {
        Instance = this;
        occupied = new bool[width, height];
    }

    public Vector3 GetCellWorldPosition(int x, int y)
    {
        return new Vector3(
            boardOrigin.x + x * cellSize,
            boardOrigin.y - y * cellSize,
            0f
        );
    }

    public bool IsInsideBoard(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public bool IsCellOccupied(int x, int y)
    {
        if (!IsInsideBoard(x, y))
            return true;

        return occupied[x, y];
    }

    public void SetCellOccupied(int x, int y, bool value)
    {
        if (!IsInsideBoard(x, y))
            return;

        occupied[x, y] = value;
    }

    public void ClearBoard()
    {
        occupied = new bool[width, height];
    }

    public Rect GetBoardWorldRect()
    {
        float left = boardOrigin.x - cellSize * 0.5f;
        float top = boardOrigin.y + cellSize * 0.5f;
        float right = boardOrigin.x + (width - 1) * cellSize + cellSize * 0.5f;
        float bottom = boardOrigin.y - (height - 1) * cellSize - cellSize * 0.5f;

        return Rect.MinMaxRect(left, bottom, right, top);
    }

    public void ForcePlaceShape(Vector2Int[] shapeCells, Vector2Int anchor)
    {
        foreach (Vector2Int cell in shapeCells)
        {
            int x = anchor.x + cell.x;
            int y = anchor.y + cell.y;

            if (IsInsideBoard(x, y))
            {
                SetCellOccupied(x, y, true);
            }
        }
    }
}