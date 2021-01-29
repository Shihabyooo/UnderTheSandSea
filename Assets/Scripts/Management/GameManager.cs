using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(ControlManager))]
//[RequireComponent(typeof(ResourcesManager))]
[RequireComponent(typeof(SimulationManager))]
[RequireComponent(typeof(BuildingsManager))]
//[RequireComponent(typeof(ClimateManager))]
[RequireComponent(typeof(PopulationManager))]
[RequireComponent(typeof(UIManager))]
//[RequireComponent(typeof(EconomyManager))]
public class GameManager : MonoBehaviour
{
    static public GameManager gameMan = null;
    static ControlManager controlMan = null;
    static public BuildingsManager buildMan = null;
    static public SimulationManager simMan = null;
    static public PopulationManager popMan = null;
    static public UIManager uiMan = null;

    static public GameObject canvas = null;

    void Awake()
    {
        if (gameMan == null)
        {
            gameMan = this;
            controlMan = this.gameObject.GetComponent<ControlManager>();
            buildMan = this.gameObject.GetComponent<BuildingsManager>();
            simMan = this.gameObject.GetComponent<SimulationManager>();
            popMan = this.gameObject.GetComponent<PopulationManager>();
            uiMan = this.gameObject.GetComponent<UIManager>();

            canvas = GameObject.Find("Canvas");
        }
        else
        {
            Destroy (this.gameObject);
        }

        StartNewSimulation(); //to be moved to game starting logic once implemented.
    }

    void StartNewSimulation()
    {
        simMan.Initialize();
        popMan.Initialize();
        buildMan.Initialize();
        uiMan.Initialize();
    }

    public void SwitchToBuildingPlacement(int buildingID)
    {
        if (controlMan.CurrentCursorMode() != ControlMode.freeMode) 
            return;                                                    

        controlMan.SwitchToObjectPlacement(buildMan.StartNewBuildingProposal(buildingID));
    }

    public void StartWorkDay()
    {
        print ("Starting new work day at gameMan");
        //block all input here (except for skip animation)
        simMan.StartWorkDay();
    }

    public void HireWorker(WorkerType type, Building sourceBuilding) //Source building => building that the hiring was started from.
    {
        if (type == WorkerType.excavator) //Excavators aren't assigned to a building, hired only from HQ.
        {
            popMan.HireNewWorker(type,
                                null,
                                buildMan.GetVacantBed(),
                                simMan.currentDate);
        }
        else
        {
            popMan.HireNewWorker(type,
                                sourceBuilding,
                                buildMan.GetVacantBed(),
                                simMan.currentDate);
        }
    }

    public void StartSelectingExcavationArea()
    {
        controlMan.SwitchToExcavationTargetting();
    }
    public void SetExcavationArea(Cell cell, float radius)
    {
        simMan.workPlan.SetExcavationArea(new Vector2Int((int)cell.cellID[0],(int)cell.cellID[1] ), radius);
    }




    //Win/Lose cases

    public void HandleBankrupcy()
    {
        
    }

}
