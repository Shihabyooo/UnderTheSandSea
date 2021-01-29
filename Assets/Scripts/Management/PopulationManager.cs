﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    Population population;
    void Awake()
    {
        population = new Population();
    }

    public void Initialize()
    {
        population.Clear();
    }

//Pop management and general stats
    public uint Count(WorkerType type)
    {
        return population.Count(type);
    }

    public uint CountAll()
    {
        return population.TotalCount();
    }

    public void HireNewWorker(WorkerType type, Building assignedWorkBuilding, SleepingTent assignedSleepingTent, System.DateTime currentDate)
    {
        //Generate worker
        Worker newWorker = GenerateWorker(type, currentDate);

        //Assign to workplace.

        if (assignedWorkBuilding != null) //null is the case for excavators.
            //assignedWorkBuilding.AssignWorker(newWorker);
            newWorker.AssignWorkBuilding(assignedWorkBuilding);

        //Assign to sleeping tent.
        //assignedSleepingTent.AssignWorker(newWorker);
        newWorker.AssignSleepTent(assignedSleepingTent);

        //add to population
         population.AddWorker(newWorker);
    }

    Worker GenerateWorker (WorkerType type, System.DateTime currentDate)
    {
        Worker newWorker = new Worker(NameGenerator.GenerateRandomName(), currentDate, type);
        
        return newWorker;
    }

    public bool CanHireWorker(WorkerType type, Building building, uint count = 1) //for excavators, building should be null.
    {
        //first, test whether building has capacity
        if (building != null
            && type != WorkerType.excavator //excavators don't need to be assigned to a work building, only sleepingtent.
            && building.AvailableSlots() < count) 
            return false;
        
        //Then, test whether we have enough beds
        if (GameManager.buildMan.TotalAvailableBeds() < count)
            return false;

        //Now we handle special conditions that may arise (e.g. can't hire more than 10 excavators per archaelogist)
        switch(type)
        {
            case WorkerType.archaeologist:
                break;
            case WorkerType.geologist:
                break;
            case WorkerType.excavator:
            {
                int supportedExcavCount = (int)GameManager.simMan.simParam.excavatorsPerArchaelogist * (int)Count(WorkerType.archaeologist); //Should be safe to int it. Doubt 
                                                                                                                                            //RHS would ever exceed 2^31-1
                if (count > (supportedExcavCount - Count(WorkerType.excavator)))
                    return false;
                break;
            }
            case WorkerType.cook:
                break;
            case WorkerType.physician:
                break;
            case WorkerType.generic:
                break;
            default:
                break;
        }

        return true;
    }

    public bool RemoveWorker (Worker worker)
    {
        worker.PrepareForRemoval();
        return population.RemoveWorker(worker);
    }


//Metrics computation
    public float ExcavationProduction()
    {
        float totalProduction = 0.0f;

        foreach(Worker excavator in population.excavators)
        {
            totalProduction += excavator.Production();
        }
        
        return totalProduction;
    }


    //Testing methods
    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 35;

        GUI.Label(new Rect(10, 80, 100, 20), "Population: " + CountAll().ToString(), style);
        style.fontSize = 25;
        GUI.Label(new Rect(10, 120, 100, 20), "Archelogists: " + Count(WorkerType.archaeologist).ToString() + " | Geologists: " + Count(WorkerType.geologist).ToString(), style);
        GUI.Label(new Rect(10, 150, 100, 20), "Excavators: " + Count(WorkerType.excavator).ToString() + " | Cooks: " + Count(WorkerType.cook).ToString() + " | physicians: " + Count(WorkerType.physician).ToString(), style);
    }
}

class Population
{
    public List<Worker> excavators {get; private set;}
    public List<Worker> geologists {get; private set;}
    public List<Worker> archaelogists {get; private set;}
    public List<Worker> physicians {get; private set;}
    public List<Worker> cooks {get; private set;}


    public Population()
    {
        excavators = new List<Worker>();
        geologists = new List<Worker>();
        archaelogists = new List<Worker>();
        physicians = new List<Worker>();
        cooks = new List<Worker>();
    }

    public void AddWorker(Worker worker)
    {
        switch(worker.type)
        {
            case WorkerType.archaeologist:
                archaelogists.Add(worker);
                break;
            case WorkerType.geologist:
                geologists.Add(worker);
                break;
            case WorkerType.excavator:
                excavators.Add(worker);
                break;
            case WorkerType.physician:
                physicians.Add(worker);
                break;
            case WorkerType.cook:
                cooks.Add(worker);
                break;
            default:
                //print("WARNING! Attempting to add a worker with an unhandled type."); //needs to derive from monobehaviour to work
                break;
        }
    }

    public bool RemoveWorker(Worker worker)
    {
        switch(worker.type)
        {
            case WorkerType.archaeologist:
                return archaelogists.Remove(worker);
            case WorkerType.geologist:
                return geologists.Remove(worker);
            case WorkerType.excavator:
                return excavators.Remove(worker);
            case WorkerType.physician:
                return physicians.Remove(worker);
            case WorkerType.cook:
                return cooks.Remove(worker);
            default:
                return false;
        }
    }

    public uint Count(WorkerType type)
    {
        switch(type)
        {
            case WorkerType.archaeologist:
                return (uint) archaelogists.Count;
            case WorkerType.geologist:
                return (uint) geologists.Count;
            case WorkerType.excavator:
                return (uint) excavators.Count;
            case WorkerType.cook:
                return (uint) cooks.Count;
            case WorkerType.physician:
                return (uint) physicians.Count;
            case WorkerType.generic:
                return 0;
            default:
                return 0;
        }
    }

    public uint TotalCount()
    {
        return (uint)(archaelogists.Count + geologists.Count + excavators.Count + physicians.Count + cooks.Count);
    }

    public void Clear() //clears all lists
    {
        archaelogists.Clear();
        geologists.Clear();
        excavators.Clear();
        physicians.Clear();
        cooks.Clear();
    }



}