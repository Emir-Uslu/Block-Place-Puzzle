using UnityEngine;

public class DraggablePiece : MonoBehaviour
{
    [Header("Shape Cells")]
    public Vector2Int[] cells;

    [Header("Sorting")]
    public int normalSortingOrder = 1;
    public int draggingSortingOrder = 10;
    public int ghostSortingOffset = -1;

    [Header("Ghost Preview")]
    [Range(0f, 1f)]
    public float ghostAlpha = 0.35f;

    [Header("Board Detection")]
    public float boardDetectionMargin = 0.2f;

    private Camera cam;
    private SpriteRenderer[] renderers;

    private Vector3 trayPosition;
    private Vector3 dragOffset;

    private bool isPlaced;
    private bool wasPlacedBeforeDrag;

    private Vector2Int currentAnchor;
    private Vector2Int anchorBeforeDrag;

    private GameObject ghostRoot;

    private void Start()
    {
        cam = Camera.main;
        renderers = GetComponentsInChildren<SpriteRenderer>();

        trayPosition = transform.position;

        SetSortingOrder(normalSortingOrder);

        CreateGhost();
        HideGhost();
    }

    public bool IsPlaced()
    {
        return isPlaced;
    }

    public void MarkAsPrePlaced(Vector2Int anchor)
    {
        currentAnchor = anchor;
        isPlaced = true;
    }

    public void SetTrayPosition(Vector3 pos)
    {
        trayPosition = pos;

        if (!isPlaced)
            transform.position = trayPosition;
    }

    private void OnMouseDown()
    {
        SetSortingOrder(draggingSortingOrder);

        wasPlacedBeforeDrag = isPlaced;

        if (wasPlacedBeforeDrag)
        {
            anchorBeforeDrag = currentAnchor;
            ClearCurrentPlacement();
        }

        Vector3 mouseWorld = GetMouseWorldPosition();
        dragOffset = transform.position - mouseWorld;

        ShowGhost();
        UpdateGhostPreview();
    }

    private void OnMouseDrag()
    {
        Vector3 mouseWorld = GetMouseWorldPosition();
        Vector3 targetPos = mouseWorld + dragOffset;
        targetPos.z = 0f;

        transform.position = targetPos;
        UpdateGhostPreview();
    }

    private void OnMouseUp()
    {
        HandleDrop();
        HideGhost();
        SetSortingOrder(normalSortingOrder);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouse = Input.mousePosition;
        mouse.z = Mathf.Abs(cam.transform.position.z);

        Vector3 world = cam.ScreenToWorldPoint(mouse);
        world.z = 0f;

        return world;
    }

    private void HandleDrop()
    {
        if (BoardManager.Instance == null)
        {
            ReturnToTray();
            return;
        }

        bool overBoard = IsCurrentlyOverBoardArea();
        Vector2Int rawAnchor = GetNearestAnchorCell();

        if (!overBoard)
        {
            ReturnToTray();
            return;
        }

        if (CanPlaceAt(rawAnchor))
        {
            PlaceAt(rawAnchor);
            return;
        }

        if (wasPlacedBeforeDrag)
        {
            RestorePreviousBoardPlacement();
        }
        else
        {
            ReturnToTray();
        }
    }

    private Vector2Int GetNearestAnchorCell()
    {
        BoardManager board = BoardManager.Instance;

        float rawX = (transform.position.x - board.boardOrigin.x) / board.cellSize;
        float rawY = (board.boardOrigin.y - transform.position.y) / board.cellSize;

        int x = Mathf.RoundToInt(rawX);
        int y = Mathf.RoundToInt(rawY);

        return new Vector2Int(x, y);
    }

    private bool CanPlaceAt(Vector2Int anchor)
    {
        BoardManager board = BoardManager.Instance;

        foreach (Vector2Int cell in cells)
        {
            int x = anchor.x + cell.x;
            int y = anchor.y + cell.y;

            if (!board.IsInsideBoard(x, y))
                return false;

            if (board.IsCellOccupied(x, y))
                return false;
        }

        return true;
    }

    private void PlaceAt(Vector2Int anchor)
    {
        BoardManager board = BoardManager.Instance;
        if (board == null) return;

        Vector3 anchorWorld = board.GetCellWorldPosition(anchor.x, anchor.y);
        transform.position = anchorWorld;

        foreach (Vector2Int cell in cells)
        {
            int x = anchor.x + cell.x;
            int y = anchor.y + cell.y;
            board.SetCellOccupied(x, y, true);
        }

        currentAnchor = anchor;
        isPlaced = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPlace();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPiecePlaced();
        }
    }

    private void RestorePreviousBoardPlacement()
    {
        BoardManager board = BoardManager.Instance;
        if (board == null) return;

        Vector3 anchorWorld = board.GetCellWorldPosition(anchorBeforeDrag.x, anchorBeforeDrag.y);
        transform.position = anchorWorld;

        foreach (Vector2Int cell in cells)
        {
            int x = anchorBeforeDrag.x + cell.x;
            int y = anchorBeforeDrag.y + cell.y;

            if (board.IsInsideBoard(x, y))
                board.SetCellOccupied(x, y, true);
        }

        currentAnchor = anchorBeforeDrag;
        isPlaced = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPiecePlaced();
        }
    }

    private void ReturnToTray()
    {
        transform.position = trayPosition;
        isPlaced = false;
    }

    private void ClearCurrentPlacement()
    {
        if (!isPlaced || BoardManager.Instance == null)
            return;

        BoardManager board = BoardManager.Instance;

        foreach (Vector2Int cell in cells)
        {
            int x = currentAnchor.x + cell.x;
            int y = currentAnchor.y + cell.y;

            if (board.IsInsideBoard(x, y))
                board.SetCellOccupied(x, y, false);
        }

        isPlaced = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPieceRemoved();
        }
    }

    private bool IsCurrentlyOverBoardArea()
    {
        BoardManager board = BoardManager.Instance;
        if (board == null) return false;

        Rect rect = board.GetBoardWorldRect();
        rect.xMin -= boardDetectionMargin;
        rect.xMax += boardDetectionMargin;
        rect.yMin -= boardDetectionMargin;
        rect.yMax += boardDetectionMargin;

        foreach (Vector2Int cell in cells)
        {
            Vector3 cellWorld = transform.position + new Vector3(
                cell.x * board.cellSize,
                -cell.y * board.cellSize,
                0f
            );

            if (rect.Contains(cellWorld))
                return true;
        }

        return false;
    }

    private void SetSortingOrder(int baseOrder)
    {
        if (renderers == null) return;

        foreach (SpriteRenderer sr in renderers)
            sr.sortingOrder = baseOrder;
    }

    private void CreateGhost()
    {
        if (ghostRoot != null)
            Destroy(ghostRoot);

        ghostRoot = new GameObject(name + "_Ghost");
        ghostRoot.transform.position = transform.position;
        ghostRoot.transform.rotation = transform.rotation;
        ghostRoot.transform.localScale = transform.localScale;

        SpriteRenderer[] originals = GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer original in originals)
        {
            if (original.transform == transform)
                continue;

            GameObject ghostPart = new GameObject(original.gameObject.name + "_GhostPart");
            ghostPart.transform.SetParent(ghostRoot.transform, false);

            ghostPart.transform.localPosition = original.transform.localPosition;
            ghostPart.transform.localRotation = original.transform.localRotation;
            ghostPart.transform.localScale = original.transform.localScale;

            SpriteRenderer ghostSr = ghostPart.AddComponent<SpriteRenderer>();
            ghostSr.sprite = original.sprite;
            ghostSr.color = new Color(original.color.r, original.color.g, original.color.b, ghostAlpha);
            ghostSr.sortingLayerID = original.sortingLayerID;
            ghostSr.sortingOrder = original.sortingOrder + ghostSortingOffset;
            ghostSr.drawMode = original.drawMode;
            ghostSr.flipX = original.flipX;
            ghostSr.flipY = original.flipY;
            ghostSr.maskInteraction = original.maskInteraction;
            ghostSr.spriteSortPoint = original.spriteSortPoint;
            ghostSr.material = original.sharedMaterial;
        }
    }

    private void UpdateGhostPreview()
    {
        if (ghostRoot == null || BoardManager.Instance == null)
            return;

        bool overBoard = IsCurrentlyOverBoardArea();
        Vector2Int rawAnchor = GetNearestAnchorCell();

        if (!overBoard || !CanPlaceAt(rawAnchor))
        {
            HideGhost();
            return;
        }

        ShowGhost();

        Vector3 anchorWorld = BoardManager.Instance.GetCellWorldPosition(rawAnchor.x, rawAnchor.y);
        ghostRoot.transform.position = anchorWorld;
        ghostRoot.transform.rotation = transform.rotation;
        ghostRoot.transform.localScale = transform.localScale;
    }

    private void ShowGhost()
    {
        if (ghostRoot != null)
            ghostRoot.SetActive(true);
    }

    private void HideGhost()
    {
        if (ghostRoot != null)
            ghostRoot.SetActive(false);
    }

    private void OnDestroy()
    {
        if (ghostRoot != null)
            Destroy(ghostRoot);
    }
}