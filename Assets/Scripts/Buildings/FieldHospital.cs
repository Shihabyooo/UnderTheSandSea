﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldHospital : Building
{
    protected override void Awake()
    {
        base.Awake();
        stats.type = BuildingType.fieldHospital;
        effectiveness = GameManager.simMan.simParam.baseFieldHospitalEffectiveness;
    }

    public override float ComputeEffectiveness()
    {
        if (assignedWorkers.Count == 0)
        {
            effectiveness = 0.0f;
        }
        else
        {
            effectiveness = GameManager.simMan.simParam.baseFieldHospitalEffectiveness;
            effectiveness = effectiveness * (float)assignedWorkers.Count / (float)stats.capacity;
            //add current effects 
            effectiveness = effectiveness * GameManager.simMan.statEffects.fieldHospitalEffectModifier + GameManager.simMan.statEffects.fieldHospitalEffectBonus;
        }

        return effectiveness;
    }
}