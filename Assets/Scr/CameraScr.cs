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
                            tile.SelectBuld(CreateBuilding(selectedBuildingType));
                        break;

                    case InteractionMode.CreatingZone:
                        HandleZoneClick(tile);
                        break;
                }
            }
        }
    }

    // --- Розміщення будівель ---

    private void SelectWorkshop()  
    { 
        SetSelectedBuld(BuildingType.Workshopm); 
    }

    private void SelectWarehouse() 
    {
        SetSelectedBuld(BuildingType.Composition); 
    }

    private void SelectDevelopmentCenter() 
    { 
        SetSelectedBuld(BuildingType.Development); 
    }

    private void DeselectBuilding() 
    { 
        SetSelectedBuld(BuildingType.None); 
    }

    public void SetSelectedBuld(BuildingType buld)
    {
        selectedBuildingType = buld;
        currentMode = buld == BuildingType.None ? InteractionMode.None : InteractionMode.PlacingBuilding;
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

    public void StartZoneCreation()
    {
        currentMode = InteractionMode.CreatingZone;
        zoneStartCell = null;
        Debug.Log("Режим створення зони увімкнено. Вибери точку A.");
    }

    public void CancelZoneCreation()
    {
        currentMode = InteractionMode.None;
        zoneStartCell = null;
    }

    private void HandleZoneClick(CellScr clickedCell)
    {
        if (zoneStartCell == null)
        {
            zoneStartCell = clickedCell;
            Debug.Log($"Точка A: {clickedCell.gridPosition}");
            return;
        }

        ZoneClass newZone = CreateZone(zoneStartCell, clickedCell);
        factory.RegisterZone(newZone);

        zoneStartCell = null;
        currentMode = InteractionMode.None;
    }

    private ZoneClass CreateZone(CellScr pointA, CellScr pointB)
    {
        ZoneClass zone = new ZoneClass();

        int minX = Mathf.Min(pointA.gridPosition.x, pointB.gridPosition.x);
        int maxX = Mathf.Max(pointA.gridPosition.x, pointB.gridPosition.x);
        int minY = Mathf.Min(pointA.gridPosition.y, pointB.gridPosition.y);
        int maxY = Mathf.Max(pointA.gridPosition.y, pointB.gridPosition.y);

        foreach (CellScr cell in map.cells)
        {
            if (cell.gridPosition.x >= minX && cell.gridPosition.x <= maxX &&
                cell.gridPosition.y >= minY && cell.gridPosition.y <= maxY)
            {
                zone.cells.Add(cell);
            }
        }

        return zone;
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