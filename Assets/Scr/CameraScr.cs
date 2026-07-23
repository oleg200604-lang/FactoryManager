using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScr : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    private WorkshopClass selectedBuld;
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

                if (tile != null)
                {
                    tile.SelectBuld(selectedBuld);
                }
            }
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

    public void SetSelectedBuld(BuildingType buld)
    {
        if ()
        {
           selectedBuld = workshop;
        }
    }
}
