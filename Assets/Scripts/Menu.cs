using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    public void StartFirstLevel()
    {
        // Cleanup for restart
        ScenePersist scenePersist = FindObjectOfType<ScenePersist>();
        if (scenePersist != null)
        {
            Destroy(scenePersist.gameObject);
        }
        GameSession gameSession = FindObjectOfType<GameSession>();
        gameSession.ResetLives();
        gameSession.ResetScore();
        SceneManager.LoadScene(1);
    }

    public void StartMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
