using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeadsUpDisplayController : MonoBehaviour
{
    // Tunables
    [SerializeField] TextMeshProUGUI livesText = null;
    [SerializeField] TextMeshProUGUI scoreText = null;

    // Cached references
    GameSession gameSession = null;

    private void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
        UpdateLivesText();
        UpdateScoreText();
    }

    public void UpdateLivesText()
    {
        if (livesText != null)
        {
            livesText.text = gameSession.GetLives().ToString();
        }
    }

    public void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = gameSession.GetScore().ToString();
        }
    }
}
