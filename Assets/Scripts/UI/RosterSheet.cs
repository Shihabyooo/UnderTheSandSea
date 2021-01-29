using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RosterSheet : MonoBehaviour
{
    Text names;
    Text roles;
    Text health;
    Text sanity;
    Text food;


    void Awake()
    {
        names = this.transform.Find("Scroll View").Find("Viewport").Find("Content").GetComponent<Text>();
        roles = names.transform.Find("Roles").GetComponent<Text>();
        health = names.transform.Find("Health").GetComponent<Text>();
        sanity = names.transform.Find("Sanity").GetComponent<Text>();
        food = names.transform.Find("Food").GetComponent<Text>();


        Close();
    }

    public void Show()
    {
        ToggleVisibility(true);
        UpdateContent();
    }


    public void UpdateContent()
    {
        Population population = GameManager.popMan.population;
        names.text = "";
        roles.text = "";
        health.text ="";
        sanity.text = "";
        food.text = "";
        
        AddBlockToRoster(population.archaelogists);
        AddBlockToRoster(population.geologists);
        AddBlockToRoster(population.physicians);
        AddBlockToRoster(population.cooks);
        AddBlockToRoster(population.excavators);
    }

    void AddBlockToRoster(List<Worker> block)
    {
        foreach(Worker worker in block)
        {
            names.text += " " + worker.name.Compound(); //padding with space because Scrollview is bloody stupid! -_-
            names.text += "\n";
            roles.text += Worker.WorkerTypeString(worker.type);
            roles.text += "\n";
            health.text += worker.health.ToString();
            health.text += "\n";
            sanity.text += worker.sanity.ToString();
            sanity.text += "\n";
            food.text += worker.food.ToString();
            food.text += "\n";
        }
    }

    public void Close()
    {
        ToggleVisibility(false);
        this.enabled = false;
    }

    void ToggleVisibility(bool isShown)
    {
        this.transform.Find("Titles").gameObject.SetActive(isShown);
        this.transform.Find("Scroll View").gameObject.SetActive(isShown);
        this.transform.Find("Close").gameObject.SetActive(isShown);
        this.GetComponent<RawImage>().enabled = isShown;
    }

}

