using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Discovery_Remains_Bad : ScenarioEvent
{
    //Bad remains discovery reduces sanity of crew by 10;

    protected override void Awake()
    {
        base.Awake();
        scenarioImage = Resources.Load<Texture2D>("HumanRemains");

        scenarioText = "No more than a few minutes into the work day, your excavators run into human remains. At first you suspect those remains to be ancient ones of ";
        scenarioText += "archaelogical interest, but further inspection tells you that those are relatively recent. You found the dead bodies of unlucky prospectors who ";
        scenarioText += "could not survive the harshness of the sand sea. This discovery greatly affects your crew's morale.";

        scenarioEffectText = "All workers' sanity -10.";
        scenarioName = "Discovery_Remains_Bad"; //test
    }

    public override void Play(System.DateTime date)
    {
        //print ("Playing Sandstorm event at" + this.gameObject.name);
        BadRemainsDiscovery();

        base.Play(date);
    }

    void BadRemainsDiscovery()
    {
        //Decrease everyone's sanity by 10
        GameManager.popMan.GlobalSanityChange(-10);
    }

    public override bool CheckRequirement() //IMPORTANT! This method is called before the event is initialized.
    {
        //can't discover something if we don't have exacavators.
        if (GameManager.popMan.Count(WorkerType.excavator) < 1)
            return false;


        return true;
    }
}
