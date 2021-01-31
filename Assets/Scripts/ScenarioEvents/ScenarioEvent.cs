using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioEvent : MonoBehaviour
{
    public string scenarioName; //test, for some testin on the editor.
    public System.DateTime date {get; protected set;}
    protected Building building; //ref to building to be affected by this scenario.
    protected Worker worker; //ref to worker affected by this scenario.

    public Texture2D scenarioImage;// {get; protected set;}
    public string scenarioText {get; protected set;}

    protected virtual void Awake()
    {
        date = new System.DateTime();
        scenarioText = "[Place holder for scenario text]";
        scenarioName = "BaseScenarioEvent"; //test
    }

    public virtual void Initialize(System.DateTime date, Building targetBuilding = null, Worker targetWorker = null)
    {
        date = new System.DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
        building = targetBuilding;
        worker = targetWorker;
    }

    public virtual void Play(System.DateTime date)
    {
        //print ("Attempting to play a generic event scenario");
        EventViewer.viewer.Show(this);
    }

    public virtual void Cancel() //To be called only by simMan when overriding a set event.
    {
        Destroy(this.gameObject);
    }

    public virtual void FinishEvent()
    {
        //GameManager.simMan.RemoveScenarioEvent(this);
        GameManager.simMan.FinishEvent(this);
        Destroy(this.gameObject);
    }

}


public class EventsLists
{
    static public string[] environmentalEventsDay = { "Discovery_Fossil"};

    static public string[] environmentalEventsNight = {"SandStorm"};

}