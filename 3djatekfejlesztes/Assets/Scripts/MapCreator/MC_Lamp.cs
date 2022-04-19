using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MC_Lamp : MC_Prefab
{
    [SerializeField] private Transform lampHead;

    private int lampId = 0;
    private int attachedLightControlId = 0;


    public override void Awake()
    {
        if (FindObjectOfType<MC_Manager>() == null)
        {
            Destroy(GetComponent<Collider>());
        }


        base.Awake();
    }
   

    public Vector3 GetLampHeadRotation()
    {
        return lampHead.localRotation.eulerAngles;
    }

    public void SetLampHeadRotation(Vector3 _eulers)
    {
        lampHead.localRotation = Quaternion.Euler(_eulers);
    }

    public int GetLampId()
    {
        return lampId;
    }

    public void SetLampId(int _id)
    {
        lampId = _id;
    }

    public int GetLightControlId()
    {
        return attachedLightControlId;
    }

    public void SetLightControlId(int _id)
    {
        attachedLightControlId = _id;
    }
}
