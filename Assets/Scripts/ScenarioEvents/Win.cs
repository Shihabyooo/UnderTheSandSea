using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Win : ScenarioEvent
{
    protected override void Awake()
    {
        base.Awake();
        //scenarioImage = Resources.Load<Texture2D>("SandStorm");

        scenarioText = "Wait! You can win in this thing????????";
        scenarioEffectText = null;
        scenarioName = "Winning"; //test
    }

    public override void Play(System.DateTime date)
    {
        //print ("Playing Sandstorm event at" + this.gameObject.name);
        HandleWinning();

        base.Play(date);
    }

    void HandleWinning()
    {
        
    }

    public override void FinishEvent()
    {
        //GameManager.simMan.FinishEvent(this);
        GameManager.gameMan.ReturnToMainMenu();
        Destroy(this.gameObject);
    }
}
