using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDoor : MonoBehaviour
{
    [SerializeField] private Interactable attachedInteractable = null;


    private void Start()
    {
        if(attachedInteractable == null) { return; }

        attachedInteractable.activated += Open;
        attachedInteractable.deactivated += Close;
    }

    private void OnDestroy()
    {
        if (attachedInteractable == null) { return; }

        attachedInteractable.activated -= Open;
        attachedInteractable.deactivated -= Close;
    }


    private void Open()
    {
        gameObject.SetActive(false);
    }

    private void Close()
    {
        gameObject.SetActive(true);
    }

    public void SetInteractable(Interactable _i)
    {
        attachedInteractable = _i;
    }
}
