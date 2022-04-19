using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class EndMap : MonoBehaviour
{

    private Interactable interactable = null;

    public static Action OnMapFinished;

    void Start()
    {
        interactable = GetComponent<Interactable>();

        interactable.activated += FinishSimulation;
    }

    private void OnDestroy()
    {
        interactable.activated -= FinishSimulation;
    }

    private void FinishSimulation()
    {
        OnMapFinished?.Invoke();
        // temp
        SceneManager.LoadScene(0);

    }


    void Update()
    {
        
    }
}
