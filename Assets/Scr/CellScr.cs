using UnityEngine;

public class CellScr : MonoBehaviour
{
    public Vector2Int gridPosition;

    [SerializeField] private Renderer cellRenderer;

    private void Awake()
    {
        if (cellRenderer == null)
            cellRenderer = GetComponent<Renderer>();
    }

    public void SetHighlight(Color color)
    {
        if (cellRenderer != null)
            cellRenderer.material.color = color;
    }

    public void ClearHighlight()
    {
        SetHighlight(Color.white);
    }
}