using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HQ : Building
{
    protected override void Awake()
    {
        base.Awake();
        dashboard = GameManager.canvas.transform.Find("BuildingDashboards").Find("HQ").gameObject;
    }

    public override void ShowBuildingDashboard()
    {
        dashboard.SetActive(true);
        dashboard.GetComponent<HQ_Dash>().Reinitialize(this);

    }
}