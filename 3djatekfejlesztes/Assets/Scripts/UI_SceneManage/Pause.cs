using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public static Action<bool> OnGamePaused;

    private GameObject pausePanel;
    private Button exitToMainMenuButton;
    private Button exitToDesktopButton;

    

    private void Awake()
    {
        
        pausePanel = FindObjectOfType<Canvas>().transform.Find("PausePanel").gameObject;

        exitToMainMenuButton = pausePanel.transform.Find("ExitToMainMenu").GetComponent<Button>();
        exitToMainMenuButton.onClick.AddListener(() => LoadMainMenu());

        exitToDesktopButton = pausePanel.transform.Find("ExitToDesktop").GetComponent<Button>();
        exitToDesktopButton.onClick.AddListener(() => Quit());


        pausePanel.SetActive(false);
    }


    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);

    }

    public void Quit()
    {
        Time.timeScale = 1;
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(!pausePanel.activeSelf)
            {
                pausePanel.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                OnGamePaused?.Invoke(true);
                Time.timeScale = 0;
            }
            else
            {
                pausePanel.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                OnGamePaused?.Invoke(false);
                Time.timeScale = 1;
            }
        }
    }
}
