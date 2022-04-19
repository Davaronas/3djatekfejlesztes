using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MC_Bridge : MC_Prefab
{
    private int bridgeId;
    private int attachedInteractableId;

    private Vector3 scaleTo;

    public override void Awake()
    {
        scaleTo = transform.localScale;
        base.Awake();
    }


    public int GetBridgeId()
    {
        return bridgeId;
    }

    public void SetBridgeId(int _id)
    {
        bridgeId = _id;
    }

    public int GetattachedInteractableId()
    {
        return attachedInteractableId;
    }

    public void SetAttachedInteractableId(int _id)
    {
        attachedInteractableId = _id;
    }

    public void SetScaleTo(Vector3 _scaleTo)
    {
        scaleTo = _scaleTo;
    }

    public Vector3 GetScaleTo()
    {
        return scaleTo;
    }
}