using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MainUI : BaseUI
{
    public Action onRestart;

    [SerializeField]
    private Button m_resetButton;

    [SerializeField]
    private TextMeshProUGUI m_score;


    private void OnEnable()
    {
        m_resetButton.onClick.AddListener(HandleRestart);
    }

    private void OnDisable()
    {
        m_resetButton.onClick.RemoveListener(HandleRestart);
    }

    public void SetScore(int score)
    {
        m_score.text = score.ToString();
    }

    public void DisableRestart()
    {
        m_resetButton.gameObject.SetActive(false);
    }

    private void HandleRestart()
    {
        onRestart();
    }
}
