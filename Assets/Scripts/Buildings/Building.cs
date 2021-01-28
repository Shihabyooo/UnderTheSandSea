using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Building : MonoBehaviour
{
    public GameObject model {get; private set;}
    public uint[] occupiedCell = new uint[2]; //the cell this building is constructed on, set by BuildingsManager.
    public System.DateTime constructionDate {get; private set;}
    [SerializeField] uint constructionDuration = 1;
    
    BoxCollider buildingCollider;

    virtual protected void Awake()
    {
        buildingCollider = this.gameObject.GetComponent<BoxCollider>();
    }
    
    
    //building construction methods
    #region construction
    bool isUnderConstruction = false;
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
        float ratio = Mathf.Min((float)constructionDaysElapsed / (float)constructionDuration , 1.0f);
        this.transform.localScale = new Vector3(ratio, ratio, ratio);

        if (ratio >= 0.999f)
            OnConstructionComplete();
    }

    protected virtual void OnConstructionComplete()
    {
        SimulationManager.onNewDay -= ProgressConstruction;
        isUnderConstruction = false;
        constructionDate = GameManager.simMan.currentDate;
    }
    #endregion


}
