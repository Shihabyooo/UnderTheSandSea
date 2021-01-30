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
    BoxCollider buildingCollider;
    [SerializeField] protected BuildingStats stats;
    public  List<Worker> assignedWorkers {get; private set;}
    protected GameObject dashboard;
    public float effectiveness {get; protected set;}
    public uint budget {get; protected set;}
    public string description {get; protected set;}

    virtual protected void Awake()
    {
        buildingCollider = this.gameObject.GetComponent<BoxCollider>();
        assignedWorkers = new List<Worker>();
        effectiveness = 0.0f;
        budget = (uint)Mathf.RoundToInt(((float)stats.maxBudget + (float)stats.minBudget) / 2.0f);
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
    
    public virtual float ComputeEffectiveness()
    {
        return effectiveness;
    }

    public virtual void SetBudget(uint newBudget)
    {
        budget = (uint)Mathf.Clamp(newBudget, stats.minBudget, stats.maxBudget);
    }

    protected virtual float BudgetEffect()
    {
        //Using a slightly modified Logistic function Y = L/(1+exp(-k(x-x0))) + a
            // L = Max Value (1.0 here), k = growth rate, x0 = midpoint x, a = modification
        float budgetRatio = ((float)budget - (float)stats.minBudget)/((float)stats.maxBudget - (float)stats.minBudget);
        float effect = 1.0f / (1.0f + Mathf.Exp(-5 * (budgetRatio - 0.6f))) + 0.12f;

        return effect;
    }
    #endregion

}

[System.Serializable]
public class BuildingStats
{
    public int id;
    public uint cost = 500;
    public BuildingType type = BuildingType.undefined;
    public uint constructionDuration = 1; //In days
    public uint capacity;
    public uint minBudget = 5;
    public uint maxBudget = 100;

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
        minBudget = source.minBudget;
        maxBudget = source.maxBudget;
    }
}