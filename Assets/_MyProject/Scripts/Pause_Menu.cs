using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause_Menu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public static bool InControlMenu = false;
    public GameObject pauseMenuUI;
    public GameObject equipMenuUI;
    public GameObject controlMenuUI;
    private AudioSource bgMusicAudioSource;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && InControlMenu == false)
        {
            if (GameIsPaused == false)
            {
                Pause();
            }     
            else if (GameIsPaused == true)
            {
                Resume();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape) && InControlMenu == true)
        {
            BackFromControlMenu();
        }

    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        equipMenuUI.SetActive(true);
        Time.timeScale = 1f;
        GameIsPaused = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        bgMusicAudioSource = GameObject.FindGameObjectWithTag("BGM").GetComponent<AudioSource>();
        bgMusicAudioSource.UnPause();

    }
    public void Pause()
    {

        pauseMenuUI.SetActive(true);
        equipMenuUI.SetActive(false);
        
        GameIsPaused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        bgMusicAudioSource = GameObject.FindGameObjectWithTag("BGM").GetComponent<AudioSource>();
        bgMusicAudioSource.Pause();
        Time.timeScale = 0.000001f;


    }

    public void LoadControlMenu()
    {
        pauseMenuUI.SetActive(false);
        controlMenuUI.SetActive(true);
        InControlMenu = true;
        
    }
    
        public void BackFromControlMenu()
    {
        pauseMenuUI.SetActive(true);
        controlMenuUI.SetActive(false);
        InControlMenu = false;
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        SceneManager.LoadScene(0);
    }
}
