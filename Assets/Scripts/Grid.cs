using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
public class Grid : MonoBehaviour
{
    static public Grid grid = null;

    public Vector2Int gridSize; //cannot be less than 5x5, force checked and corrected in Awake();
    public const int cellSize = 1;

    BoxCollider gridCollider;
    SpriteRenderer baseGridRenderer;

    //Layers
    GridLayer<int> cellOccupationStatus;

    //Methods
    void Awake()
    {
        //Singletone check
        if (grid == null)
        {
            grid = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        //Force min grid
        gridSize.x = Mathf.Max(gridSize.x, 5);
        gridSize.y = Mathf.Max(gridSize.y, 5);

        //Force Centering
        this.transform.position = Vector3.zero;

        //Set references.
        gridCollider = this.gameObject.GetComponent<BoxCollider>();
        baseGridRenderer = this.transform.Find("GridView").GetComponent<SpriteRenderer>();

        //Other init stuff
        UpdateGridBoundary();
        InitializeGridLayers();
    }

    void InitializeGridLayers()
    {
        cellOccupationStatus = new GridLayer<int>(gridSize);
    }

    void UpdateGridBoundary()
    {
        baseGridRenderer.size = gridSize;

         //update collider boundary
        Vector3 _centre = this.transform.position;
        _centre.y = -0.5f; //half the thickness, so the collider is spawned with its surface (which the raycast will hit matches the sufrace of the grid).
        
        Vector3 boundary = new Vector3(gridSize.x * cellSize, gridSize.y * cellSize, 1.0f);
        
        float colliderPadding = cellSize * 5.0f; //Because object placement preview requires collider.
        gridCollider.center = _centre;
        gridCollider.size = new Vector3(gridSize.x * cellSize + colliderPadding,  gridSize.y * cellSize + colliderPadding, 1.0f);
    }

    public void ToggleBaseGridView(bool state)
    {

    }

    public void SetCellOccupiedState (Cell cell, bool isOccupied)
    {
        SetCellOccupationState(cell.cellID[0], cell.cellID[1], isOccupied? 1 : 0);
    }

    public void SetCellOccupiedState (uint cellID_x, uint cellID_y, bool isOccupied)
    {
        SetCellOccupationState(cellID_x, cellID_y, isOccupied? 1 : 0);
    }

    void SetCellOccupationState(uint cellID_x, uint cellID_y, int state)
    {
        cellOccupationStatus.SetCellValue(cellID_x, cellID_y, state);
    }


}

public class Cell
{
    public uint[] cellID = new uint[2];
    public Vector3 cellCentre;
    public bool isOccupied = false;
}

public class GridLayer<T>
{
    public T[,] grid;

    public GridLayer(uint width, uint height)
    {
        grid = new T[width, height];
    }

    public GridLayer(Vector2Int size)
    {
        grid = new T[size.x, size.y];
    }

    public T GetCellValue(uint cellID_x, uint cellID_y) //Returns default value of assigned type if index outside array range
    {
        if (cellID_x >= grid.GetLength(0) || cellID_y >= grid.GetLength(1))
            return default(T);
        
        return grid[cellID_x, cellID_y];
    }

     public ref T GetCellRef(uint cellID_x, uint cellID_y) 
    {
        return ref grid[cellID_x, cellID_y];
    }

    public void SetCellValue(uint cellID_x, uint cellID_y, T value)
    {
        if (cellID_x >= grid.GetLength(0) || cellID_y >= grid.GetLength(1))
            return;
        grid[cellID_x, cellID_y] = value;
    }

    public Vector2Int GridSize()
    {
        return new Vector2Int(grid.GetLength(0), grid.GetLength(1));
    }

    public bool CopyToLayer(GridLayer<T> targetLayer)
    {
        if (GridSize() != targetLayer.GridSize())
            return false;

        for (uint i = 0; i < grid.GetLength(0); i++)
            for (uint j = 0; j < grid.GetLength(1); j++)
                targetLayer.SetCellValue(i, j, grid[i,j]);

        return true;
    }
}