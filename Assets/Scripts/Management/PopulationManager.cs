using System.Collections;
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

    public ulong ExcavationProduction()
    {
        ulong totalProduction = 0;

        foreach(Worker excavator in population.excavators)
        {
            totalProduction += excavator.Production();
        }
        
        return totalProduction;
    }

    public bool HireNewWorker(WorkerType type, Building assignedWorkBuilding, SleepingTent assignedSleepingTent)
    {
        switch(type)
        {
            case WorkerType.archaeologist:
                break;
            case WorkerType.geologist:
                break;
            case WorkerType.excavator:
                break;
            case WorkerType.cook:
                break;
            case WorkerType.physician:
                break;
            case WorkerType.generic:
                break;
            default:
                break;
        }

        return false;
    }

    // public bool CanHire(WorkerType type)
    // {
    //     switch(type)
    //     {
    //         case WorkerType.archaeologist:

    //             break;
    //         case WorkerType.geologist:
    //             break;
    //         case WorkerType.excavator:
    //             break;
    //         case WorkerType.cook:
    //             break;
    //         case WorkerType.physician:
    //             break;
    //         case WorkerType.generic:
    //             break;
    //         default:
    //             break;
    //     }
    // }

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

    public uint Count()
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