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

    public float Production()
    {
        float performance = GameManager.simMan.simParam.baseExcavatorPerformance;
        
        //modify performance based on health, and traits (if applicable)
        //Using a slightly modified Logistic function Y = L/(1+exp(-k(x-x0))) + a
            // L = Max Value (1.0 here), k = growth rate, x0 = midpoint x, a = modification
        
        float healthEffect = 1.0f / (1.0f + Mathf.Exp(-0.07f*((float)health - 55.0f))) + 0.04f;
        
        float bonus = 0.0f;

        foreach (WorkerTrait trait in traits)
        {
            switch (trait)
            {
                case WorkerTrait.athletic:
                    bonus += 0.075f;
                    break;
                case WorkerTrait.lazy:
                    bonus -= 0.1f;
                    break;
                case WorkerTrait.cautious:
                    bonus -= 0.05f;
                    break;
                case WorkerTrait.highlySkilled:
                    bonus += 0.1f;
                    break;
                case WorkerTrait.unskilled:
                    bonus -= 0.075f;
                    break;
            }
        }

        performance = Mathf.Max(performance * healthEffect + performance * bonus, 0.0f);

        return performance;
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