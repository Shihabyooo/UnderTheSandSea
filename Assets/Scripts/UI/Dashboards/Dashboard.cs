using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dashboard : MonoBehaviour
{

    protected WorkerType mainWorkerType;
    protected Button mainWorkerHireButton = null;
    protected Button mainWorkerFireButton = null;
    protected Text mainWorkerCount = null;
    protected Building building = null;
    Slider budgetSlider = null;
    Text budgetText = null;
    Text effectiveness = null;

    protected virtual void Awake()
    {
        if (this.transform.Find("HireMainWorker") != null)
        {
            mainWorkerHireButton = this.transform.Find("HireMainWorker").GetComponent<Button>();
            mainWorkerFireButton = this.transform.Find("FireMainWorker").GetComponent<Button>();
            mainWorkerCount = this.transform.Find("MainWorkerCount").GetComponent<Text>();
        }
        
        if (this.transform.Find("BudgetSlider") != null)
        {
            budgetSlider = this.transform.Find("BudgetSlider").GetComponent<Slider>();
            budgetSlider.onValueChanged.AddListener(OnBudgetSliderChange);
        }

        if (this.transform.Find("Budget") != null)
            budgetText = this.transform.Find("Budget").GetComponent<Text>();

        effectiveness = this.transform.Find("Effectiveness").GetComponent<Text>();
    }

    public virtual void Show(Building callingBuilding, WorkerType workerType = WorkerType.generic)
    {
        GameManager.uiMan.SwitchDashboard(this);
        building = callingBuilding;
        mainWorkerType = workerType;

        if (this.transform.Find("MainWorkerHeading") != null)
            this.transform.Find("MainWorkerHeading").GetComponent<Text>().text = "Assigned " + Worker.WorkerTypeString(workerType)  + ":";
        
        this.transform.Find("Description").GetComponent<Text>().text = building.description;
        this.transform.Find("Title").GetComponent<Text>().text = building.gameObject.name;

        UpdateButtons();
        UpdateStats();
        UpdateBudget();
    
    }

    protected virtual void UpdateButtons()
    {
        if (mainWorkerHireButton != null)
            mainWorkerHireButton.interactable = GameManager.popMan.CanHireWorker(mainWorkerType, building);
        if (mainWorkerFireButton != null)
            mainWorkerFireButton.interactable = building.assignedWorkers.Count > 0;
    }

    protected virtual void UpdateStats()
    {
        if (mainWorkerCount != null)
            mainWorkerCount.text = building.assignedWorkers.Count.ToString() + " / " + building.GetStats().capacity.ToString();
        effectiveness.text = Mathf.RoundToInt(building.ComputeEffectiveness() * 100.0f).ToString() + "%";
    }

    protected virtual void UpdateBudget()
    {
        if(budgetSlider != null)
        {
            //Changing the values of the slider triggers OnBudgetSliderChange, causing unwanted modifications of budget value in building object, so we remove the listner,
            //do our changes, then reset the listener.
            budgetSlider.onValueChanged.RemoveListener(OnBudgetSliderChange);
            budgetSlider.minValue = building.GetStats().minBudget;
            budgetSlider.maxValue = building.GetStats().maxBudget;
            budgetSlider.value = building.budget;
            budgetSlider.onValueChanged.AddListener(OnBudgetSliderChange);
        }

        if (budgetText != null)
            budgetText.text = "$" + building.budget.ToString();
    }

    public virtual void OnBudgetSliderChange(float value) //will be assigned to slider in Editor, so we don't need to check that this instance has one.
    {
        building.SetBudget((uint)budgetSlider.value);
        budgetText.text = "$" + building.budget.ToString();
        UpdateStats();
    }

    public void HireMainWorker()
    {
        GameManager.gameMan.HireWorker(mainWorkerType, building);
        UpdateButtons();
        UpdateStats();
    }

    public void FireMainWorker()
    {
        GameManager.gameMan.FireWorker(mainWorkerType, building);
        UpdateButtons();
        UpdateStats();
    }

    public virtual void Close()
    {
        this.gameObject.SetActive(false);
    }
}
