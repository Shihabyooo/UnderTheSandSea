using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepingTent : Building
{
    protected override void Awake()
    {
        base.Awake();
        stats.type = BuildingType.sleepTent;
        effectiveness = GameManager.simMan.simParam.baseSleepingTentEffectiveness;
        dashboard = GameManager.canvas.transform.Find("BuildingDashboards").Find("GenericUnManned").gameObject;
        description = "It's a place to sleep in. What else is there to tell?";
    }

    public uint AvailableBeds()
    {
        return AvailableSlots();
    }


    public override void ShowBuildingDashboard()
    {
        dashboard.SetActive(true);
        dashboard.GetComponent<Dashboard>().Show(this, WorkerType.generic);
    }

}
