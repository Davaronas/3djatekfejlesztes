using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MC_DuplicateButton : MC_PrefabButton
{
    private void Awake()
    {
        MC_Manager.OnObjectSelected += SetPrefab;
    }

    private void OnDestroy()
    {
        MC_Manager.OnObjectSelected -= SetPrefab;
    }


    private void SetPrefab(Transform _t)
    {
        if (_t.GetComponent<MC_Prefab>().prefabId >= 0)
        {
            heldPrefab = _t.gameObject;
        }
        else
        {
            heldPrefab = null;
        }

        
    }

}
