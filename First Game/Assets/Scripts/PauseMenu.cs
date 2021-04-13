using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public PlayerState playerState;

    public void QuitGame()
    {
        Debug.Log("Quit game.");
        //playerState.ClosePause();
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        playerState.paused = false;
        SceneManager.LoadScene("MainMenu");
        //Application.Quit();
    }
}
