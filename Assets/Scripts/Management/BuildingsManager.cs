using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsManager : MonoBehaviour
{
    [SerializeField] BuildingsDatabase database = new BuildingsDatabase();

    BuildingProposal currentProposal = null;
    public List<Building> constructedBuildings {get; private set;}
    public List<SleepingTent> sleepingTents {get; private set;}

    public void Initialize()
    {
        constructedBuildings = new List<Building>();
    }

    public BuildingStats GetBuildingStats(int buildingID) 
    {
        BuildingStats tempStats = new BuildingStats(database.GetStatsForBuilding(buildingID));
        
        return tempStats;
    }

    public BuildingProposal StartNewBuildingProposal(int buildingID)
    {
        currentProposal = new BuildingProposal(buildingID);
        return currentProposal;
    }

    public void AddBuilding(Building newBuilding)
    {
        constructedBuildings.Add(newBuilding);

        switch(newBuilding.GetStats().type)
        {
            case BuildingType.sleepTent:
                sleepingTents.Add(newBuilding.gameObject.GetComponent<SleepingTent>());
                break;
            case BuildingType.canteen:
                break;
            case BuildingType.fieldHospital:
                break;
            case BuildingType.geologistLab:
                break;
            case BuildingType.hq:
                break;
            case BuildingType.latrine:
                break;
            case BuildingType.lounge:
                break;
            default:
                break;
        }
    }

    public bool RemoveBuilding(Building building)
    {
        return constructedBuildings.Remove(building);
    }

    public SleepingTent AvailableBed() //returns a random SleepingTent with at least one empty bed.
    {
        List<SleepingTent> tentsWithAvailableBeds = new List<SleepingTent>();

        foreach (SleepingTent tent in sleepingTents)
        {
            if (tent.AvailableBeds() > 0)
                tentsWithAvailableBeds.Add(tent);
        }

        if (tentsWithAvailableBeds.Count < 1) //no avaiable beds.
            return null;

        return tentsWithAvailableBeds[Random.Range(0, tentsWithAvailableBeds.Count - 1)];
    }

    //=======================================================================================================================
    //=======================================================================================================================
    //I made this class within BuildingsManager to access some of the latter's private members without extra lines of code...
    //TODO review the choice above.
    public class BuildingProposal
    {
        public GameObject targetBuilding {get; private set;}
        const float buildingRotationIncrements = 90.0f;

        public BuildingProposal(int _targetBuildingID)//, BuildingsManager _buildingsManager)
        {
            //TODO once a mesh loader (or dedicated prefabs are created) for the mock avatar for the buildings, replace the line bellow.
            GameObject newProposedBuilding = GameManager.buildMan.database.GetBuildingObject(_targetBuildingID).gameObject;
            targetBuilding = GameObject.Instantiate(newProposedBuilding);

            if (targetBuilding == null)
                print ("WARNING! targetBuildingProposal is set to null, meaning no building of provided ID could be found.");
        }

      public bool CanConstructHere(Cell cell)
       {
            if (cell.isOccupied)// || !targetBuilding.GetComponent<Building>().CheckConstructionResourceRequirements(cell))
                return false;

            //TODO add logic to assess whether positition supports building of this type here.

            return true;
        }

        public bool Construct(Cell cell)
        {
            if (!CanConstructHere(cell))
                return false;
            
            targetBuilding.GetComponent<Building>().BeginConstruction(cell);
            targetBuilding = null;
                
            return true;
        }

        public void Cancel()
        {
            Destroy (targetBuilding);
            GameManager.buildMan.currentProposal = null;
        }

        public void MovePlan(Vector3 positition)
        {
            targetBuilding.transform.position = positition;
            //print ("moving to: " + positition);
        }

        public void RotatePlan(float direction)
        {
            targetBuilding.transform.Rotate(0.0f, buildingRotationIncrements * Mathf.Sign(direction), 0.0f);
        }
    }
}


[System.Serializable]
class BuildingsDatabase
{
    public Building[] buildings;

    public BuildingStats GetStatsForBuilding(int buildingID)
    {
        foreach (Building building in buildings)
        {
            if (building.GetStats().id == buildingID)
                return building.GetStats();
        }

        return null;
    }

    public GameObject GetBuildingObject(int buildingID)
    {
        foreach (Building building in buildings)
        {
            if (building.GetStats().id == buildingID)
                return building.gameObject;
        }

        return null;
    }

}