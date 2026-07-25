using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractionMode
{
    None,
    PlacingBuilding,
    CreatingZone
}

public class CameraScr : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private MapScr map;
    [SerializeField] private FactoryScr factory;

    private BuildingType selectedBuildingType = BuildingType.None;
    private InteractionMode currentMode = InteractionMode.None;
    private ZoneType pendingZoneType = ZoneType.None;
    private CellScr zoneStartCell;

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

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                CellScr tile = hit.collider.GetComponent<CellScr>();
                if (tile == null) return;

                switch (currentMode)
                {
                    case InteractionMode.PlacingBuilding:
                        if (selectedBuildingType != BuildingType.None)
                        {
                            bool wasEmpty = tile.IsEmpty;
                            IBuilding newBuilding = CreateBuilding(selectedBuildingType);
                            tile.SelectBuld(newBuilding);

                            if (wasEmpty && newBuilding is WorkshopClass workshop)
                            {
                                ZoneClass zone = factory.GetZoneForCell(tile);
                                if (zone != null && zone.type == ZoneType.Workshop)
                                    factory.AutoSetupWorkshop(workshop);
                            }
                        }
                        break;

                    case InteractionMode.CreatingZone:
                        HandleZoneClick(tile);
                        break;
                }
            }
        }
    }

    // --- Розміщення будівель ---

    public void SetSelectedBuld(int buildingId)
    {
        selectedBuildingType = (BuildingType)buildingId;
        currentMode = selectedBuildingType == BuildingType.None ? InteractionMode.None : InteractionMode.PlacingBuilding;
    }

    private IBuilding CreateBuilding(BuildingType type)
    {
        switch (type)
        {
            case BuildingType.Workshopm: return new WorkshopClass();
            case BuildingType.Composition: return new WarehouseClass();
            case BuildingType.Development: return new DevelopmentClass();
            default: return null;
        }
    }

    // --- Створення зон ---

    public void StartZoneCreation(int specialization)
    {
        pendingZoneType = (ZoneType)specialization;

        if (pendingZoneType == ZoneType.None)
        {
            Debug.Log("Невірна спеціалізація зони.");
            return;
        }

        currentMode = InteractionMode.CreatingZone;
        zoneStartCell = null;
        Debug.Log($"Режим створення зони '{pendingZoneType}' увімкнено. Вибери точку A.");
    }

    public void CancelZoneCreation()
    {
        currentMode = InteractionMode.None;
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

        List<CellScr> candidateCells = GetCellsBetween(zoneStartCell, clickedCell);

        if (IsOverlappingExistingZone(candidateCells))
        {
            Debug.Log("Ця зона перетинається з уже існуючою. Будівництво скасовано.");
            zoneStartCell = null;
            currentMode = InteractionMode.None;
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
        currentMode = InteractionMode.None;
        pendingZoneType = ZoneType.None;
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