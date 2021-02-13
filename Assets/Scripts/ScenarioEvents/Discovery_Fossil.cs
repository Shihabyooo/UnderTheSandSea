using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Discovery_Fossil : ScenarioEvent
{
    //Fossil discovery adds funds, restores everyone's sanity by 2;

    protected override void Awake()
    {
        base.Awake();
        scenarioImage = Resources.Load<Texture2D>("Fossil");

        scenarioText = "One of your excavation crew runs towards you holding what first looked like a rock. Upon further inspection, you discover that you're looking at a fossil";
        scenarioText += "for an unrecorded species. You rush to the office to inform your sponsors and prepare a statement for the media.";
        
        scenarioEffectText = "Funds +500.\nAll workers' sanity +2.";

        scenarioName = "Discovery_Fossil"; //test
    }

    public override void Play(System.DateTime date)
    {
        //print ("Playing Sandstorm event at" + this.gameObject.name);
        FossilDiscoveryEffect();

        base.Play(date);
    }

    void FossilDiscoveryEffect()
    {
        //Increase funds by 500
        GameManager.simMan.finances.AddFunds(500);

        //Restore everyone's sanity by 2
        GameManager.popMan.GlobalSanityChange(2);
    }

    public override bool CheckRequirement() //IMPORTANT! This method is called before the event is initialized.
    {
        //can't discover something if we don't have exacavators.

        if (GameManager.popMan.Count(WorkerType.excavator) < 1)
            return false;


        return true;
    }

}
