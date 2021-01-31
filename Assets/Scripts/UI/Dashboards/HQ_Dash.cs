using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HQ_Dash : Dashboard
{
    //Button hireArchaelogoistButton;
    Button hireExcavatorButton;
    Button fireExcavatorButton;
    //Text archaelogistsCount;
    Text excavatorsCount;

    //HQ hq = null; //set by calling object

    
    protected override void Awake()
    {
        mainWorkerType = WorkerType.archaeologist;
        base.Awake();

        //hireArchaelogoistButton = this.transform.Find("HireArchaelogist").GetComponent<Button>();
        hireExcavatorButton = this.transform.Find("HireExcavator").GetComponent<Button>();

        fireExcavatorButton = this.transform.Find("FireExcavator").GetComponent<Button>();

        //archaelogistsCount = this.transform.Find("ArchaelogistsCount").GetComponent<Text>();
        excavatorsCount = this.transform.Find("ExcavatorsCount").GetComponent<Text>();

        
        //disable dashboard until needed.
        //this.gameObject.SetActive(false);
    }

    public override void Show(Building callingBuilding, WorkerType workerType = WorkerType.generic)
    {
        base.Show(callingBuilding, WorkerType.archaeologist);

        //hq = callingBuilding.GetComponent<HQ>();
        // UpdateButtons();
        // UpdateStats();
    }

    protected override void UpdateButtons()
    {
        //Check whether we can hire an archaelogist
        // if (GameManager.popMan.CanHireWorker(WorkerType.archaeologist, hq))
        //     hireArchaelogoistButton.enabled = true;
        // else
        //     hireArchaelogoistButton.enabled = false;
        base.UpdateButtons();


        //check whether we can hire an excavator
        hireExcavatorButton.interactable = GameManager.popMan.CanHireWorker(WorkerType.excavator, null);

        fireExcavatorButton.interactable = GameManager.popMan.Count(WorkerType.excavator) > 0;
    }

    protected override void UpdateStats()
    {
        base.UpdateStats();
        //archaelogistsCount.text = hq.assignedWorkers.Count.ToString() + " / " + hq.GetStats().capacity.ToString();

        uint currentExcavCount = GameManager.popMan.Count(WorkerType.excavator);
        int maxExcavCount = Mathf.RoundToInt(Mathf.Min(currentExcavCount + GameManager.buildMan.TotalAvailableBeds(),
                                                        (int)GameManager.simMan.simParam.excavatorsPerArchaelogist * (int)GameManager.popMan.Count(WorkerType.archaeologist)));
        excavatorsCount.text = currentExcavCount.ToString() + " / " + maxExcavCount.ToString();
    }

    public void HireExcavator()
    {
        GameManager.gameMan.HireWorker(WorkerType.excavator, building);
        UpdateButtons();
        UpdateStats();
    }

    public void FireExcavator()
    {
        GameManager.gameMan.FireWorker(WorkerType.excavator, building);
        UpdateButtons();
        UpdateStats();
    }

    public void SetExcavationTarget()
    {
        GameManager.gameMan.StartSelectingExcavationArea();
    }

    public override void Close()
    {
        //hq = null;
        base.Close();
    }
}