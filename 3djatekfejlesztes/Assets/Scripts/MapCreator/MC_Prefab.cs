using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MC_Prefab : MonoBehaviour
{

    public int prefabId;
    [Space]
    private Collider col;

    MC_Manager mc_manager = null;

    [SerializeField] private bool destroyColliderIfNotMapCreator = false;
    [SerializeField] private bool destroyRigidbodyIfMapCreator = false;

    virtual public void Awake()
    {
        mc_manager = FindObjectOfType<MC_Manager>();
        col = GetComponent<Collider>();
        
        if(mc_manager == null)
        {
            Destroy(this);

            /*
            if(destroyColliderIfNotMapCreator)
            {
                Destroy(col);
            }
            */

            
        }
        else
        {
            if (destroyColliderIfNotMapCreator)
            {
                foreach (Collider _c in GetComponentsInChildren<Collider>())
                {
                    if (_c != col)
                    {
                        if (_c.GetComponent<DestructiblePart>() != null)
                        {
                            Destroy(_c.GetComponent<DestructiblePart>());
                        }
                        Destroy(_c);
                    }
                }
            }

            if (destroyRigidbodyIfMapCreator)
            {
                Destroy(GetComponent<Rigidbody>());
            }
        }

        MC_Manager.OnObjectSelected += NewSelection;
        MC_Manager.OnSwitchedFromMoveTool += SwitchedFromMoveTool;



       
    }

    private void OnDestroy()
    {
        MC_Manager.OnObjectSelected -= NewSelection;
        MC_Manager.OnSwitchedFromMoveTool -= SwitchedFromMoveTool;
    }


    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject() == true)
        {
            return;
        }

        MC_Manager.OnObjectSelected?.Invoke(transform);

        if (mc_manager.selectedTool == MC_Manager.MC_Tools.Select)
        {
            col.enabled = false;
        }
    }

    public void Created()
    {
        col.enabled = false;
    }


    private void NewSelection(Transform _t)
    {
        if(mc_manager.selectedTool != MC_Manager.MC_Tools.Select) { return; }

        if(_t != transform)
        {
            col.enabled = true;
        }
        
    }

    private void SwitchedFromMoveTool()
    {
        col.enabled = true;
    }

    
}
