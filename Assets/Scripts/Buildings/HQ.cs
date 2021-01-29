using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HQ : Building
{
    protected override void Awake()
    {
        base.Awake();
        stats.type = BuildingType.hq;
        effectiveness = GameManager.simMan.simParam.baseHQEffectiveness;
        dashboard = GameManager.canvas.transform.Find("BuildingDashboards").Find("HQ").gameObject;
    }

    public override void ShowBuildingDashboard()
    {
        dashboard.SetActive(true);
        dashboard.GetComponent<HQ_Dash>().Reinitialize(this);
    }

    public override float ComputeEffectiveness()
    {
        if (assignedWorkers.Count == 0)
        {
            effectiveness = 0.0f;
        }
        else
        {
            effectiveness = GameManager.simMan.simParam.baseHQEffectiveness;
            effectiveness = effectiveness * (float)assignedWorkers.Count / (float)stats.capacity;
            //add current effects 
            effectiveness = effectiveness * GameManager.simMan.statEffects.hqEffectModifier + GameManager.simMan.statEffects.hqEffectBonus;
        }
        return effectiveness;
    }
}