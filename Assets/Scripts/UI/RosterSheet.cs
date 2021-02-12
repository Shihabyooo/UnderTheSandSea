using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RosterSheet : MonoBehaviour
{
    static public RosterSheet rosterSheet = null;

    TextMeshProUGUI names;
    TextMeshProUGUI roles;
    TextMeshProUGUI health;
    TextMeshProUGUI sanity;
    TextMeshProUGUI food;

    void Awake()
    {
        if (rosterSheet == null)
        {
            rosterSheet = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        names = this.transform.Find("Scroll View").Find("Viewport").Find("Content").GetComponent<TextMeshProUGUI>();
        roles = names.transform.Find("Roles").GetComponent<TextMeshProUGUI>();
        health = names.transform.Find("Health").GetComponent<TextMeshProUGUI>();
        sanity = names.transform.Find("Sanity").GetComponent<TextMeshProUGUI>();
        food = names.transform.Find("Food").GetComponent<TextMeshProUGUI>();
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

