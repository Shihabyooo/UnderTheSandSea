using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeologyLab : Building
{
    protected override void Awake()
    {
        base.Awake();
        stats.type = BuildingType.geologyLab;
        effectiveness = GameManager.simMan.simParam.baseGeologyLabEffectiveness;
        dashboard = GameManager.canvas.transform.Find("BuildingDashboards").Find("GenericManned").gameObject;
        description = "Geology labs house those rare breed of eggheads who just would drop to the floor when the spot a shiny stone faster than you'd blink.\nGeology labs allow for helpfull special projects.";
    }

    public override float ComputeEffectiveness()
    {
        if (assignedWorkers.Count == 0)
        {
            effectiveness = 0.0f;
        }
        else
        {
            effectiveness = GameManager.simMan.simParam.baseGeologyLabEffectiveness;
            effectiveness = effectiveness * (float)assignedWorkers.Count / (float)stats.capacity;
            //add budget effect
            effectiveness = BudgetEffect() * effectiveness;
            //add current effects 
            effectiveness = effectiveness * GameManager.simMan.statEffects.geologyLabEffectModifier + GameManager.simMan.statEffects.geologyLabEffectBonus;
        }
        return effectiveness;
    }

    public override void ShowBuildingDashboard()
    {
        dashboard.SetActive(true);
        dashboard.GetComponent<Dashboard>().Show(this, WorkerType.geologist);
    }

}
