using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MC_Door : MC_Prefab
{
    private int doorId;

    public void SetDoorId(int _id)
    {
        doorId = _id;
    }

    public int GetDoorId()
    {
        return doorId;
    }
}
