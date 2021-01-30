using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bankrupcy : ScenarioEvent
{
    protected override void Awake()
    {
        base.Awake();
        //scenarioImage = Resources.Load<Texture2D>("SandStorm");

        scenarioText = "[Placeholder bankrupcy message]";
        scenarioName = "Bankrupcy"; //test
    }

    public override void Play(System.DateTime date)
    {
        //print ("Playing Sandstorm event at" + this.gameObject.name);
        HandleBankrupcy();

        base.Play(date);
    }

    void HandleBankrupcy()
    {
        
    }

    public override void FinishEvent()
    {
        //GameManager.simMan.FinishEvent(this);
        GameManager.gameMan.ReturnToMainMenu();
        Destroy(this.gameObject);
    }
}
