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
        dashboard = GameManager.canvas.transform.Find("BuildingDashboards").Find("GenericUnManned").gameObject;
        description = "Your workers one and only entertainment in this sea of nothing. Lounges restore your crews sanity, but make sure they aren't overcrowded.";
    }

    public override float ComputeEffectiveness()
    {
        effectiveness = GameManager.simMan.simParam.baseLoungeEffectiveness;
        //add budget effect
        effectiveness = BudgetEffect() * effectiveness;
        //add current effects 
        effectiveness = effectiveness * GameManager.simMan.statEffects.loungeEffectModifier + GameManager.simMan.statEffects.loungEffectBonus;

        //compute traits effect
        effectiveness += GameManager.popMan.ComputeLoungeTraitBonus();

        return effectiveness;
    }

    public override void ShowBuildingDashboard()
    {
        dashboard.SetActive(true);
        dashboard.GetComponent<Dashboard>().Show(this, WorkerType.generic);
    }

}
