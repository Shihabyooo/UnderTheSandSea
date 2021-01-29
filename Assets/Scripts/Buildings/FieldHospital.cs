using System.Collections;
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
        effectiveness = GameManager.simMan.simParam.baseFieldHospitalEffectiveness;
        //add current effects 
        effectiveness = effectiveness * GameManager.simMan.statEffects.fieldHospitalEffectModifier + GameManager.simMan.statEffects.fieldHospitalEffectBonus;

        return effectiveness;
    }
}
