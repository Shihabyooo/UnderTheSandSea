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
        //InitializeGridLayers();
    }

    public void Initialize()
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

    public Cell SampleForCell(Vector3 position)
    {
        Cell cell = new Cell();
        
        Vector3 offset = position - this.transform.position;

        if (Mathf.Abs(offset.x) > (float)(gridSize.x * cellSize) / 2.0f || Mathf.Abs(offset.y) > (float)(gridSize.y * cellSize) / 2.0f)
        {
            //print ("Sampled outside boundary"); //test
            return null;
        }

        float[] rawDistInCells = {  ((offset.x + (Mathf.Sign(offset.x) * gridSize.x%2 * (float)cellSize / 2.0f)) / (float)cellSize),
                                    ((offset.y + (Mathf.Sign(offset.y) * gridSize.y%2 * (float)cellSize / 2.0f)) / (float)cellSize)};

        int[] rawDistSigns = {(int)Mathf.Sign(rawDistInCells[0]), (int)Mathf.Sign(rawDistInCells[1])};

        Vector2Int distInCells = new Vector2Int(Mathf.FloorToInt(Mathf.Abs(rawDistInCells[0])) * rawDistSigns[0],
                                                Mathf.FloorToInt(Mathf.Abs(rawDistInCells[1])) * rawDistSigns[1]);
        
        //Compute position of cell's centre.
        //TODO simplify the eqns bellow.
        cell.cellCentre.x = (float)(distInCells.x * cellSize);
        cell.cellCentre.x += (1 - gridSize.x%2) * (float)cellSize / 2.0f * rawDistSigns[0]; //special consideration for even numbered cell x count.

        cell.cellCentre.y = (float)(distInCells.y * cellSize);
        cell.cellCentre.y += (1 - gridSize.y%2) * (float)cellSize / 2.0f * rawDistSigns[1]; //special consideration for even numbered cell y count.

        cell.cellCentre.z = this.transform.position.z;

        //Compute cellID
        //IMPORTANT! You're gambling that the calculations above WILL NEVER produce negative numbers. They should, but you need to double check that...
        cell.cellID[0] = (uint)(Mathf.FloorToInt((float)gridSize.x / 2.0f) + distInCells.x);
        cell.cellID[0] += (uint)((1 - gridSize.x%2) * (((1 + rawDistSigns[0]) / 2) - 1)); //special consideration for even numbered cell x count.

        cell.cellID[1] = (uint)(Mathf.FloorToInt((float)gridSize.y / 2.0f) + distInCells.y);
        cell.cellID[1] += (uint)((1 - gridSize.y%2) * (((1 + rawDistSigns[1]) / 2) - 1)); //special consideration for even numbered cell y count.

        GetAllCellStates(ref cell);

        //lastCellCentre = cell.cellCentre; //test
        //print ("dist in cells: " + distInCells + ", or: " + (rawDistInCells[0] * rawDistSigns[0]) + ", " + (rawDistInCells[1] * rawDistSigns[1]) ); //test
        //print ("cellID: " + cell.cellID[0] + ", " +cell.cellID[1]); //test
        return cell;
    }

    public Cell SampleForCell(uint cellID_x, uint cellID_y)
    {
        Cell cell = new Cell();

        cell.cellCentre = GetCellPosition(cellID_x, cellID_y);
        cell.cellID[0] = cellID_x;
        cell.cellID[1] = cellID_y;

        GetAllCellStates(ref cell);
        
        return cell;
    }

    public Vector3 GetCellPosition (uint cellID_x, uint cellID_y)
    {
        Vector3 _position = this.transform.position;
        Vector3 position = new Vector3( _position.x - (float)gridSize.x * (float)cellSize / 2.0f + cellID_x * (float)cellSize + (float)cellSize / 2.0f,
                                        _position.y - (float)gridSize.y * (float)cellSize / 2.0f + cellID_y * (float)cellSize + (float)cellSize / 2.0f,
                                        _position.z);
        return position;
    }

    void GetAllCellStates(ref Cell cell)
    {
        GetCellOccupationState(ref cell);
        // GetCellInfrastructureStates(ref cell);
        // GetCellNaturalResourcesStates(ref cell);
        // GetOtherCellStates(ref cell);
    }

    void GetCellOccupationState(ref Cell cell)
    {   
        switch(cellOccupationStatus.GetCellValue(cell.cellID[0], cell.cellID[1]))
        {
            case (0):
            cell.isOccupied = false;
                break;
            case (1):
            cell.isOccupied = true;
                break;
            default:
                break;
        }
    }

    public Vector2Int GetRandomCellID(uint paddingX = 0, uint paddingY = 0)
    {
        Vector2Int randomID = new Vector2Int();

        int sanitizedPaddingX = (Mathf.Min((int)paddingX, Mathf.FloorToInt((float)gridSize.x / 2.0f) - 1));
        int sanitizedPaddingY = (Mathf.Min((int)paddingY, Mathf.FloorToInt((float)gridSize.y / 2.0f) - 1));

        randomID.x = Random.Range((int)sanitizedPaddingX, gridSize.x - (int)sanitizedPaddingX);
        randomID.y = Random.Range((int)sanitizedPaddingY, gridSize.y - (int)sanitizedPaddingY);

        return randomID;
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