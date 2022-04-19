using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MC_Interactable : MC_Prefab
{
    private int interactableId = 0;

    public void SetInteractableId(int _id)
    {
        interactableId = _id;
    }

    public int GetInteractableId()
    {
        return interactableId;
    }
}
