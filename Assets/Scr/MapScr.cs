using System.Collections.Generic;
using UnityEngine;

public class MapScr : MonoBehaviour
{
    public GameObject cell;
    public List<CellScr> cells = new List<CellScr>();
    public Vector2Int mapSize;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateMap();
        }
    }

    public void GenerateMap()
    {
        cells.Clear();

        for (int y = 0; y < mapSize.y; y++)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                GameObject newCell = Instantiate(cell, new Vector3(x, 0, y), Quaternion.identity);

                cells.Add(newCell.GetComponent<CellScr>());
            }
        }
    }

}