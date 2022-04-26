using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MC_Door : MC_Prefab
{
    private int attachedInteractableId;
    

    public void SetAttachedInteractableId(int _id)
    {
        attachedInteractableId = _id;
    }

    public int GetAttachedInteractableId()
    {
        return attachedInteractableId;
    }
}
