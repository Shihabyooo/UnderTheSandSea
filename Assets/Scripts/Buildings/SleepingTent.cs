using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepingTent : Building
{
    protected override void Awake()
    {
        base.Awake();
    }

    public uint AvailableBeds()
    {
        return (uint)Mathf.Max(stats.capacity - assignedWorkers.Count , 0);
    }

}
