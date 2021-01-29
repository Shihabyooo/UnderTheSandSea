using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public enum WorkerType
    {
        excavator, geologist, archaeologist, physician, cook, generic
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
        highlySkilled, //overall better performance and less chance of negative effects from special tasks, higher wage (+40%).
        unskilled, //overall lower performance and greater chance for negative effects from special tasks, lower wage (-40%).
    }

[System.Serializable]
public class Worker
{
    public List<WorkerTrait> traits {get; private set;}
    public Name name {get; private set;}
    System.DateTime hiringDate;
    public uint health {get; private set;}
    public uint sanity {get; private set;}
    public uint wage {get; private set;}
    public WorkerType type {get; private set;}
    
    public Worker(Name workerName, System.DateTime workerHiringDate, WorkerType workerType, uint workerHealth = 100, uint workerSanity = 100)
    {
        name = workerName;
        hiringDate = workerHiringDate;
        health = workerHealth;
        sanity = workerSanity;
        type = workerType;

        traits = new List<WorkerTrait>(); //starts with an empty trait list.

        wage = 0;

        switch(type)
        {
            case WorkerType.archaeologist:
                wage = (uint)Mathf.RoundToInt( Random.Range(0.9f, 1.1f) * (float)BaseWages.archaeologist);
                break;
            case WorkerType.geologist:
                wage = (uint)Mathf.RoundToInt( Random.Range(0.9f, 1.1f) * (float)BaseWages.geologist);
                break;
            case WorkerType.excavator:
                wage = (uint)Mathf.RoundToInt( Random.Range(0.9f, 1.1f) * (float)BaseWages.excavator);
                break;
            case WorkerType.cook:
                wage = (uint)Mathf.RoundToInt( Random.Range(0.9f, 1.1f) * (float)BaseWages.cook);
                break;
            case WorkerType.physician:
                wage = (uint)Mathf.RoundToInt( Random.Range(0.9f, 1.1f) * (float)BaseWages.physician);
                break;
            case WorkerType.generic:
                break;
            default:
                break;
        }

        foreach(WorkerTrait trait in traits)
        {
            if (trait == WorkerTrait.highlySkilled)
                wage = (uint)Mathf.CeilToInt(1.2f * (float)wage);
            else if (trait == WorkerTrait.unskilled)
                wage = (uint)Mathf.CeilToInt(0.8f * (float)wage);
        }

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

    public uint Production()
    {
        return 0;
    }

}

public struct BaseWages
{
    public const uint geologist = 150;
    public const uint archaeologist = 250;
    public const uint physician = 150;
    public const uint cook = 100;
    public const uint excavator = 50;
}