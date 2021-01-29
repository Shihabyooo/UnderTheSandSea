using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dashboard : MonoBehaviour
{

    protected WorkerType mainWorkerType;
    protected Button mainWorkerHireButton = null;
    protected Text mainWorkerCount = null;
    protected Building building = null;
    Slider budgetSlider = null;
    Text budgetText = null;

    protected virtual void Awake()
    {
        if (this.transform.Find("HireMainWorker") != null)
        {
            mainWorkerHireButton = this.transform.Find("HireMainWorker").GetComponent<Button>();
            mainWorkerCount = this.transform.Find("MainWorkerCount").GetComponent<Text>();
        }
        if (this.transform.Find("BudgetSlider") != null)
        {
            budgetText = this.transform.Find("Budget").GetComponent<Text>();
            budgetSlider = this.transform.Find("BudgetSlider").GetComponent<Slider>();
            budgetSlider.onValueChanged.AddListener(OnBudgetSliderChange);
        }
    }

    public virtual void Show(Building callingBuilding, WorkerType workerType = WorkerType.generic)
    {
        GameManager.uiMan.SwitchDashboard(this);
        building = callingBuilding;
        mainWorkerType = workerType;
        this.transform.Find("MainWorkerHeading").GetComponent<Text>().text = "Assigned " + Worker.WorkerTypeString(workerType)  + ":";
        this.transform.Find("Description").GetComponent<Text>().text = building.description;
        this.transform.Find("Title").GetComponent<Text>().text = building.gameObject.name;

        UpdateButtons();
        UpdateStats();
        UpdateBudget();
    
    }

    protected virtual void UpdateButtons()
    {
        mainWorkerHireButton.enabled = GameManager.popMan.CanHireWorker(mainWorkerType, building);
    }

    protected virtual void UpdateStats()
    {
        mainWorkerCount.text = building.assignedWorkers.Count.ToString() + " / " + building.GetStats().capacity.ToString();
    }

    protected virtual void UpdateBudget()
    {
        if(budgetSlider == null)
            return;
        
        budgetSlider.minValue = building.GetStats().minBudget;
        budgetSlider.maxValue = building.GetStats().maxBudget;
        budgetSlider.value = building.budget;

        budgetText.text = "$" + building.budget.ToString();
    }

    public virtual void OnBudgetSliderChange(float value) //will be assigned to slider in Editor, so we don't need to check that this instance has one.
    {
        building.SetBudget((uint)budgetSlider.value);
        budgetText.text = "$" + building.budget.ToString();
    }

    public void HireMainWorker()
    {
        GameManager.gameMan.HireWorker(mainWorkerType, building);
        UpdateButtons();
        UpdateStats();
    }


    public virtual void Close()
    {
        this.gameObject.SetActive(false);
    }
}
