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
        description = "The HQ is the centre of your camp, from which all operations are managed. The HQ can be run by up to three Archaelogists. Excavation workers can be hired from the HQ.";
    }

    public override void ShowBuildingDashboard()
    {
        dashboard.SetActive(true);
        dashboard.GetComponent<HQ_Dash>().Show(this);
    }

    public override void BeginConstruction(Cell cell)
    {
        base.BeginConstruction(cell);
        //easiest solution to ensure only one HQ per game, we lock the button once we begin constructing the first!
        //We'll have to remember unlocking it in GameManager.StartNewSimulation().
        GameManager.canvas.transform.Find("ConstructionMenu").Find("HQ").GetComponent<BuildingButton>().SetButtonLockState(true);

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