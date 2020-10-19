using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    // Tunables
    [SerializeField] int defaultNumberOfLives = 3;

    // States
    int numberOfLives = 3; // Serialized for debug
    int score = 0; // Serialized for debug

    private void Awake()
    {
        int numberOfGameSessions = FindObjectsOfType<GameSession>().Length;
        if (numberOfGameSessions > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        numberOfLives = defaultNumberOfLives;
        score = 0;
    }

    public void ProcessPlayerDeath()
    {
        if (numberOfLives > 1)
        {
            TakeLife();
        }
        else
        {
            GameOverScreen();
        }
    }

    private void TakeLife()
    {
        numberOfLives--;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void GameOverScreen()
    {
        numberOfLives--;
        SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
    }

    public void ResetLives()
    {
        numberOfLives = defaultNumberOfLives;
    }

    public int GetLives()
    {
        return numberOfLives;
    }

    public void AddToScore(int scoreIncrement)
    {
        score += scoreIncrement;
    }

    public void ResetScore()
    {
        score = 0;
    }

    public int GetScore()
    {
        return score;
    }
}
