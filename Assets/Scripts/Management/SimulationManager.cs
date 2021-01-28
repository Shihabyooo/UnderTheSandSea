using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    //public System.DateTime startingDate {get; private set;}
    public System.DateTime currentDate {get; private set;}

    [SerializeField] [Range(1900, 2030)] int startYear = 1960;
    [SerializeField] [Range(1, 12)] int startMonth = 1;

    public delegate void OnNewDay(System.DateTime date);
    public static OnNewDay onNewDay;

    public void InitializeSimulation()
    {
        currentDate = new System.DateTime(startYear, startMonth, 1);
    }


    //test
    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 20;

        GUI.Label(new Rect(10, 40, 100, 20), "Date: " + currentDate.ToString(), style);
    }
}
