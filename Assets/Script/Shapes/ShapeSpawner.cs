using UnityEngine;

public class ShapeSpawner : MonoBehaviour
{
    public GameObject[] shapePrefabs;

    [Header("Grid Layout")]
    public int columns = 3;
    public float slotWidth = 2.2f;
    public float slotHeight = 1.8f;
    public Vector2 startPosition = new Vector2(-2.2f, 0.8f);

    public void SpawnShapesFromOutside()
    {
        ClearOldChildren();
        SpawnShapes();
    }

    void SpawnShapes()
    {
        for (int i = 0; i < shapePrefabs.Length; i++)
        {
            int row = i / columns;
            int col = i % columns;

            Vector3 slotCenter = new Vector3(
                startPosition.x + col * slotWidth,
                startPosition.y - row * slotHeight,
                0f
            );

            GameObject shape = Instantiate(shapePrefabs[i], Vector3.zero, Quaternion.identity, transform);

            CenterShapeInSlot(shape, slotCenter);

            DraggablePiece dp = shape.GetComponent<DraggablePiece>();
            if (dp != null)
            {
                dp.SetTrayPosition(shape.transform.position);
            }
        }
    }

    void CenterShapeInSlot(GameObject shape, Vector3 slotCenter)
    {
        SpriteRenderer[] renderers = shape.GetComponentsInChildren<SpriteRenderer>();

        if (renderers.Length == 0)
        {
            shape.transform.position = slotCenter;
            return;
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        Vector3 offset = bounds.center - shape.transform.position;
        shape.transform.position = slotCenter - offset;
    }

    void ClearOldChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}