using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject pauseMenuUI;

    private bool _axisInUse = false;

    private void Awake()
    {
        pauseMenuUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //TODO change this to use axis
        if (Input.GetAxisRaw("Start") != 0.0f && !_axisInUse)
        {
            _axisInUse = true;
            
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
        else if (Input.GetAxisRaw("Start") == 0.0f && _axisInUse)
        {
            _axisInUse = false;
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1.0f;
        isPaused = false;
    }

    public void Quit()
    {
        Resume();
        SceneManager.LoadScene("MainMenu");
    }
    
    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0.0f;
        isPaused = true;
    }
}
