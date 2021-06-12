using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameOverUI : BaseUI
{
    [SerializeField]
    private Button m_restartButton;

    [SerializeField]
    private TextMeshProUGUI m_score;

    [SerializeField]
    private TextMeshProUGUI m_highScore;

    public Action onRestart;

    private void OnEnable()
    {
        m_restartButton.onClick.AddListener(HandleRestart);
        Time.timeScale = 0;
    }

    private void OnDisable()
    {
        m_restartButton.onClick.RemoveListener(HandleRestart);
        Time.timeScale = 1;
    }

    public void SetScore(int score)
    {
        m_score.text = "Score: " + score.ToString();
    }

    public void SetHighscore(int score)
    {
        m_highScore.text = "Highscore: " + score.ToString();
    }

    private void HandleRestart()
    {
        onRestart();
    }
}
