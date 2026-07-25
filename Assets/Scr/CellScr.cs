using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CellScr : MonoBehaviour
{
    public IBuilding buld;
    public Vector2Int gridPosition;

    [SerializeField] private Renderer cellRenderer;

    public bool IsEmpty => buld == null;

    private void Awake()
    {
        if (cellRenderer == null)
            cellRenderer = GetComponent<Renderer>();
    }

    public void SelectBuld(IBuilding newBuld)
    {
        if (newBuld == null) return;

        if (!IsEmpty)
        {
            Debug.Log($"Клітинка {name} вже зайнята будівлею {buld.GetType().Name}.");
            return;
        }

        buld = newBuld;
        Debug.Log($"Будівлю {newBuld.GetType().Name} розміщено на клітинці {name}.");
    }

    public void RemoveBuld()
    {
        buld = null;
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