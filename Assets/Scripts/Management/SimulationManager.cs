﻿using System.Collections;
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
    public SimulationParameters simParam = new SimulationParameters(1);

    public uint progress {get; private set;} //discovery progress, percentage.

    public void Initialize()
    {
        currentDate = new System.DateTime(startYear, startMonth, 1);
        progress = 0;
    }

    public void StartWorkDay()
    {
        //TODO redo this part to run as two coroutines:
            //1- handles animation (with skipping functionality)
            //2- handles remaining tasks (performance updating, random events decision, etc)
        //At end of execution, each coroutine calls a function with a counter and checks for counter >= 2 (to mark both courtines finishing), then
        //continue to wrap up day and show results.

        //Start animation

        //process special tasks.
        if (WorkPlan.onWorkStart != null)
            WorkPlan.onWorkStart.Invoke();

        //compute performance and update progress
        progress += Performance();

        //process random events.

        //Update workers stats.

        //finialize things, show results.

        //progress to next day
        currentDate += new System.TimeSpan(1, 0, 0, 0);
        
        if (onNewDay != null)
            onNewDay.Invoke(currentDate);
    }

    uint Performance()
    {
        uint performance = 0;
        
        return (uint)Mathf.RoundToInt((float)performance * simParam.performanceModifier);
    }

    void UpdateWorkersStats() //end of day
    {
        // foreach(Worker worker in GameManager.popMan.)
    }

    //test
    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 35;

        GUI.Label(new Rect(10, 40, 100, 20), "Date: " + currentDate.ToString(), style);
    }
}

public class WorkPlan
{
    public System.DateTime date {get; private set;} //for history logging
    public uint hours {get; private set;}
    public uint foodBudget {get; private set;}
    public uint medicineBudget {get; private set;}
    
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
    public void SetPlan(System.DateTime workDayDate, uint dayWorkHours, uint dayFoodBudget, uint dayMedicineBudget)
    {
        date = workDayDate;
        hours = dayWorkHours;
        foodBudget = dayFoodBudget;
        medicineBudget = dayMedicineBudget;
    }
}

public class SimulationParameters
{
    public uint baseSanityLossRate {get; private set;} 
    public uint baseHealthLossRate {get; private set;}
    public uint baseFundsGainRate {get; private set;}
    public float disasterSanityLossModifier {get; private set;}
    public float disasterHealthLossModifier {get; private set;}
    public float performanceModifier {get; private set;}

    public SimulationParameters(uint level) //0 = easy, 1 = medium, 2 = brutal!
    {
        if (level < 1)
        {
            baseSanityLossRate = 5;
            baseHealthLossRate = 5;
            baseFundsGainRate = 100;
            disasterSanityLossModifier = 0.5f;
            disasterHealthLossModifier = 0.5f;
            performanceModifier = 1.5f;
        }
        else if (level == 2)
        {
            baseSanityLossRate = 10;
            baseHealthLossRate = 10;
            baseFundsGainRate = 75;
            disasterSanityLossModifier = 1.0f;
            disasterHealthLossModifier = 1.0f;
            performanceModifier = 1.0f;
        }
        else
        {
            baseSanityLossRate = 15;
            baseHealthLossRate = 15;
            baseFundsGainRate = 50;
            disasterSanityLossModifier = 1.5f;
            disasterHealthLossModifier = 1.5f;
            performanceModifier = 0.75f;
        }
    }
}