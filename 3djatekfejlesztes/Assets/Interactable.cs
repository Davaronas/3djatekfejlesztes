using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Action activated;
    public Action deactivated;

    public bool isActivated = false;
    public bool isOneWayActivation = false;
    public bool isInteractable = true;

    
    public void Interacted()
    {
        if(isOneWayActivation)
        {
            if (isActivated) { return; }
        }

        isActivated = !isActivated;

        if(isActivated)
        {
            activated?.Invoke();
        }
        else
        {
            deactivated?.Invoke();
        }
    }

    public void Interacted(bool _state)
    {
        if (isOneWayActivation)
        {
            if (isActivated) { return; }
        }

        if (_state)
        {
            isActivated = true;
            activated?.Invoke();
        }
        else
        {
            isActivated = false;
            deactivated?.Invoke();
        }
    }


}
