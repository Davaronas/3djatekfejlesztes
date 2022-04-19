using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private MainMenuAudio mma;

    private int mapId = 1;

    private void Start()
    {
        mma = FindObjectOfType<MainMenuAudio>();
        Cursor.lockState = CursorLockMode.None;
    }


    public void RemoteCall_LoadScene(int _id)
    {
        Cursor.lockState = CursorLockMode.Locked;
        mma.ClickSound();
        mapId = _id;
        Invoke(nameof(LoadSceneAfterClickSound), .3f);
    }

    public void RemoteCall_Quit()
    {
        Application.Quit();
    }


    private void LoadSceneAfterClickSound()
    {
        SceneManager.LoadSceneAsync(mapId);
    }

    public void OpenCustomMapDirectory()
    {
        string _dirPath = Application.persistentDataPath;

        _dirPath = _dirPath.Replace('/', '\\');

        print(Application.persistentDataPath);
        System.Diagnostics.Process.Start("explorer.exe", _dirPath);
    }

}

