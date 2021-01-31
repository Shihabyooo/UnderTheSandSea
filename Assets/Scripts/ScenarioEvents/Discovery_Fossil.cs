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
        scenarioText += "\nEffects:\nFunds +500.\nWorker sanity +2.";
        scenarioName = "Discovery_Fossil"; //test
    }

    public override void Play(System.DateTime date)
    {
        print ("Playing Sandstorm event at" + this.gameObject.name);
        SandStormEffect();

        base.Play(date);
    }

    void SandStormEffect()
    {
        //Increase funds by 500
        GameManager.simMan.finances.AddFunds(500);

        //Restore everyone's sanity by 2
        GameManager.popMan.GlobalSanityChange(2);
    
        //FinishEvent();
    }

}
