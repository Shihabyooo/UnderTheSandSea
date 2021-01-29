using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canteen : Building
{
    protected override void Awake()
    {
        base.Awake();
        stats.type = BuildingType.canteen;
        effectiveness = GameManager.simMan.simParam.baseCanteentEffectiveness;
        dashboard = GameManager.canvas.transform.Find("BuildingDashboards").Find("GenericManned").gameObject;

        description = "Canteens provide your workers with food. Performance of canteents is affected by staffing, and number of serviced workers.\nMalnourished workers lose significant health, so make sure they're well staffed, well budgeted and not overcrowded!";
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

    public override void ShowBuildingDashboard()
    {
        dashboard.SetActive(true);
        dashboard.GetComponent<Dashboard>().Show(this, WorkerType.cook);
    }
}
