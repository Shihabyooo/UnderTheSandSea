using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsManager : MonoBehaviour
{
    [SerializeField] BuildingsDatabase database = new BuildingsDatabase();
    [SerializeField] BuildingLimits buildingLimits = new BuildingLimits();

    BuildingProposal currentProposal = null;
    public ConstructedBuildings constructedBuildings {get; private set;}

    public void Initialize()
    {
        constructedBuildings = new ConstructedBuildings();
        
        if (currentProposal != null)
            currentProposal.Cancel();
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

    public bool CanConstruct (BuildingType buildingType)
    {

        if (constructedBuildings.Count(buildingType) >= buildingLimits.MaxCount(buildingType))
            return false;
        
        return true;
    }

    public bool CanConstruct (int buildingID) //for ease of use with BuildingButton
    {
        return CanConstruct(GetBuildingStats(buildingID).type);
    }

    public uint TotalAvailableBeds()
    {
        uint count = 0;

        foreach (SleepingTent tent in constructedBuildings.sleepingTents)
        {
            count += tent.AvailableBeds();
        }

        return count;
    }

    public SleepingTent GetVacantBed() //returns a random SleepingTent with at least one empty bed.
    {
        List<SleepingTent> tentsWithAvailableBeds = new List<SleepingTent>();

        foreach (SleepingTent tent in constructedBuildings.sleepingTents)
        {
            if (!tent.isUnderConstruction && tent.AvailableBeds() > 0)
                tentsWithAvailableBeds.Add(tent);
        }

        if (tentsWithAvailableBeds.Count < 1) //no avaiable beds.
            return null;

        return tentsWithAvailableBeds[Random.Range(0, tentsWithAvailableBeds.Count - 1)];
    }

    public ulong ComputeBuildingsExpenses()
    {
        return constructedBuildings.TotalExpenses();
    }

    public void CleanUp()
    {
        constructedBuildings.Clear();
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
            
            GameManager.simMan.finances.SubtractFunds(targetBuilding.GetComponent<Building>().GetStats().cost);
            targetBuilding.GetComponent<Building>().BeginConstruction(cell);
            GameManager.uiMan.UpdateConstructionButtons();
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
            targetBuilding.transform.Rotate(0.0f, 0.0f , buildingRotationIncrements * Mathf.Sign(direction));
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

[System.Serializable]
class BuildingLimits
{
    [SerializeField] uint hq = 1;
    [SerializeField] uint sleepingTent = 5;
    [SerializeField] uint canteen = 2;
    [SerializeField] uint fieldHospital = 2;
    [SerializeField] uint lounge = 2;
    [SerializeField] uint geologyLab = 1;
    [SerializeField] uint latrine = 3;

    public uint MaxCount(BuildingType buildingType)
    {
        switch(buildingType)
        {
            case BuildingType.hq:
                return hq;
            case BuildingType.sleepTent:
                return sleepingTent;
            case BuildingType.canteen:
                return canteen;
            case BuildingType.fieldHospital:
                return fieldHospital;
            case BuildingType.lounge:
                return lounge;
            case BuildingType.geologyLab:
                return geologyLab;
            case BuildingType.latrine:
                return latrine;
            default:
                return 0;
        }
    }
}

public class ConstructedBuildings
{
    public List<Building> all {get; private set;}
    public List<HQ> hqs {get; private set;}
    public List<SleepingTent> sleepingTents {get; private set;}
    public List<FieldHospital> fieldHospitals {get; private set;}
    public List<Canteen> canteens {get; private set;}
    public List<GeologyLab> geologyLabs {get; private set;}
    public List<Latrine> latrines {get; private set;}
    public List<Lounge> lounges {get; private set;}


    public ConstructedBuildings()
    {
        all = new List<Building>();
        hqs = new List<HQ>();
        sleepingTents = new List<SleepingTent>();
        canteens = new List<Canteen>();
        fieldHospitals = new List<FieldHospital>();
        geologyLabs = new List<GeologyLab>();
        latrines = new List<Latrine>();
        lounges = new List<Lounge>();
    }


    public void AddBuilding(Building newBuilding)
    {

        all.Add(newBuilding);

        switch(newBuilding.GetStats().type)
        {
            case BuildingType.sleepTent:
                sleepingTents.Add(newBuilding.gameObject.GetComponent<SleepingTent>());
                break;
            case BuildingType.canteen:
                canteens.Add(newBuilding.gameObject.GetComponent<Canteen>());
                break;
            case BuildingType.fieldHospital:
                fieldHospitals.Add(newBuilding.gameObject.GetComponent<FieldHospital>());
                break;
            case BuildingType.geologyLab:
                geologyLabs.Add(newBuilding.gameObject.GetComponent<GeologyLab>());
                break;
            case BuildingType.hq:
                hqs.Add(newBuilding.gameObject.GetComponent<HQ>());
                break;
            case BuildingType.latrine:
                latrines.Add(newBuilding.gameObject.GetComponent<Latrine>());
                break;
            case BuildingType.lounge:
                lounges.Add(newBuilding.gameObject.GetComponent<Lounge>());
                break;
            default:
                break;
        }
    }

    public bool RemoveBuilding(Building building)
    {
        all.Remove(building);
        switch(building.GetStats().type)
        {
            case BuildingType.sleepTent:
                return sleepingTents.Remove(building.gameObject.GetComponent<SleepingTent>());
            case BuildingType.canteen:
                return canteens.Remove(building.gameObject.GetComponent<Canteen>());
            case BuildingType.fieldHospital:
                return fieldHospitals.Remove(building.gameObject.GetComponent<FieldHospital>());
            case BuildingType.geologyLab:
                return geologyLabs.Remove(building.gameObject.GetComponent<GeologyLab>());
            case BuildingType.hq:
                return hqs.Remove(building.gameObject.GetComponent<HQ>());
            case BuildingType.latrine:
                return latrines.Remove(building.gameObject.GetComponent<Latrine>());
            case BuildingType.lounge:
                return lounges.Remove(building.gameObject.GetComponent<Lounge>());
            default:
                return false;
        }
    }

    public uint Count(BuildingType buildingType)
    {
         switch(buildingType)
        {
            case BuildingType.hq:
                return (uint)hqs.Count;
            case BuildingType.sleepTent:
                return (uint)sleepingTents.Count;
            case BuildingType.canteen:
                return (uint)canteens.Count;
            case BuildingType.fieldHospital:
                return (uint)fieldHospitals.Count;
            case BuildingType.geologyLab:
                return (uint)geologyLabs.Count;
            case BuildingType.latrine:
                return (uint)latrines.Count;
            case BuildingType.lounge:
                return (uint)lounges.Count;
            default:
                return 0;
        }
    }

    public uint CountActive(BuildingType buildingType) //returns count of buildings that have finished construction
    {
        uint count = 0;
         switch(buildingType)
        {
            case BuildingType.hq:
                foreach(Building building in hqs)
                {
                    if (!building.isUnderConstruction)
                        count++;
                }
                break;
            case BuildingType.sleepTent:
                foreach(Building building in sleepingTents)
                {
                    if (!building.isUnderConstruction)
                        count++;
                }
                break;
            case BuildingType.canteen:
                foreach(Building building in canteens)
                {
                    if (!building.isUnderConstruction)
                        count++;
                }
                break;
            case BuildingType.fieldHospital:
                foreach(Building building in fieldHospitals)
                {
                    if (!building.isUnderConstruction)
                        count++;
                }
                break;
            case BuildingType.geologyLab:
                foreach(Building building in geologyLabs)
                {
                    if (!building.isUnderConstruction)
                        count++;
                }
                break;
            case BuildingType.latrine:
                foreach(Building building in latrines)
                {
                    if (!building.isUnderConstruction)
                        count++;
                }
                break;
            case BuildingType.lounge:
                foreach(Building building in lounges)
                {
                    if (!building.isUnderConstruction)
                        count++;
                }
                break;
            default:
                break;
        }

        return count;
    }

    public ulong TotalExpenses()
    {
        ulong expenses = 0;

        foreach(Building building in all)
        {
            if (!building.isUnderConstruction)
                expenses += building.budget;
        }

        return expenses;
    }

    public void Clear()
    {
        foreach(Building building in all)
            MonoBehaviour.Destroy(building.gameObject);
        
        all.Clear();
        hqs.Clear();
        sleepingTents.Clear();
        canteens.Clear();
        fieldHospitals.Clear();
        geologyLabs.Clear();
        latrines.Clear();
        lounges.Clear();
    }

}
