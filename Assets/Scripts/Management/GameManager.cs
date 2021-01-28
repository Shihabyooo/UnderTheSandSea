using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(ControlManager))]
//[RequireComponent(typeof(ResourcesManager))]
[RequireComponent(typeof(SimulationManager))]
[RequireComponent(typeof(BuildingsManager))]
//[RequireComponent(typeof(ClimateManager))]
[RequireComponent(typeof(PopulationManager))]
//[RequireComponent(typeof(UIManager))]
//[RequireComponent(typeof(EconomyManager))]
public class GameManager : MonoBehaviour
{
    static public GameManager gameMan = null;
    static ControlManager controlMan = null;
    static public BuildingsManager buildMan = null;
    static public SimulationManager simMan;
    static public PopulationManager popMan;

    void Awake()
    {
        if (gameMan == null)
        {
            gameMan = this;
            controlMan = this.gameObject.GetComponent<ControlManager>();
            buildMan = this.gameObject.GetComponent<BuildingsManager>();
            simMan = this.gameObject.GetComponent<SimulationManager>();
            popMan = this.gameObject.GetComponentInChildren<PopulationManager>();
        }
        else
        {
            Destroy (this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
