using System.Collections.Generic;
using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    [Header("All Shape Prefabs")]
    public GameObject[] allShapePrefabs;

    [Header("Easy Preplaced Parent")]
    public Transform easyPlacedParent;

    [Header("Shape Spawner")]
    public ShapeSpawner shapeSpawner;

    private List<string> blockedShapeIds = new List<string>();

    private void Start()
    {
        switch (GameSession.SelectedMode)
        {
            case GameMode.Easy:
                SetupEasyMode();
                break;

            case GameMode.Medium:
                SetupMediumMode();
                break;

            case GameMode.Hard:
                SetupHardMode();
                break;
        }
    }

    void SetupEasyMode()
    {
        int patternIndex = Random.Range(0, 3);

        switch (patternIndex)
        {
            case 0:
                // Pattern 1
                PreplaceShape("PlusShape", new Vector2Int(3, 2));
                PreplaceShape("CornerShape", new Vector2Int(10, 2));
                break;

            case 1:
                // Pattern 2
                PreplaceShape("SqrShape", new Vector2Int(5, 3));
                PreplaceShape("WShape", new Vector2Int(1, 3));
                break;

            case 2:
                // Pattern 3
                PreplaceShape("UShape", new Vector2Int(9, 0));
                PreplaceShape("Shape", new Vector2Int(0, 3));
                break;
        }

        SpawnRemainingShapes();
    }

    void SetupMediumMode()
    {
        SpawnAllShapes();
    }

    void SetupHardMode()
    {
        SpawnAllShapes();
    }

    void PreplaceShape(string targetShapeId, Vector2Int anchor)
    {
        GameObject prefab = FindShapePrefabById(targetShapeId);

        if (prefab == null)
        {
            Debug.LogError("Prefab bulunamadý: " + targetShapeId);
            return;
        }

        GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, easyPlacedParent);

        DraggablePiece draggable = obj.GetComponent<DraggablePiece>();
        ShapeId shapeId = obj.GetComponent<ShapeId>();

        if (draggable == null || shapeId == null)
        {
            Debug.LogError("DraggablePiece veya ShapeId eksik: " + targetShapeId);
            return;
        }

        Vector3 worldPos = BoardManager.Instance.GetCellWorldPosition(anchor.x, anchor.y);
        obj.transform.position = worldPos;

        BoardManager.Instance.ForcePlaceShape(draggable.cells, anchor);
        draggable.MarkAsPrePlaced(anchor);

        blockedShapeIds.Add(shapeId.shapeId);

        draggable.enabled = false;

        Collider2D col = obj.GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;
    }

    void SpawnRemainingShapes()
    {
        List<GameObject> remaining = new List<GameObject>();

        foreach (GameObject prefab in allShapePrefabs)
        {
            ShapeId id = prefab.GetComponent<ShapeId>();
            if (id == null)
                continue;

            if (!blockedShapeIds.Contains(id.shapeId))
            {
                remaining.Add(prefab);
            }
        }

        shapeSpawner.shapePrefabs = remaining.ToArray();
        shapeSpawner.SpawnShapesFromOutside();
    }

    void SpawnAllShapes()
    {
        shapeSpawner.shapePrefabs = allShapePrefabs;
        shapeSpawner.SpawnShapesFromOutside();
    }

    GameObject FindShapePrefabById(string id)
    {
        foreach (GameObject prefab in allShapePrefabs)
        {
            ShapeId shapeId = prefab.GetComponent<ShapeId>();
            if (shapeId != null && shapeId.shapeId == id)
                return prefab;
        }

        return null;
    }
}