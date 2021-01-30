using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandStorm : ScenarioEvent
{
    //Sandstorms reduce sanity of everyone by base 10, reduces health by base 5, have chance of generating expences to fix broken buildings.
    //Sandstorms can occur day or night.
    //Sandstorms have immediate effects (i.e. start the day/night they are generated, end immediatly after)

    protected override void Awake()
    {
        base.Awake();
        scenarioImage = Resources.Load<Texture2D>("SandStorm");

        scenarioText = "A sandstorm sweeps through the excavation camp, chipping away at your crew's morale.\nAll workers health -5.\nAll workers sanity -10";
        scenarioName = "Sandstorm"; //test
    }

    public override void Play(System.DateTime date)
    {
        print ("Playing Sandstorm event at" + this.gameObject.name);
        SandStormEffect();

        base.Play(date);
    }

    void SandStormEffect()
    {
        //Reduce everyone's sanity by 10
        GameManager.popMan.GlobalSanityChange(-10);
        //Reduce everyone's health by 5
        GameManager.popMan.GlobalFoodChange(-5);

        //FinishEvent();
    }

}
