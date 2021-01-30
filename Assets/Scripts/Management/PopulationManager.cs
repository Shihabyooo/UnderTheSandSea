using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    public Population population {get; private set;}
    
    public void Initialize()
    {
        population = new Population();
        workersToKill = new List<Worker>();
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

//Worker stats updating
    List<Worker> workersToKill = new List<Worker>();
    public void UpdateWorkersHealth()
    {
        //first compute field hospital effect
        float healthGain = 0.0f;
        
        if (GameManager.buildMan.fieldHospitals.Count > 0)
        {
            //overall effectiveness => to account for overload (more visitors than facility can handle)

            float visitorsPerHospital = (float)GameManager.popMan.CountAll() / (float)GameManager.buildMan.fieldHospitals.Count;
            float overallEffectiveness = Mathf.Min((float)GameManager.simMan.simParam.fieldHospitalVisitorsThreshold / visitorsPerHospital , 1.0f);
            float averageHospitalEffectiveness = 0.0f;
            
            foreach(FieldHospital hospital in GameManager.buildMan.fieldHospitals)
            {
                averageHospitalEffectiveness += hospital.ComputeEffectiveness();
            }
            averageHospitalEffectiveness = averageHospitalEffectiveness / (float) GameManager.buildMan.fieldHospitals.Count;

            // print("averageHospitalEffectiveness:" + averageHospitalEffectiveness.ToString());//test
            // print("overallEffectiveness:" + overallEffectiveness.ToString());//test
            // print("visitorsPerHospital:" + visitorsPerHospital.ToString());//test

            healthGain = (float)GameManager.simMan.simParam.baseFieldHospitalHealthRestore * averageHospitalEffectiveness * overallEffectiveness;
        }
        //add some randomness:
        healthGain = Random.Range(0.85f * healthGain, 1.15f * healthGain);

        //print("Health Gain:" + healthGain.ToString());//test

        //then modify workers health
        foreach (Worker worker in population.all)
        {
            uint health = worker.health;

            //extreme hunger halves health
            if (worker.food < GameManager.simMan.simParam.malnourishmentThreshold)
                health = (uint)Mathf.RoundToInt((float) health / 2.0f);

            float healthLoss = (float)GameManager.simMan.simParam.baseHealthLossRate;
            
            //compute trait effects.
            foreach (WorkerTrait trait in worker.traits)
            {
                switch(trait)
                {
                    case WorkerTrait.coward:
                        healthLoss = healthLoss * 0.7f;
                        break;
                    case WorkerTrait.athletic:
                        healthLoss = healthLoss * 0.7f;
                        break;
                    case WorkerTrait.weak:
                        healthLoss = healthLoss * 1.7f;
                        break;
                }
            }
            //add some randomness:
            healthLoss = Random.Range(0.85f * healthLoss, 1.15f * healthLoss);

            health = (uint)Mathf.Clamp((int)health + Mathf.RoundToInt(healthGain - healthLoss), 0, 100);
            worker.SetHealth(health);

            if (health == 0)
                workersToKill.Add(worker);
        }
    }

    public void UpdateWorkersFood()
    {
        float foodGain = 0.0f;

        if (GameManager.buildMan.canteens.Count > 0)
        {
            //overall effectiveness => to account for overload (more visitors than facility can handle)

            float visitorsPerCanteen = (float)GameManager.popMan.CountAll() / (float)GameManager.buildMan.canteens.Count;
            float overallEffectiveness = Mathf.Min((float)GameManager.simMan.simParam.fieldHospitalVisitorsThreshold / visitorsPerCanteen, 1.0f);
            float averageCanteenEffectiveness = 0.0f;
            
            foreach(FieldHospital hospital in GameManager.buildMan.fieldHospitals)
            {
                averageCanteenEffectiveness += hospital.ComputeEffectiveness();
            }
            averageCanteenEffectiveness = averageCanteenEffectiveness / (float) GameManager.buildMan.fieldHospitals.Count;

            foodGain = (float)GameManager.simMan.simParam.baseFieldHospitalHealthRestore * averageCanteenEffectiveness * overallEffectiveness;
        }
        //add some randomness
        foodGain = Random.Range(0.85f * foodGain, 1.15f * foodGain);

        //print("Food Gain:" + foodGain.ToString());//test

        //then modify workers health
        foreach (Worker worker in population.all)
        {
            uint food = worker.food;
            float foodLoss = (float)GameManager.simMan.simParam.baseFoodLossRate;
            
            foreach (WorkerTrait trait in worker.traits)
            {
                switch(trait)
                {
                    default:
                        break;
                }
            }
            //add some randomness
            foodLoss = Random.Range(0.85f * foodLoss, 1.15f * foodLoss);

            food = (uint)Mathf.Clamp((int)food + Mathf.RoundToInt(foodGain - foodLoss), 0, 100);
            worker.SetFood(food);
        }
    }

    public void UpdateWorkersSanity()
    {
        float sanityGain = 0.0f;

        //print("Sanity Gain:" + sanityGain.ToString());//test

        foreach (Worker worker in population.all)
        {
            uint sanity = worker.sanity;
            float sanityLoss = (float)GameManager.simMan.simParam.baseSanityLossRate;
            
            foreach (WorkerTrait trait in worker.traits)
            {
                switch(trait)
                {
                    default:
                        break;
                }
            }

            sanity = (uint)Mathf.Clamp((int)sanity + Mathf.RoundToInt(sanityGain - sanityLoss), 0, 100);
            worker.SetSanity(sanity);
        }
    }

    public List<Name> DeadWorkersCleanup()
    {
        List<Name> deadList = new List<Name>();
        print ("Cleaning up dead workers, count: " + workersToKill.Count);
        
        for (int i = workersToKill.Count - 1; i >= 0; i--)
        {
            deadList.Add(workersToKill[i].name);
            workersToKill[i].PrepareForRemoval();
            population.RemoveWorker(workersToKill[i]);
            workersToKill.RemoveAt(i);
        }

        return deadList;
    }

     public void GlobalHealtyhChange (int delta) //change everyone health based on delta (positive = restore, negative = loss)
    {
        foreach (Worker worker in population.all)
        {
            int change = delta;
            //check for modifying traits
            foreach (WorkerTrait trait in worker.traits)
            {
                switch(trait)
                {
                    case WorkerTrait.coward:
                        if (change < 0)
                            change = Mathf.RoundToInt(change * 0.5f);
                        break;
                    case WorkerTrait.cautious:
                        if (change < 0)
                            change = Mathf.RoundToInt(change * 0.5f);
                        break;
                }
            }
            
            //apply change
            worker.SetHealth((uint)Mathf.Clamp((int)worker.health + change, 0, 100));
        }
    }

    public void GlobalSanityChange (int delta) //change everyone sanity based on delta (positive = restore, negative = loss)
    {
        foreach (Worker worker in population.all)
        {
            int change = delta;
            //check for modifying traits
            foreach (WorkerTrait trait in worker.traits)
            {
                switch(trait)
                {
                    case WorkerTrait.coward:
                        if (change < 0)
                            change = Mathf.RoundToInt(change * 1.5f);
                        break;
                }
            }
            //apply change
            worker.SetSanity((uint)Mathf.Clamp((int)worker.sanity + change, 0, 100));
        }
    }

    public void GlobalFoodChange (int delta) //change everyone sanity based on delta (positive = restore, negative = loss)
    {
        foreach (Worker worker in population.all)
        {
            int change = delta;
            //check for modifying traits
            foreach (WorkerTrait trait in worker.traits)
            {
                switch(trait)
                {
                    case WorkerTrait.glutton:
                        if (change < 0)
                            change = Mathf.RoundToInt((Mathf.Sign(change) - 0.5f) * (float)change); //basically, 150% if negative, 50% if positive.
                        break;
                }
            }
            //apply change
            worker.SetFood((uint)Mathf.Clamp((int)worker.food + change, 0, 100));
        }
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

    public float ComputeLoungeTraitBonus()
    {
        float bonus = 0.0f;

        return bonus;
    }
    
    public float ComputeLatrineTraitBonus()
    {
        float bonus = 0.0f;

        return bonus;
    }

    public float ComputeCanteenTraitBonus()
    {
        float bonus = 0.0f;

        return bonus;
    }

    public ulong ComputeTotalWages()
    {
        ulong result = 0;

        foreach (Worker worker in population.all)
        {
            result += worker.wage;
        }

        return result;
    }

    //Testing methods
    // void OnGUI()
    // {
    //     GUIStyle style = new GUIStyle();
    //     style.fontSize = 35;

    //     GUI.Label(new Rect(10, 80, 100, 20), "Population: " + CountAll().ToString(), style);
    //     style.fontSize = 25;
    //     GUI.Label(new Rect(10, 120, 100, 20), "Archelogists: " + Count(WorkerType.archaeologist).ToString() + " | Geologists: " + Count(WorkerType.geologist).ToString(), style);
    //     GUI.Label(new Rect(10, 150, 100, 20), "Excavators: " + Count(WorkerType.excavator).ToString() + " | Cooks: " + Count(WorkerType.cook).ToString() + " | physicians: " + Count(WorkerType.physician).ToString(), style);
    // }
}

public class Population
{
    public List<Worker> all {get; private set;}
    public List<Worker> excavators {get; private set;}
    public List<Worker> geologists {get; private set;}
    public List<Worker> archaelogists {get; private set;}
    public List<Worker> physicians {get; private set;}
    public List<Worker> cooks {get; private set;}

    public Population()
    {
        all = new  List<Worker>();
        excavators = new List<Worker>();
        geologists = new List<Worker>();
        archaelogists = new List<Worker>();
        physicians = new List<Worker>();
        cooks = new List<Worker>();
    }

    public void AddWorker(Worker worker)
    {
        all.Add(worker);
        
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
                return (archaelogists.Remove(worker) && all.Remove(worker));
            case WorkerType.geologist:
                return (geologists.Remove(worker) && all.Remove(worker));
            case WorkerType.excavator:
                return (excavators.Remove(worker) && all.Remove(worker));
            case WorkerType.physician:
                return (physicians.Remove(worker) && all.Remove(worker));
            case WorkerType.cook:
                return (cooks.Remove(worker) && all.Remove(worker));
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
        //return (uint)(archaelogists.Count + geologists.Count + excavators.Count + physicians.Count + cooks.Count);
        return (uint) all.Count;
    }

    public void Clear() //clears all lists
    {
        all.Clear();
        archaelogists.Clear();
        geologists.Clear();
        excavators.Clear();
        physicians.Clear();
        cooks.Clear();
    }
}