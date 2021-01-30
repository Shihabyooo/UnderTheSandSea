using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    intro, mainMenu, loading,
    gameplayStage1,  //Planning stage: building construction, worker hiring, workplan setting.
    gameplayStage2, //Work animation (after player clicks "Start Day"), day events may occur.
    gameplayStage3,  //Showing day results
    gameplayStage4,  //post-results, before start of new day. Workers rest, night events may occur.
    endGame,        //Win-Lose animation/screen. Afterwards, returns to mainMenu.
    defaultState
}


[RequireComponent(typeof(ControlManager))]
[RequireComponent(typeof(SimulationManager))]
[RequireComponent(typeof(BuildingsManager))]
[RequireComponent(typeof(PopulationManager))]
[RequireComponent(typeof(UIManager))]
public class GameManager : MonoBehaviour
{
    static public GameManager gameMan = null;
    static ControlManager controlMan = null;
    static public BuildingsManager buildMan = null;
    static public SimulationManager simMan = null;
    static public PopulationManager popMan = null;
    static public UIManager uiMan = null;

    static public GameObject canvas = null;
    static public GameState currentGameState {get; private set;}

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

        
    }

    void Start()
    {
        StartNewSimulation(); //to be moved to game starting logic once implemented.
    }

    void StartNewSimulation() //i.e. new game.
    {
        simMan.Initialize();
        popMan.Initialize();
        buildMan.Initialize();
        uiMan.Initialize();

        currentGameState = GameState.gameplayStage1;
        SimulationManager.onNewDay += FinishNight;
    }

    public void SwitchToBuildingPlacement(int buildingID)
    {
        if (controlMan.CurrentCursorMode() != ControlMode.freeMode) 
            return;                                                    

        controlMan.SwitchToObjectPlacement(buildMan.StartNewBuildingProposal(buildingID));
    }

    public void StartWorkDay() //Called when clicking on "Start Day" button.
    {
        if (currentGameState != GameState.gameplayStage1)
        {
            print ("WARNING! Attempted to start a new day from a gamestage that isn't gameStage1");
            return;
        }

        print ("Starting new work day at gameMan");
        //close current open windows
        if (RosterSheet.rosterSheet.enabled)
            RosterSheet.rosterSheet.Close();
        
        uiMan.SwitchDashboard(null);

        //TODO block all input here (except for skip animation)
        currentGameState = GameState.gameplayStage2;
        simMan.StartWorkDay();
    }

    public void FinishWorkDay() //called by simMan after both day work coroutines finish
    {
        currentGameState = GameState.gameplayStage3;
        uiMan.ShowDayReport();
    }

    public void StartNight() //called when clicking on "Sign" button on day report.
    {
        uiMan.HideReport();
        simMan.finances.ResetDayStatistics();
        currentGameState = GameState.gameplayStage4;
        simMan.StartNight();
        //start night animation/event decision.
    }

    public void FinishNight(System.DateTime date) //delegated to onNewDay event in simMan
    {
        currentGameState = GameState.gameplayStage1;
        //re-enable player control for construction, planning and management.
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
        currentGameState = GameState.endGame;
    }
}
