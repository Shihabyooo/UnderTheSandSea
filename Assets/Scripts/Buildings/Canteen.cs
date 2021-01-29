﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canteen : Building
{
    protected override void Awake()
    {
        base.Awake();
        stats.type = BuildingType.canteen;
        effectiveness = GameManager.simMan.simParam.baseCanteentEffectiveness;
    }

    public override float ComputeEffectiveness()
    {
        if (assignedWorkers.Count == 0)
        {
            effectiveness = 0.0f;
        }
        else
        {
            effectiveness = GameManager.simMan.simParam.baseCanteentEffectiveness;
            effectiveness = effectiveness * (float)assignedWorkers.Count / (float)stats.capacity;
            //add current effects 
            effectiveness = effectiveness * GameManager.simMan.statEffects.canteentEffectModifier + GameManager.simMan.statEffects.canteentEffectBonus;

            //compute traits effect
            effectiveness += GameManager.popMan.ComputeCanteenTraitBonus();
        }
        return effectiveness;
    }
}