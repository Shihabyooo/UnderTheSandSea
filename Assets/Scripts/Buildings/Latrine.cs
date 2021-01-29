using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Latrine : Building
{
   protected override void Awake()
    {
        base.Awake();
        stats.type = BuildingType.latrine;
        effectiveness = GameManager.simMan.simParam.baseLatrineEffectiveness;
    }

    public override float ComputeEffectiveness()
    {
        //latrines has base effitiveness of 1.0f
        //Effectiveness increased (or decreased) with worker traits
        
        effectiveness = GameManager.simMan.simParam.baseLatrineEffectiveness;
        //add current effects 
        effectiveness = effectiveness * GameManager.simMan.statEffects.latrineEffectModifier + GameManager.simMan.statEffects.latrineEffectBonus;

        //compute traits effect
        effectiveness += GameManager.popMan.ComputeLatrineTraitBonus();

        return effectiveness;
    }

}
