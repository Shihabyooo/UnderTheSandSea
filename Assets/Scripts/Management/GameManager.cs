using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(ControlManager))]
//[RequireComponent(typeof(ResourcesManager))]
[RequireComponent(typeof(SimulationManager))]
[RequireComponent(typeof(BuildingsManager))]
//[RequireComponent(typeof(ClimateManager))]
[RequireComponent(typeof(PopulationManager))]
//[RequireComponent(typeof(UIManager))]
//[RequireComponent(typeof(EconomyManager))]
public class GameManager : MonoBehaviour
{
    static public GameManager gameMan = null;
    static ControlManager controlMan = null;
    static public BuildingsManager buildMan = null;
    public SimulationManager simMan;
    static public PopulationManager popMan;

    void Awake()
    {
        if (gameMan == null)
        {
            gameMan = this;
            controlMan = this.gameObject.GetComponent<ControlManager>();
            buildMan = this.gameObject.GetComponent<BuildingsManager>();
            simMan = this.gameObject.GetComponent<SimulationManager>();
            popMan = this.gameObject.GetComponentInChildren<PopulationManager>();
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

}
