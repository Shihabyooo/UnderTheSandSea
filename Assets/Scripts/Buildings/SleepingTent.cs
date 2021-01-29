using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepingTent : Building
{
    protected override void Awake()
    {
        base.Awake();
        stats.type = BuildingType.sleepTent;
        effectiveness = GameManager.simMan.simParam.baseSleepingTentEffectiveness;
    }

    public uint AvailableBeds()
    {
        return AvailableSlots();
    }

}
