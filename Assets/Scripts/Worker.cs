using System.Collections;
using System.Collections.Generic;
//using UnityEngine;

 public enum WorkerType
    {
        excavator, geologist, archaeologist, physician, cook
    }

    public enum WorkerTrait //For future implementation
    {
        kleptomaniac, //slightly increase daily spending due to "missing" items from camp.
        joker, //slightly improves performance of sanity restoring buildings.
        singer, //slightly improves performance of sanity restoring buildings.
        coward, //sanity affected too much by negative events, health drops less.
        athletic, //slightly increases productivity of manual labour, health drops less.
        glutton, //requires more food.
        lazy, //lower productivity.
        cautious, //less chance of causing negative events due to work on special tasks, but less overall performance.
        highlySkilled, //overall better performance and less chance of negative effects from special tasks, higher wage.
    }

public class Worker
{
    public List<WorkerTrait> traits {get; private set;}
    public string name {get; private set;}
    System.DateTime hiringDate;
    public uint health {get; private set;}
    public uint sanity {get; private set;}
    public uint wage {get; private set;}

    public Worker(string workerName, System.DateTime workerHiringDate, uint workerWage, uint workerHealth = 100, uint workerSanity = 100)
    {
        name = workerName;
        hiringDate = workerHiringDate;
        health = workerHealth;
        sanity = workerSanity;
        wage = workerWage;

        traits = new List<WorkerTrait>(); //starts with an empty trait list.
    }

    //setters
    public void SetHealth(uint newHealth)
    {
        health = newHealth;
    }

    public void SetSanity(uint newSanity)
    {
        sanity = newSanity;
    }

    public void AddTrait(WorkerTrait newTrait)
    {
        if (!HasTrait(newTrait))
            traits.Add(newTrait);
    }

    public bool RemoveTrait(WorkerTrait trait)
    {
        return traits.Remove(trait);    
    }

    //getters and other returns
    public bool HasTrait(WorkerTrait trait)
    {
        foreach(WorkerTrait existingTrait in traits)
        {
            if (existingTrait == trait)
                return true;
        }

        return false;
    }
}