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
        playerState.ClosePause();
        SceneManager.LoadScene("MainMenu");
        //Application.Quit();
    }
}
