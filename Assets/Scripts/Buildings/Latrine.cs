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
        dashboard = GameManager.canvas.transform.Find("BuildingDashboards").Find("GenericUnManned").gameObject;
        description = "What comes in must go out, somehow, and it's better for your crew's sanity that they don't be wading through their own excrement when they're not doing so on hot sand.";
    }

    public override float ComputeEffectiveness()
    {
        effectiveness = GameManager.simMan.simParam.baseLatrineEffectiveness;
        //add budget effect
        effectiveness = BudgetEffect() * effectiveness;
        //add current effects 
        effectiveness = effectiveness * GameManager.simMan.statEffects.latrineEffectModifier + GameManager.simMan.statEffects.latrineEffectBonus;

        //compute traits effect
        effectiveness += GameManager.popMan.ComputeLatrineTraitBonus();

        return effectiveness;
    }

    public override void ShowBuildingDashboard()
    {
        dashboard.SetActive(true);
        dashboard.GetComponent<Dashboard>().Show(this, WorkerType.generic);
    }


}
