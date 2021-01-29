using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeologyLab : Building
{
    protected override void Awake()
    {
        base.Awake();
        stats.type = BuildingType.geologyLab;
        effectiveness = GameManager.simMan.simParam.baseGeologyLabEffectiveness;
    }

    public override float ComputeEffectiveness()
    {
        if (assignedWorkers.Count == 0)
        {
            effectiveness = 0.0f;
        }
        else
        {
            effectiveness = GameManager.simMan.simParam.baseGeologyLabEffectiveness;
            effectiveness = effectiveness * (float)assignedWorkers.Count / (float)stats.capacity;
            //add current effects 
            effectiveness = effectiveness * GameManager.simMan.statEffects.geologyLabEffectModifier + GameManager.simMan.statEffects.geologyLabEffectBonus;
        }
        return effectiveness;
    }

}
