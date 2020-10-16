using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverSceneUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI lastScoreText;

    private void Awake()
    {
        DisplayAndSaveScores();
    }

    private void DisplayAndSaveScores()
    {
        var highScore = PlayerPrefs.GetInt(StoredVariables.Highscore, 0);
        var lastScore = PlayerPrefs.GetInt(StoredVariables.Lastscore, 0);

        if (lastScore > highScore)
        {
            PlayerPrefs.SetInt(StoredVariables.Highscore, lastScore);
            highScoreText.text = "HIGHSCORE: " + lastScore;
            lastScoreText.text = "NEW HIGHSCORE!";

            return;
        }
        
        highScoreText.text = "HIGHSCORE: " + highScore;
        lastScoreText.text = "SCORE: " + lastScore;
    }
    
    public void OnClickPlayAgain()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OnClickMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
