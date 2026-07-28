using System.Collections.Generic;
using UnityEngine;

public class CameraScr : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private MapScr map;
    [SerializeField] private FactoryScr factory;

    private ZoneType pendingZoneType = ZoneType.None;
    private CellScr zoneStartCell;
    private bool isCreatingZone;

    public float moveSpeed = 5f;
    public float zoomSpeed = 5f;
    public float minZoom = 5f;
    public float maxZoom = 20f;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        HandleMovement();
        HandleZoom();

        if (isCreatingZone && Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                CellScr tile = hit.collider.GetComponent<CellScr>();
                if (tile != null)
                    HandleZoneClick(tile);
            }
        }
    }

    public void StartZoneCreation(int specialization)
    {
        pendingZoneType = (ZoneType)specialization;

        if (pendingZoneType == ZoneType.None)
        {
            Debug.Log("Невірна спеціалізація зони.");
            return;
        }

        isCreatingZone = true;
        zoneStartCell = null;
        Debug.Log($"Режим створення зони '{pendingZoneType}' увімкнено. Вибери точку A.");
    }

    public void CancelZoneCreation()
    {
        isCreatingZone = false;
        zoneStartCell = null;
        pendingZoneType = ZoneType.None;
    }

    private void HandleZoneClick(CellScr clickedCell)
    {
        if (zoneStartCell == null)
        {
            zoneStartCell = clickedCell;
            Debug.Log($"Точка A: {clickedCell.gridPosition}");
            return;
        }

        Vector2Int size = GetZoneSize(zoneStartCell, clickedCell);
        Vector2Int minSize = GetMinimumZoneSize(pendingZoneType);

        if (size.x < minSize.x || size.y < minSize.y)
        {
            Debug.Log($"Зона замала: {size.x}x{size.y}, потрібно щонайменше {minSize.x}x{minSize.y}. Вибери іншу точку B.");
            return;
        }

        List<CellScr> candidateCells = GetCellsBetween(zoneStartCell, clickedCell);

        if (IsOverlappingExistingZone(candidateCells))
        {
            Debug.Log("Ця зона перетинається з уже існуючою. Створення скасовано.");
            zoneStartCell = null;
            isCreatingZone = false;
            pendingZoneType = ZoneType.None;
            return;
        }

        ZoneClass newZone = new ZoneClass
        {
            type = pendingZoneType,
            cells = candidateCells
        };

        factory.RegisterZone(newZone);
        HighlightZone(newZone);

        zoneStartCell = null;
        isCreatingZone = false;
        pendingZoneType = ZoneType.None;
    }

    private Vector2Int GetZoneSize(CellScr pointA, CellScr pointB)
    {
        int width = Mathf.Abs(pointA.gridPosition.x - pointB.gridPosition.x) + 1;
        int height = Mathf.Abs(pointA.gridPosition.y - pointB.gridPosition.y) + 1;
        return new Vector2Int(width, height);
    }

    private Vector2Int GetMinimumZoneSize(ZoneType type)
    {
        switch (type)
        {
            case ZoneType.Workshop: return new Vector2Int(2, 2);
            case ZoneType.Warehouse: return new Vector2Int(3, 3);
            case ZoneType.Development: return new Vector2Int(2, 2);
            default: return new Vector2Int(1, 1);
        }
    }

    private List<CellScr> GetCellsBetween(CellScr pointA, CellScr pointB)
    {
        List<CellScr> result = new List<CellScr>();

        int minX = Mathf.Min(pointA.gridPosition.x, pointB.gridPosition.x);
        int maxX = Mathf.Max(pointA.gridPosition.x, pointB.gridPosition.x);
        int minY = Mathf.Min(pointA.gridPosition.y, pointB.gridPosition.y);
        int maxY = Mathf.Max(pointA.gridPosition.y, pointB.gridPosition.y);

        foreach (CellScr cell in map.cells)
        {
            if (cell.gridPosition.x >= minX && cell.gridPosition.x <= maxX &&
                cell.gridPosition.y >= minY && cell.gridPosition.y <= maxY)
            {
                result.Add(cell);
            }
        }

        return result;
    }

    private bool IsOverlappingExistingZone(List<CellScr> candidateCells)
    {
        foreach (CellScr cell in candidateCells)
        {
            if (factory.IsCellInAnyZone(cell))
                return true;
        }

        return false;
    }

    private void HighlightZone(ZoneClass zone)
    {
        Color zoneColor = GetZoneColor(zone.type);

        foreach (CellScr cell in zone.cells)
        {
            cell.SetHighlight(zoneColor);
        }
    }

    private Color GetZoneColor(ZoneType type)
    {
        switch (type)
        {
            case ZoneType.Workshop: return new Color(1f, 0.6f, 0.2f);
            case ZoneType.Warehouse: return new Color(0.2f, 0.6f, 1f);
            case ZoneType.Development: return new Color(0.6f, 1f, 0.4f);
            default: return Color.white;
        }
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Vertical");
        float vertical = Input.GetAxisRaw("Horizontal");

        Vector3 right = new Vector3(1, 0, -1);
        Vector3 forward = new Vector3(1, 0, 1);

        Vector3 moveDirection = (-right * horizontal + forward * vertical).normalized;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        mainCamera.orthographicSize -= scrollInput * zoomSpeed;
        mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, minZoom, maxZoom);
    }
}