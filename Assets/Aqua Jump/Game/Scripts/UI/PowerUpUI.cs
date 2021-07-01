using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpUI : MonoBehaviour
{
    private BasePowerUp m_powerUp;

    [SerializeField]
    private Image m_icon;

    [SerializeField]
    private Image m_timerShadowOverlay;

    public BasePowerUp powerUp => m_powerUp;

    public void SetPowerUp(BasePowerUp powerUp)
    {
        gameObject.SetActive(true);

        m_powerUp = powerUp;
        m_icon.sprite = m_powerUp.icon;
        m_timerShadowOverlay.fillAmount = 0;

        m_powerUp.onPowerUpEnded += OnPowerUpEnded;
    }

    private void OnPowerUpEnded()
    {
        m_powerUp.onPowerUpEnded -= OnPowerUpEnded;
        m_powerUp = null;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        m_timerShadowOverlay.fillAmount = 1 - (m_powerUp.time / m_powerUp.initialTime);
    }
}
