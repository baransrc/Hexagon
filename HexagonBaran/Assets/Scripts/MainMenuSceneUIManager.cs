using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSceneUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI highScoreText;

    private void Awake()
    {
        DisplayHighscore();
    }

    private void DisplayHighscore()
    {
        var highScore = PlayerPrefs.GetInt(StoredVariables.Highscore, 0);

        highScoreText.text = "HIGHSCORE: " + highScore;
    }
    
    public void OnClickPlay()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OnClickExit()
    {
        Application.Quit();
    }
}
