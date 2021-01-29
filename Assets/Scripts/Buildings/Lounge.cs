using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lounge : Building
{
    protected override void Awake()
    {
        base.Awake();
        stats.type = BuildingType.lounge;
        effectiveness = GameManager.simMan.simParam.baseLoungeEffectiveness;
    }

    public override float ComputeEffectiveness()
    {
        effectiveness = GameManager.simMan.simParam.baseLoungeEffectiveness;
        //add current effects 
        effectiveness = effectiveness * GameManager.simMan.statEffects.loungeEffectModifier + GameManager.simMan.statEffects.loungEffectBonus;

        //compute traits effect
        effectiveness += GameManager.popMan.ComputeLoungeTraitBonus();

        return effectiveness;
    }
}
