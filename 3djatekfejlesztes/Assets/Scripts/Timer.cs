using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    private float bestTime = -1;
    private float timer;

    private bool isTimerActive = false;

    private Text timerText = null;
    private Text bestTimeText = null;



    private void Start()
    {
        PlayerMovement.OnFirstMovement += StartTimer;
        DeathCollider.OnPlayerDeadlyCollision += StopTimer;
        EndMap.OnMapFinished += SaveTime;

        timerText = FindObjectOfType<Canvas>().transform.Find("Timer").GetComponent<Text>();
        bestTimeText = FindObjectOfType<Canvas>().transform.Find("BestTimeText").GetComponent<Text>();

        bestTime = PlayerPrefs.GetFloat($"Map{SceneManager.GetActiveScene().buildIndex}", -1);
        if(bestTime > -1)
        {
            bestTimeText.text = "Best: " + bestTime.ToString("F2");
        }
    }

    private void OnDestroy()
    {
        PlayerMovement.OnFirstMovement -= StartTimer;
        DeathCollider.OnPlayerDeadlyCollision -= StopTimer;
        EndMap.OnMapFinished -= SaveTime;
    }

    private void StartTimer()
    {
        isTimerActive = true;
    }

    private void StopTimer()
    {
        isTimerActive = false;
    }

    private void SaveTime()
    {
        StopTimer();

        if(timer < bestTime || bestTime == -1)
        {
            bestTime = timer;
            PlayerPrefs.SetFloat($"Map{SceneManager.GetActiveScene().buildIndex}", bestTime);
        }
    }

   

    private void Update()
    {
        if (isTimerActive)
        {
            timer += Time.deltaTime;
            timerText.text = timer.ToString("F2");
        }
    }

}
