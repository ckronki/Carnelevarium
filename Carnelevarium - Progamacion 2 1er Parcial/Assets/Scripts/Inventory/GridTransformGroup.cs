using UnityEngine;

[ExecuteAlways]
public class GridTransformGroup : MonoBehaviour
{
    [Header("Configuración del Grid")]
    [SerializeField] private int columns = 4;
    [SerializeField] private Vector2 cellSize = new Vector2(1, 1);
    [SerializeField] private Vector2 spacing = new Vector2(0.1f, 0.1f);

    void OnValidate()
    {
        UpdateGridLayout();
    }

    void UpdateGridLayout()
    {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            int row = i / columns;
            int column = i % columns;

            Vector3 position = new Vector3(
                column * (cellSize.x + spacing.x),
                -row * (cellSize.y + spacing.y),
                0
            );

            child.localPosition = position;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        foreach (Transform child in transform)
        {
            Vector3 center = child.position;
            Vector3 size = new Vector3(cellSize.x, cellSize.y, 0.1f);
            Gizmos.DrawWireCube(center, size);
        }
    }
}
