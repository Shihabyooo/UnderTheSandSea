using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public System.DateTime currentDate {get; private set;}
    [SerializeField] [Range(1900, 2030)] int startYear = 1960;
    [SerializeField] [Range(1, 12)] int startMonth = 1;

    public delegate void OnNewDay(System.DateTime date);
    public static OnNewDay onNewDay;

    public WorkPlan workPlan = new WorkPlan();
    public Finances finances = new Finances();
    public SimulationParameters simParam = new SimulationParameters(1);
    public StatusEffects statEffects = new StatusEffects();

    public float progress {get; private set;} //discovery progress, percentage.
    public Vector2Int goalCell = new Vector2Int(); //the cell closes to target. Excavating near it yields better performance. Randomized each game.

    public void Initialize()
    {
        currentDate = new System.DateTime(startYear, startMonth, 1);
        progress = 0.0f;

        goalCell = Grid.grid.GetRandomCellID(1,1);
    }

    //Day Time
    Coroutine workingAnimation = null;
    Coroutine workingProcess = null;
    int helperCounter = 0;
    
    public void StartWorkDay()
    {
        workingAnimation = StartCoroutine(WorkAnimation());
        workingProcess = StartCoroutine(WorkProcess());
    }

    IEnumerator WorkAnimation()
    {
        OnWorkDayComponentFinish();
        yield return null;
    }

    IEnumerator WorkProcess()
    {
        //process special tasks.
        if (WorkPlan.onWorkStart != null)
            WorkPlan.onWorkStart.Invoke();

        //compute performance and update progress
        progress += Performance();

        //process random day events.

        //Update Finances
        UpdateFinances();

        OnWorkDayComponentFinish();
        yield return null;
    }
    
    void OnWorkDayComponentFinish() //called only by WorkAnimation () and WorkProcess() coroutines.
    {
        helperCounter++;
        if (helperCounter < 2)
            return;

        //reaching here means that both coroutines have finished
        helperCounter = 0;
        workingAnimation = null;
        workingProcess = null;

        //finialize things, show results.
        GameManager.uiMan.UpdateProgress((uint)Mathf.FloorToInt(progress));
        GameManager.gameMan.FinishWorkDay();
    }

    float Performance()
    {
        float effectivePerformance = 0;
        
        //rawPerformance is the pure excavation progress, affected solely by excavators performance. Doesn't necessarilly equally affect progress because we could
        //be digging far from where the actual target is.
        
        float rawPerfomance = GameManager.popMan.ExcavationProduction();

        //TODO modify rawPerformance based on proximity to actual location of target, and any other factors.
        float distanceToTarget = Vector3.Distance(Grid.grid.GetCellPosition((uint)goalCell.x, (uint)goalCell.y),
                                                    Grid.grid.GetCellPosition((uint)workPlan.excavationAreaCentre.x, (uint)workPlan.excavationAreaCentre.y));

        if (distanceToTarget < workPlan.excavationAreaRadius)
        {
            effectivePerformance = rawPerfomance;
            //print ("Inside Radius"); //test
        }
        else
        {
            float distanceOutSideRadius = distanceToTarget - workPlan.excavationAreaRadius;
            float performanceDrop = simParam.performanceDropPerCellWidth * distanceOutSideRadius / (float)Grid.cellSize;
            effectivePerformance = Mathf.Max(rawPerfomance - performanceDrop, 0.0f);
            //print ("Outside Radius"); //test
        }
        //print ("Effective Perf: " + effectivePerformance); //test
        return (float)effectivePerformance * simParam.performanceModifier;
    }

    void UpdateFinances()
    {
        //revenue
        ulong revenue = (ulong)Mathf.Round((float)simParam.baseFundsGainRate * statEffects.revenueEffectModifier) + statEffects.revenueEffectBonus;
        finances.AddFunds(revenue);


        //expenses
        ulong buildingExpenses = GameManager.buildMan.ComputeBuildingsExpenses();
        ulong wages = GameManager.popMan.ComputeTotalWages();

        finances.SubtractFunds(buildingExpenses + wages);
        
    }

    //Night time
    Coroutine nightAnimation = null;
    Coroutine nightProcess = null;
    int helperCounter2 = 0;

    public void StartNight()
    {
        nightAnimation = StartCoroutine(NightAnimation());
        nightProcess = StartCoroutine(NightProcess());
    }

    IEnumerator NightAnimation()
    {
        OnNightComponentFinish();
        yield return null;
    }

    IEnumerator NightProcess()
    {
        //loop over workers, modify their sanity and health based on available facilities.
        GameManager.popMan.UpdateWorkersHealth();
        GameManager.popMan.UpdateWorkersFood();
        GameManager.popMan.UpdateWorkersSanity();
        OnNightComponentFinish();
        yield return null;
    }
    
    void OnNightComponentFinish()
    {
        helperCounter2++;
        if (helperCounter2 < 2)
            return;

        helperCounter2 = 0;
        nightAnimation = null;
        nightProcess = null;

        //progress to next day
        currentDate += new System.TimeSpan(1, 0, 0, 0);
        
        if (onNewDay != null)
            onNewDay.Invoke(currentDate);
    }

    //test
    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 35;

        GUI.Label(new Rect(10, 40, 100, 20), "Date: " + currentDate.ToString(), style);
    }

    void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(Grid.grid.GetCellPosition((uint)goalCell.x, (uint)goalCell.y), 0.35f);
        }
    }
}

public class WorkPlan
{
    public System.DateTime date {get; private set;} //for history logging
    public uint hours {get; private set;}
    public uint foodBudget {get; private set;}
    public uint medicineBudget {get; private set;}
    public Vector2Int excavationAreaCentre {get; private set;} //in cell ID
    public float excavationAreaRadius {get; private set;}
    
    public delegate void OnWorkStart(); //event delegate for special tasks
    public static OnWorkStart onWorkStart;

    // public WorkPlan(System.DateTime workDayDate, uint dayWorkHours, uint dayFoodBudget, uint dayMedicineBudget)
    // {
    //     date = workDayDate;
    //     hours = dayWorkHours;
    //     foodBudget = dayFoodBudget;
    //     medicineBudget = dayMedicineBudget;
        
    // }

    public WorkPlan()
    {
        date = new System.DateTime();
        hours = 8;
        foodBudget = 100;
        medicineBudget = 100;
    }

    //setters
    public void SetPlan(System.DateTime workDayDate, uint dayWorkHours, uint dayFoodBudget, uint dayMedicineBudget,
                        Vector2Int dayExcavationAreaCentre, float dayExcavationAreaRadius)
    {
        date = workDayDate;
        hours = dayWorkHours;
        foodBudget = dayFoodBudget;
        medicineBudget = dayMedicineBudget;
        excavationAreaCentre = dayExcavationAreaCentre;
        excavationAreaRadius = dayExcavationAreaRadius;
    }
    
    public void SetExcavationArea(Vector2Int centre, float radius)
    {
        excavationAreaCentre = centre;
        excavationAreaRadius = radius;
    }
}

public class Finances
{
    public long funds {get; private set;}
    public ulong dayExpenses; //solely for caching.
    public ulong dayRevenue; //solely for caching.

    public Finances()
    {
        funds = 1000;
        ResetDayStatistics();
    }


    public void AddFunds(ulong revenue)
    {
        dayRevenue += revenue;
        funds += (int)revenue;
    }

    public void SubtractFunds(ulong expense)
    {
        funds -= (long)expense;
        dayExpenses += expense;

        if (funds < 0)
            GameManager.gameMan.HandleBankrupcy();
    }
    
    public bool CanAfford(uint expense)
    {
        if (expense > funds)
            return false;
        return true;
    }

    public void ResetDayStatistics()
    {
        dayExpenses = 0;
        dayRevenue = 0;
    }
}

public class SimulationParameters
{
    public uint baseSanityLossRate {get; private set;} 
    public uint baseHealthLossRate {get; private set;}
    public uint baseFoodLossRate {get; private set;}
    public uint baseFundsGainRate {get; private set;}
    public float disasterSanityLossModifier {get; private set;}
    public float disasterHealthLossModifier {get; private set;}
    public float performanceModifier {get; private set;}
    public float performanceDropPerCellWidth {get; private set;} //if the excavation was outside the area selected, performance will drop with this value for every cell width distance
    //public float minExcavationDistanceModifier {get; private set;}  //perfomance drop from distance will not go lower than this value
    public uint malnourishmentThreshold {get; private set;} //if food dropped bellow this, worker basehealth will be halved before other calcs, also affects sanity.
    

    public float baseExcavatorPerformance {get; private set;}
    public uint excavatorsPerArchaelogist {get; private set;}
    //public long startingFunds {get; private set;}

    public float baseHQEffectiveness {get; private set;}
    public float baseSleepingTentEffectiveness {get; private set;}
    public float baseLoungeEffectiveness {get; private set;}
    public float baseGeologyLabEffectiveness {get; private set;}
    public float baseCanteentEffectiveness {get; private set;}
    public float baseLatrineEffectiveness {get; private set;}
    public float baseFieldHospitalEffectiveness {get; private set;}

    public uint fieldHospitalVisitorsThreshold {get; private set;} //the number of visitors after which effectiveness drops.
    public uint baseFieldHospitalHealthRestore {get; private set;}

    public uint canteenVisitorsThreshold {get; private set;} //the number of visitors after which effectiveness drops.
    public uint baseCanteenFoodRestore {get; private set;}
    public uint baseCanteenSanityRestore {get; private set;}

    public uint latrineVisitorsThreshold {get; private set;} //the number of visitors after which effectiveness drops.
    public uint baseLatrineSanityRestore {get; private set;}
    

    public SimulationParameters(uint level) //0 = easy, 1 = medium, 2 = brutal!
    {
        if (level < 1)
        {
            baseSanityLossRate = 5;
            baseHealthLossRate = 5;
            baseFoodLossRate = 5;
            baseFundsGainRate = 100;
            disasterSanityLossModifier = 0.5f;
            disasterHealthLossModifier = 0.5f;
            performanceModifier = 1.5f;
            performanceDropPerCellWidth = 0.25f;
            minExcavationDistanceModifier = 0.35f;

            malnourishmentThreshold = 10;

            baseExcavatorPerformance = 0.125f;
            excavatorsPerArchaelogist = 10;
            
            baseHQEffectiveness = 1.0f;
            baseSleepingTentEffectiveness = 1.0f;
            baseLoungeEffectiveness = 1.0f;
            baseGeologyLabEffectiveness = 1.0f;
            baseCanteentEffectiveness = 1.0f;
            baseLatrineEffectiveness = 1.0f;
            baseFieldHospitalEffectiveness = 1.0f;

            fieldHospitalVisitorsThreshold = 20;
            baseFieldHospitalHealthRestore = 7;

            canteenVisitorsThreshold = 20;
            baseCanteenFoodRestore = 7;

            latrineVisitorsThreshold = 20;
            baseLatrineSanityRestore = 2;
        }
        else if (level == 1)
        {
            baseSanityLossRate = 10;
            baseHealthLossRate = 10;
            baseFoodLossRate = 10;
            baseFundsGainRate = 75;
            disasterSanityLossModifier = 1.0f;
            disasterHealthLossModifier = 1.0f;
            performanceModifier = 1.0f;
            performanceDropPerCellWidth = 0.45f;
            minExcavationDistanceModifier = 0.15f;
            malnourishmentThreshold = 15;

            baseExcavatorPerformance = 0.1f;
            excavatorsPerArchaelogist = 10;

            baseHQEffectiveness = 1.0f;
            baseSleepingTentEffectiveness = 1.0f;
            baseLoungeEffectiveness = 1.0f;
            baseGeologyLabEffectiveness = 1.0f;
            baseCanteentEffectiveness = 1.0f;
            baseLatrineEffectiveness = 1.0f;
            baseFieldHospitalEffectiveness = 1.0f;

            fieldHospitalVisitorsThreshold = 20;
            baseFieldHospitalHealthRestore = 10;

            canteenVisitorsThreshold = 20;
            baseCanteenFoodRestore = 15;

            latrineVisitorsThreshold = 20;
            baseLatrineSanityRestore = 2;
        }
        else
        {
            baseSanityLossRate = 15;
            baseHealthLossRate = 15;
            baseFoodLossRate = 15;
            baseFundsGainRate = 50;
            disasterSanityLossModifier = 1.5f;
            disasterHealthLossModifier = 1.5f;
            performanceModifier = 0.75f;
            performanceDropPerCellWidth = 0.5f;
            minExcavationDistanceModifier = 0.0f;
            malnourishmentThreshold = 20;

            baseExcavatorPerformance = 0.075f;
            excavatorsPerArchaelogist = 7;

            baseHQEffectiveness = 0.9f;
            baseSleepingTentEffectiveness = 0.9f;
            baseLoungeEffectiveness = 0.9f;
            baseGeologyLabEffectiveness = 0.9f;
            baseCanteentEffectiveness = 0.9f;
            baseLatrineEffectiveness = 0.9f;
            baseFieldHospitalEffectiveness = 0.9f;

            fieldHospitalVisitorsThreshold = 15;
            baseFieldHospitalHealthRestore = 10;
            
            canteenVisitorsThreshold = 15;
            baseCanteenFoodRestore = 15;

            latrineVisitorsThreshold = 15;
            baseLatrineSanityRestore = 5;
        }
    }
}

public class StatusEffects //these are changed by events and external factors that apply per simulation.
{
    // variables ending with "Modifier" will be multiplied with respective base value.
    //variables ending with "Bonus" will be added to respective base value.
    //Modifiers are applied before bonuses.

    //Note: These stats are distinct from worker trait effects.

    public float hqEffectModifier = 1.0f;
    public float hqEffectBonus = 0.0f;

    public float sleepingTentEffectModifier = 1.0f;
    public float sleepingTentEffectBonus = 0.0f;

    public float loungeEffectModifier = 1.0f;
    public float loungEffectBonus = 0.0f;

    public float latrineEffectModifier = 1.0f;
    public float latrineEffectBonus = 0.0f;
    
    public float canteentEffectModifier = 1.0f;
    public float canteentEffectBonus = 0.0f;
    
    public float fieldHospitalEffectModifier = 1.0f;
    public float fieldHospitalEffectBonus = 0.0f;
    
    public float geologyLabEffectModifier = 1.0f;
    public float geologyLabEffectBonus = 0.0f;

    public float revenueEffectModifier = 1.0f;
    public ulong revenueEffectBonus = 0;
}