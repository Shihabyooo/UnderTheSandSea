using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour
{
    [SerializeField] int buildingID;
    Button button;
    //bool isLocked = false;
    public bool forceLocked = false; //for manual locking.

    void Awake()
    {
        button = this.gameObject.GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        //print ("Clicked from " + this.gameObject.name);
        GameManager.gameMan.SwitchToBuildingPlacement(buildingID);
    }

    public void UpdateState(long funds)
    {
        bool newState = CanAfford(funds)
                        && GameManager.buildMan.CanConstruct(buildingID)
                        && !forceLocked;

        SetButtonState(newState);

        print ("BUtton: " + this.gameObject.name + ", states: " + newState.ToString() + CanAfford(funds).ToString() + GameManager.buildMan.CanConstruct(buildingID).ToString() + (!forceLocked).ToString());
    }

    public void SetButtonLockState(bool lockState) //implies disabling button.
    {
        //isLocked = lockState;
        SetButtonState(!lockState);
    }

    void SetButtonState(bool state)
    {
        button.interactable = state;
    }

    bool CanAfford(long funds)
    {
        int buildingCost = (int)GameManager.buildMan.GetBuildingStats(buildingID).cost;

        if (buildingCost <= funds)
            return true;
        else
            return false;
    }
}
