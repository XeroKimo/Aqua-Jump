using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class MainUI : BaseUI
{
    public Action onRestart;

    [SerializeField]
    private Button m_resetButton;

    [SerializeField]
    private TextMeshProUGUI m_score;

    [SerializeField]
    private PowerUpUI[] m_powerUpUIs;


    private void OnEnable()
    {
        m_resetButton.onClick.AddListener(HandleRestart);
    }

    private void Start()
    {
        foreach(PowerUpUI powerUp in m_powerUpUIs)
        {
            powerUp.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        m_resetButton.onClick.RemoveListener(HandleRestart);
    }

    public void DisplayPowerUp(BasePowerUp powerUp)
    {
        PowerUpUI ui = m_powerUpUIs.FirstOrDefault(comp => comp.powerUp == powerUp);

        if(ui)
            return;

        ui = m_powerUpUIs.FirstOrDefault(comp => comp.powerUp == null);
        ui.SetPowerUp(powerUp);

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
