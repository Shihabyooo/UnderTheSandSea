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

    public void StartNewGame()
    {
        StartNewSimulation();
    }

    void StartNewSimulation() //i.e. new game.
    {
        simMan.Initialize(1);
        popMan.Initialize();
        buildMan.Initialize();
        uiMan.Initialize();
        Grid.grid.Initialize();

        currentGameState = GameState.gameplayStage1;
        SimulationManager.onNewDay += FinishNight;
    }

    public void SwitchToBuildingPlacement(int buildingID)
    {
        if (controlMan.CurrentCursorMode() != ControlMode.freeMode || currentGameState != GameState.gameplayStage1)
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

        uiMan.ToggleControls(false);


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
        uiMan.ToggleControls(true);
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
    public void HandleWinning()
    {
        //This method is callled at WorkProcess(), before dayEvent is processed. So, we simply override whatever event that may have been set with a Winning event,
        //and let it handle game loss display.
        print ("Player has won");
        currentGameState = GameState.endGame;

        GameObject eventHolder = Instantiate(new GameObject("GameOver_Win"), Vector3.zero, new Quaternion(), this.transform);
        ScenarioEvent newEvent = (ScenarioEvent)eventHolder.AddComponent(typeof(Win));
        simMan.AddScenarioEvent(newEvent, true);
    }

    public void HandleBankrupcy()
    {
        //Similar rationale to HandleWinning().

        print ("Player is bankrupt");
        currentGameState = GameState.endGame;

        GameObject eventHolder = Instantiate(new GameObject("GameOver_Bankrupcy"), Vector3.zero, new Quaternion(), this.transform);
        ScenarioEvent newEvent = (ScenarioEvent)eventHolder.AddComponent(typeof(Bankrupcy));
        simMan.AddScenarioEvent(newEvent, true);
    }

    public void ReturnToMainMenu()
    {
        buildMan.CleanUp();
        currentGameState = GameState.mainMenu;
        uiMan.ShowMainMenu();
    }
}
