using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HQ_Dash : MonoBehaviour
{
    Button hireArchaelogoistButton;
    Button hireExcavatorButton;
    Text archaelogistsCount;
    Text excavatorsCount;

    HQ hq = null; //set by calling object

    
    void Awake()
    {
        hireArchaelogoistButton = this.transform.Find("HireArchaelogist").GetComponent<Button>();
        hireExcavatorButton = this.transform.Find("HireExcavator").GetComponent<Button>();
        
        archaelogistsCount = this.transform.Find("ArchaelogistsCount").GetComponent<Text>();
        excavatorsCount = this.transform.Find("ExcavatorsCount").GetComponent<Text>();

        //disable dashboard until needed.
        this.gameObject.SetActive(false);
    }

    public void Reinitialize(HQ callingHQ)
    {
        hq = callingHQ;
        UpdateButtons();
        UpdateStats();
    }

    void UpdateButtons()
    {
        //Check whether we can hire an archaelogist
        //if (hq.AvailableSlots() < 1 || GameManager.buildMan.TotalAvailableBeds() < 1)
        if (GameManager.popMan.CanHireWorker(WorkerType.archaeologist, hq))
            hireArchaelogoistButton.enabled = true;
        else
            hireArchaelogoistButton.enabled = false;
        
        //if (GameManager.buildMan.TotalAvailableBeds() < 1)
        if (GameManager.popMan.CanHireWorker(WorkerType.excavator, null))
            hireExcavatorButton.enabled = true;
        else
            hireExcavatorButton.enabled = false;
    }

    void UpdateStats()
    {
        archaelogistsCount.text = hq.assignedWorkers.Count.ToString() + " / " + hq.GetStats().capacity.ToString();

        uint currentExcavCount = GameManager.popMan.Count(WorkerType.excavator);
        int maxExcavCount = Mathf.RoundToInt(Mathf.Min(currentExcavCount + GameManager.buildMan.TotalAvailableBeds(),
                                                        (int)GameManager.simMan.simParam.excavatorsPerArchaelogist * (int)GameManager.popMan.Count(WorkerType.archaeologist)));
        excavatorsCount.text = currentExcavCount.ToString() + " / " + maxExcavCount.ToString();
    }

    public void HireArchaelogoist()
    {
        GameManager.gameMan.HireWorker(WorkerType.archaeologist, hq);
        UpdateButtons();
        UpdateStats();
    }

    public void HireExcavator()
    {
        GameManager.gameMan.HireWorker(WorkerType.excavator, hq);
        UpdateButtons();
        UpdateStats();
    }

    public void SetExcavationTarget()
    {
        GameManager.gameMan.StartSelectingExcavationArea();
    }

    public void Close()
    {
        hq = null;
        this.gameObject.SetActive(false);
    }
}




