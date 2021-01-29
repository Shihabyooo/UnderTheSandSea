using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingType
{
    sleepTent, hq, fieldHospital, geologyLab, canteen, lounge, latrine, undefined
}

[RequireComponent(typeof(BoxCollider))]
public class Building : MonoBehaviour
{
    public GameObject model {get; private set;}
    public uint[] occupiedCell = new uint[2]; //the cell this building is constructed on, set by BuildingsManager.
    public System.DateTime constructionDate {get; private set;}
    //[SerializeField] uint constructionDuration = 1;
    
    BoxCollider buildingCollider;

    [SerializeField] protected BuildingStats stats;
    public  List<Worker> assignedWorkers {get; private set;}

    protected GameObject dashboard;

    virtual protected void Awake()
    {
        buildingCollider = this.gameObject.GetComponent<BoxCollider>();
        assignedWorkers = new List<Worker>();
    }
    
    
    //building construction methods
    #region construction
    public bool isUnderConstruction {get; private set;}
    uint constructionDaysElapsed = 0;
    public virtual void BeginConstruction(Cell cell)
    {
        isUnderConstruction = true;
        Grid.grid.SetCellOccupiedState(cell, true);
        occupiedCell = new uint[2]{cell.cellID[0], cell.cellID[1]};
        this.transform.localScale = Vector3.zero;
        SimulationManager.onNewDay += ProgressConstruction;
    }

    void ProgressConstruction(System.DateTime date)
    {
        constructionDaysElapsed++;
        float ratio = Mathf.Max(Mathf.Min((float)constructionDaysElapsed / (float)stats.constructionDuration , 1.0f), 0.25f);
        this.transform.localScale = new Vector3(ratio, ratio, ratio);

        if (ratio >= 0.999f)
            OnConstructionComplete(date);
    }

    protected virtual void OnConstructionComplete(System.DateTime date)
    {
        SimulationManager.onNewDay -= ProgressConstruction;
        isUnderConstruction = false;
        constructionDate = date;
        GameManager.buildMan.AddBuilding(this);
    }
    #endregion

    #region workers and effects
    public virtual bool AssignWorker(Worker newWorker) //IMPORTANT! Call this ONLY FROM WORKER CLASS!!!!!
    {
        //consider checking if worker is already assigned. Modify return to an int and return a code based on results,

        if (assignedWorkers.Count >= stats.capacity)
            return false;

        assignedWorkers.Add(newWorker);
        return true;
    }
    
    public virtual bool RemoveWorker(Worker worker)
    {
        return assignedWorkers.Remove(worker);
    }

    #endregion

    #region other
    public BuildingStats GetStats()
    {
        BuildingStats tempStats = new BuildingStats(stats);
        return tempStats;
    }
    
    public virtual void ShowBuildingDashboard()
    {
        print ("WARNING! Unimplemented ShowBuildingDashboard.");
    }
    
    public virtual uint AvailableSlots()
    {
        return (uint)Mathf.Max(stats.capacity - assignedWorkers.Count , 0);
    }

    #endregion

}

[System.Serializable]
public class BuildingStats
{
    public int id;
    public uint cost = 0;
    public BuildingType type = BuildingType.undefined;
    public uint constructionDuration = 1; //In days
    public uint capacity;

   
    public BuildingStats()
    {

    }

    public BuildingStats(BuildingStats source) //Deep copy
    {
        id = source.id;
        cost = source.cost;
        type = source.type;
        constructionDuration = source.constructionDuration;
        capacity = source.capacity;
    }
}