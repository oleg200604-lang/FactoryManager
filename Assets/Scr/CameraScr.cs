using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScr : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    private BuildingType selectedBuildingType = BuildingType.None;
    private IBuilding selectedBuld;
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

                if (tile != null && selectedBuildingType != BuildingType.None)
                {
                    tile.SelectBuld(CreateBuilding(selectedBuildingType));
                    print("Selected building type: " + selectedBuildingType);
                }
            }
        }
    }

    public void SetSelectedBuld(BuildingType buld)
    {
        selectedBuildingType = buld;
    }

    private IBuilding CreateBuilding(BuildingType type)
    {
        switch (type)
        {
            case BuildingType.Workshopm:
                return new WorkshopClass();
            case BuildingType.Composition:
                return new WarehouseClass();
            case BuildingType.Development:
                return new DevelopmentClass();
            default:
                return null;
        }
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Vertical");
        float vertical = Input.GetAxisRaw( "Horizontal");

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
