using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MC_LightControl : MC_Prefab
{

    private int lightControlId = 0;

    public override void Awake()
    {
        if (FindObjectOfType<MC_Manager>() == null)
        {
            if (TryGetComponent<MeshRenderer>(out MeshRenderer _mr))
            {
                _mr.enabled = false;
            }
        }
        base.Awake();
    }


    public int GetLightControlId()
    {
        return lightControlId;
    }

    public void SetLightControlId(int _id)
    {
        lightControlId = _id;
    }

   
}
